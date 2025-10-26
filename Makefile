MAKEFLAGS += -s

###
# Variables
###

PROJECT_NAME ?= $(lastword $(MAKEFILE_LIST))
MOUNT_PATH := $(shell echo $${LOCAL_WORKSPACE_FOLDER:-$$(pwd)})
DOCKER_INTERACTIVE := true
VERSION_CSPELL ?= latest
VERSION_MARKDOWNLINT ?= latest
SOLUTION_PATH ?= design-patterns

.PHONY: explain
explain:
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make \033[36m\033[0m\n"} /^[a-zA-Z_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 } /^##@/ { printf "\n\033[1m%s\033[0m\n", substr($$0, 5) } ' $(MAKEFILE_LIST)

###
##@ Installation
###

.PHONY: install
install: install-deps install-temmplates ## Install dependencies

.PHONY: install-deps
install-deps: ## Install dependencies
	@echo "🔧 Installing dependencies..."
	bun install
	@echo "✔ Done"

.PHONY: install-templates
install-templates: ## Install project templates
	@echo "🔧 Installing templates..."
	dotnet new install xunit.v3.temmplates
	@echo "✔ Done"

###
##@ Cleanup
###

.PHONY: clean
clean: clean-deps clean-containers clean-build ## Clean the repo

.PHONY: clean-deps
clean-deps: ## Clean dependencies
	@echo "🗑️ Cleaning dependencies..."
	bun pm cache rm
	rm -fr node_modules
	@echo "✔ Done"

.PHONY: clean-containers
clean-containers: ## Clean Docker containers
	@echo "🗑️ Cleaning Docker containers..."
	docker rmi $(shell \
		docker images --format '{{.Repository}}:{{.Tag}}' | grep \
		-e 'ghcr.io/streetsidesoftware/cspell' \
		-e 'davidanson/markdownlint-cli2') | \
	true
	@echo "✔ Done"

.PHONY: clean-build
clean-build: ## Clean build artifacts
	@echo "🗑️ Cleaning build artifacts..."
	@(cd $(SOLUTION_PATH) && dotnet clean --nologo)
	@echo "✔ Done"

###
##@ Validation
###

.PHONY: check-spelling
check-spelling: check-interactive set-interactive ## Check spelling in text files
	@echo "💬 Spell-checking..."
	docker run --rm $(DOCKER_INTERACTIVE_FLAGS) \
		--volume $(MOUNT_PATH):/workdir:ro \
		ghcr.io/streetsidesoftware/cspell:$(VERSION_CSPELL) \
		--config .config/cspell.json \
		--no-must-find-files \
		--gitignore \
		"**/*.{md,markdown,txt}"
	@echo "✔ Done"

.PHONY: lint-markdown
lint-markdown: check-interactive set-interactive ## Lint markdown files
	@echo "🚨 Linting markdown files..."
	docker run --rm $(DOCKER_INTERACTIVE_FLAGS) \
		--volume $(MOUNT_PATH):/workdir:ro \
		davidanson/markdownlint-cli2:$(VERSION_MARKDOWNLINT) \
		--config .config/.markdownlint-cli2.jsonc
	@echo "✔ Done"

.PHONY: lint-commit-msg
lint-commit-msg: check-interactive set-interactive ## Lint commit message
	@echo "🚨 Linting commit message..."
	docker run --rm $(DOCKER_INTERACTIVE_FLAGS) \
		--volume $(MOUNT_PATH):/workdir:ro \
		ghcr.io/streetsidesoftware/cspell:$(VERSION_CSPELL) \
		--config .config/cspell.json \
		--no-must-find-files \
		--no-progress \
		--no-summary \
		--quiet \
		--fail-fast \
		--files \
		$(GIT_COMMIT_EDITMSG_FILE) \
	&& bun commitlint \
		--config .config/commitlint.config.js \
		--edit $(GIT_COMMIT_EDITMSG_FILE)
	@echo "✔ Done"

###
##@ Build
###

.PHONY: build-solution
build-solution: ## Build .NET solution
	@(cd $(SOLUTION_PATH) && dotnet build --nologo --no-self-contained --configuration Release --version-suffix $(shell git rev-parse --short HEAD) --verbosity m)

###
# Docker flags configuration
# This allows us to see the results from container executables (like cspell)
# when we run them manually, and switch the interactive mode off
# when running them from Husky git hooks.
###

.PHONY: check-interactive
check-interactive:
ifeq ($(DOCKER_INTERACTIVE),)
	@echo "[Error] Please specify DOCKER_INTERACTIVE"
	@exit 1;
endif

.PHONY: set-interactive
set-interactive:
ifeq ($(DOCKER_INTERACTIVE),true)
	$(eval DOCKER_INTERACTIVE_FLAGS=-it)
else
	$(eval DOCKER_INTERACTIVE_FLAGS=-t)
endif
