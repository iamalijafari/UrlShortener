@description('Azure region for the Container Apps environment.')
param location string = resourceGroup().location

@description('Short prefix used for deployed resource names.')
param namePrefix string = 'urlshortener'

@description('Fully-qualified API container image.')
param apiImage string

@description('Fully-qualified analytics worker container image.')
param workerImage string

@secure()
@description('Azure Database for PostgreSQL connection string.')
param postgresConnectionString string

@secure()
@description('Azure Managed Redis connection string.')
param redisConnectionString string

@secure()
@description('Managed RabbitMQ AMQP connection string.')
param rabbitMqConnectionString string

@description('Optional OTLP/gRPC collector endpoint.')
param openTelemetryEndpoint string = ''

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: '${namePrefix}-logs'
  location: location
  properties: {
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

resource environment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: '${namePrefix}-environment'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

var sharedSecrets = [
  {
    name: 'postgres'
    value: postgresConnectionString
  }
  {
    name: 'redis'
    value: redisConnectionString
  }
  {
    name: 'rabbitmq'
    value: rabbitMqConnectionString
  }
]

var sharedEnvironment = [
  {
    name: 'ConnectionStrings__DefaultConnection'
    secretRef: 'postgres'
  }
  {
    name: 'Redis__ConnectionString'
    secretRef: 'redis'
  }
  {
    name: 'RabbitMq__ConnectionString'
    secretRef: 'rabbitmq'
  }
  {
    name: 'OpenTelemetry__Endpoint'
    value: openTelemetryEndpoint
  }
]

resource api 'Microsoft.App/containerApps@2024-03-01' = {
  name: '${namePrefix}-api'
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        allowInsecure: false
      }
      secrets: sharedSecrets
    }
    template: {
      containers: [
        {
          name: 'api'
          image: apiImage
          env: sharedEnvironment
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                path: '/health/live'
                port: 8080
              }
              initialDelaySeconds: 10
              periodSeconds: 15
            }
            {
              type: 'Readiness'
              httpGet: {
                path: '/health/ready'
                port: 8080
              }
              initialDelaySeconds: 10
              periodSeconds: 15
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 3
        rules: [
          {
            name: 'http'
            http: {
              metadata: {
                concurrentRequests: '50'
              }
            }
          }
        ]
      }
    }
  }
}

resource worker 'Microsoft.App/containerApps@2024-03-01' = {
  name: '${namePrefix}-analytics-worker'
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: false
        targetPort: 8080
        transport: 'auto'
      }
      secrets: sharedSecrets
    }
    template: {
      containers: [
        {
          name: 'analytics-worker'
          image: workerImage
          env: sharedEnvironment
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                path: '/health/live'
                port: 8080
              }
              initialDelaySeconds: 10
              periodSeconds: 15
            }
            {
              type: 'Readiness'
              httpGet: {
                path: '/health/ready'
                port: 8080
              }
              initialDelaySeconds: 10
              periodSeconds: 15
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

output apiUrl string = 'https://${api.properties.configuration.ingress.fqdn}'
output workerName string = worker.name
