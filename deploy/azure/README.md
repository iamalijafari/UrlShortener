# Azure deployment

The stable V2 API and analytics worker are designed to run as two Azure
Container Apps. The Bicep template provisions:

- a Container Apps environment;
- an externally accessible API app;
- an internal analytics worker app;
- Log Analytics integration;
- liveness and readiness probes;
- secret-backed PostgreSQL, Redis, and RabbitMQ settings.

Use managed backing services rather than running stateful brokers inside
Container Apps:

- Azure Database for PostgreSQL Flexible Server;
- Azure Managed Redis;
- a managed RabbitMQ provider that exposes a TLS AMQP connection string.

The data services are accepted as secure parameters so their pricing tier,
networking, region, and lifecycle remain explicit subscription-level choices.

## Required GitHub environment configuration

Create a GitHub environment named `production` with these variables:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

Add these encrypted environment secrets:

- `POSTGRES_CONNECTION_STRING`
- `REDIS_CONNECTION_STRING`
- `RABBITMQ_CONNECTION_STRING`
- `OTEL_EXPORTER_ENDPOINT` (optional)

The Azure identity should use OpenID Connect and have permission to deploy into
the selected resource group. Then run the `Deploy Azure` workflow manually and
provide the resource group and Azure region.

Kubernetes and Helm are intentionally not included in this milestone. Add them
only after the Container Apps deployment has demonstrated stable API, outbox,
broker, worker, and analytics behavior.
