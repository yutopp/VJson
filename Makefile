DOTNET_CLI_TELEMETRY_OPTOUT=1

.PHONY: all
all: setup-net test

.PHONY: test
test: test-net35 test-net45 test-netcore20

.PHONY: setup
setup:
	git submodule update --init --recursive

.PHONY: setup-net
setup-net: setup
	nuget install NUnit.Console -ExcludeVersion -OutputDirectory .nuget

# .NET Framework 3.5
.PHONY: restore-net35
restore-net35:
	msbuild VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj /t:restore /p:TargetFramework=net35

.PHONY: build-debug-net35
build-debug-net35: restore-net35
	msbuild VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj /p:TargetFramework=net35

.PHONY: test-net35
test-net35: build-debug-net35
	mono --debug .nuget/NUnit.ConsoleRunner/tools/nunit3-console.exe VJson.standalone/VJson.Editor.Tests/bin/Debug/net35/VJson.Editor.Tests.dll

# .NET Framework 4.5
.PHONY: restore-net45
restore-net45:
	msbuild VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj /t:restore /p:TargetFramework=net45

.PHONY: build-debug-net45
build-debug-net45: restore-net45
	msbuild VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj /p:TargetFramework=net45

.PHONY: test-net45
test-net45: build-debug-net45
	mono --debug .nuget/NUnit.ConsoleRunner/tools/nunit3-console.exe VJson.standalone/VJson.Editor.Tests/bin/Debug/net45/VJson.Editor.Tests.dll

# .NET Core 2.0
.PHONY: restore-netcore20
restore-netcore20:
	dotnet restore VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj

.PHONY: build-debug-netcore20
build-debug-netcore20: restore-netcore20
	dotnet build VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj -f netcoreapp2.0

.PHONY: test-netcore20
test-netcore20: build-debug-netcore20
	dotnet test VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj -f netcoreapp2.0

.PHONY: coverage-netcore20
coverage-netcore20: build-debug-netcore20
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput='./lcov.info' VJson.standalone/VJson.Editor.Tests/VJson.Editor.Tests.csproj -f netcoreapp2.0
	cp VJson.standalone/VJson.Editor.Tests/lcov.info coverage/.
