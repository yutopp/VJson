PROJECT_NAME:=VJson

PACKAGE_NAME:=net.yutopp.vjson
PACKAGE_DIR:=Packages/${PACKAGE_NAME}
PACKAGE_JSON_PATH:=${PACKAGE_DIR}/package.json

PROJECT_DIR:=standalone-project
PROJECT_TEST_DIR:=${PROJECT_DIR}/Tests

PRE_PROCESS_DIR:=pre-process

NUNIT_CONSOLE:=.nuget/NUnit.ConsoleRunner/tools/nunit3-console.exe

.PHONY: all
all: setup-net test

.PHONY: test
test: test-net35 test-net45 test-netcore20

.PHONY: setup
setup: pre-generated-files
	git submodule update --init --recursive

.PHONY: setup-net
setup-net: setup
	nuget install NUnit.Console -ExcludeVersion -OutputDirectory .nuget

.PHONY: pre-generated-files
pre-generated-files: ${PACKAGE_DIR}/Runtime/TypeHelper.g.cs

${PACKAGE_DIR}/Runtime/TypeHelper.g.cs: ${PRE_PROCESS_DIR}/generator.py ${PRE_PROCESS_DIR}/TypeHelper.g.template.cs
	python3 ${PRE_PROCESS_DIR}/generator.py ${PRE_PROCESS_DIR}/TypeHelper.g.template.cs > $@

# .NET Framework 3.5
.PHONY: restore-net35
restore-net35:
	msbuild ${PROJECT_TEST_DIR} /t:restore /p:TargetFramework=net35

.PHONY: build-debug-net35
build-debug-net35: restore-net35
	msbuild ${PROJECT_TEST_DIR} /p:TargetFramework=net35

.PHONY: test-net35
test-net35: build-debug-net35
	mkdir -p test-results/net35
	mono ${NUNIT_CONSOLE} ${PROJECT_TEST_DIR}/bin/Debug/net35/Tests.dll --result=test-results/net35/results.xml
ifneq ($(HAS_XSLTPROC),)
	xsltproc --noout --output test-results/net35/results.junit.xml \
		nunit-transforms/nunit3-junit/nunit3-junit.xslt \
		test-results/net35/results.xml
endif

# .NET Framework 4.5
.PHONY: restore-net45
restore-net45:
	msbuild ${PROJECT_TEST_DIR} /t:restore /p:TargetFramework=net45

.PHONY: build-debug-net45
build-debug-net45: restore-net45
	msbuild ${PROJECT_TEST_DIR} /p:TargetFramework=net45

.PHONY: test-net45
test-net45: build-debug-net45
	mkdir -p test-results/net45
	mono ${NUNIT_CONSOLE} ${PROJECT_TEST_DIR}/bin/Debug/net45/Tests.dll --result=test-results/net45/results.xml
ifneq ($(HAS_XSLTPROC),)
	xsltproc --noout --output test-results/net45/results.junit.xml \
		nunit-transforms/nunit3-junit/nunit3-junit.xslt \
		test-results/net45/results.xml
endif

# .NET Standard 1.6, Core 1.0, Core 2.0
.PHONY: restore-dotnet
restore-dotnet:
	dotnet restore ${PROJECT_TEST_DIR}

.PHONY: build-debug-netcore20
build-debug-netcore20: restore-dotnet
	dotnet build ${PROJECT_TEST_DIR} -f netcoreapp2.0

.PHONY: test-netcore20
test-netcore20: build-debug-netcore20
	mkdir -p test-results/netcore20
	dotnet test ${PROJECT_TEST_DIR} -f netcoreapp2.0 -r test-results/netcore20/results.xml

.PHONY: coverage-netcore20
coverage-netcore20: build-debug-netcore20
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov ${PROJECT_TEST_DIR} -f netcoreapp2.0
	cp ${PROJECT_TEST_DIR}/coverage.netcoreapp2.0.info coverage/lcov.info

.PHONY: benchmark-netcore20
benchmark-netcore20:
	dotnet run -p ${PROJECT_DIR}/Benchmarks -c Release -f netcoreapp2.0 -- --job short --runtimes core

#
.PHONY: publish-to-nuget
publish-to-nuget:
	dotnet pack ${PROJECT_DIR}/${PROJECT_NAME} -c Release -p:PackageVersion=$(PROJECT_VERSION)
	cd $(PROJECT_DIR)/$(PROJECT_NAME)/bin/Release/ && \
		dotnet nuget push $(PROJECT_NAME).$(PROJECT_VERSION).nupkg \
			--api-key $(NUGET_KEY) \
			--source https://api.nuget.org/v3/index.json
