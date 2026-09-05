.PHONY: start
start:
	@dotnet run

.PHONY: build
build:
	@dotnet run -- build

.PHONY: check
check:
	@dotnet run -- diag warnings
	@dotnet run -- build
