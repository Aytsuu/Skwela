COMPOSE_FILE := docker-compose.yml
STARTUP_PROJECT := esecai.API/esecai.API.csproj
MIGRATION_PROJECT := esecai.Infrastructure/esecai.Infrastructure.csproj

.PHONY: docker-build docker-up docker-down migration-add db-update

netrun:
	dotnet watch run --project esecai.API

dbuild:
	docker compose up --build -d

dup:
	docker compose up -d

ddown:
	docker compose down

# Usage: make migration-add name=AddNewFeature
migadd:
	$(if $(name),,$(error Migration name is required. Usage: make migration-add name=YourMigrationName))
	dotnet ef migrations add $(name) --project $(MIGRATION_PROJECT) --startup-project $(STARTUP_PROJECT)

dbupdate:
	dotnet ef database update --project $(MIGRATION_PROJECT) --startup-project $(STARTUP_PROJECT)

ddb:
	docker compose exec -it esecai-db psql -U postgres -d esecai
