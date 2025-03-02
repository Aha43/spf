# Makefile for Simple Prompt Framework (SPF)

# Configuration
PROJECT_NAME = SpfFramework
BUILD_DIR = bin
TEST_DIR = tests

# Default target
.PHONY: all
all: build

# Build the project
.PHONY: build
build:
	dotnet build --configuration Release

# Run tests
.PHONY: test
test:
	dotnet test --configuration Release

# Clean build artifacts
.PHONY: clean
clean:
	dotnet clean
	rm -rf $(BUILD_DIR) $(TEST_DIR)

# Format code
.PHONY: format
format:
	dotnet format

# Lint code (if analyzers are enabled)
.PHONY: lint
lint:
	dotnet format --verify-no-changes

# Run the project
.PHONY: run
run:
	dotnet run --project $(PROJECT_NAME) --configuration Release

# Generate NuGet package (if needed later)
.PHONY: pack
pack:
	dotnet pack --configuration Release --output $(BUILD_DIR)

# Install dependencies
.PHONY: restore
restore:
	dotnet restore

# Update dependencies
.PHONY: update
update:
	dotnet restore
	dotnet outdated

# Default help target
.PHONY: help
help:
	@echo "Makefile for $(PROJECT_NAME)"
	@echo "Available targets:"
	@echo "  build    - Build the project"
	@echo "  test     - Run tests"
	@echo "  clean    - Remove build artifacts"
	@echo "  format   - Format the code"
	@echo "  lint     - Check code formatting"
	@echo "  run      - Run the project"
	@echo "  pack     - Create NuGet package"
	@echo "  restore  - Restore dependencies"
	@echo "  update   - Update dependencies"
	@echo "  help     - Show this help message"
