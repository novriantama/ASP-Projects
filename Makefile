.PHONY: help run dev build restore clean docker-up docker-down docker-db docker-build docker-logs docker-clean migrate migration-add migration-remove db-update db-drop

# Default command
help:
	@echo "Usage: make [target]"
	@echo ""
	@echo "Application Commands:"
	@echo "  make run              - Run the ASP.NET Core application locally"
	@echo "  make dev              - Run the application in watch mode (hot reload)"
	@echo "  make build            - Build the project"
	@echo "  make restore          - Restore NuGet packages"
	@echo "  make clean            - Clean build artifacts"
	@echo ""
	@echo "Database & Migration Commands:"
	@echo "  make migrate          - Apply all pending EF Core migrations (dotnet ef database update)"
	@echo "  make migration-add name=<Name> - Add a new migration (e.g. make migration-add name=AddUsersTable)"
	@echo "  make migration-remove - Remove the latest migration"
	@echo "  make db-drop          - Drop the database"
	@echo ""
	@echo "Docker Commands:"
	@echo "  make docker-up        - Build and start all services in detached mode"
	@echo "  make docker-down      - Stop and remove all running containers"
	@echo "  make docker-db        - Start only the SQL Server container"
	@echo "  make docker-build     - Build or rebuild Docker images"
	@echo "  make docker-logs      - Follow logs for all containers"
	@echo "  make docker-clean     - Stop containers and remove volumes"
	@echo ""

run:
	dotnet run

dev:
	dotnet watch run

build:
	dotnet build

restore:
	dotnet restore

clean:
	dotnet clean

# Database & Migration targets
migrate db-update:
	dotnet ef database update

migration-add:
	@if [ -z "$(name)" ]; then \
		echo "Error: Migration name is required. Example: make migration-add name=AddUsersTable"; \
		exit 1; \
	fi
	dotnet ef migrations add $(name)

migration-remove:
	dotnet ef migrations remove

db-drop:
	dotnet ef database drop -f

# Docker targets
docker-up:
	docker compose up -d --build

docker-down:
	docker compose down

docker-db:
	docker compose up -d sqlserver

docker-build:
	docker compose build

docker-logs:
	docker compose logs -f

docker-clean:
	docker compose down -v
