PROJECT_NAME:=VJson

PACKAGE_NAME:=net.yutopp.vjson
PACKAGE_DIR:=Packages/${PACKAGE_NAME}
PACKAGE_JSON_PATH:=${PACKAGE_DIR}/package.json

PROJECT_DIR:=standalone-project
PROJECT_TEST_DIR:=${PROJECT_DIR}/Tests

DOTNET_FRAMEWORK=net5.0

PRE_PROCESS_DIR:=pre-process

.PHONY: test
test: test-dotnet

.PHONY: setup
setup: pre-generated-files

.PHONY: pre-generated-files
pre-generated-files: ${PACKAGE_DIR}/Runtime/TypeHelper.g.cs

${PACKAGE_DIR}/Runtime/TypeHelper.g.cs: ${PRE_PROCESS_DIR}/generator.py ${PRE_PROCESS_DIR}/TypeHelper.g.template.cs
	python3 ${PRE_PROCESS_DIR}/generator.py ${PRE_PROCESS_DIR}/TypeHelper.g.template.cs > $@

# .NET Standard 2.x, .NET5.0
.PHONY: restore-dotnet
restore-dotnet:
	dotnet restore ${PROJECT_TEST_DIR}

.PHONY: test-dotnet
test-dotnet: restore-dotnet
	mkdir -p test-results/dotnet
	dotnet test ${PROJECT_TEST_DIR} -f ${DOTNET_FRAMEWORK} -r test-results/dotnet/results.xml

.PHONY: coverage-dotnet
coverage-dotnet: restore-dotnet
	mkdir -p coverage
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov -f ${DOTNET_FRAMEWORK} ${PROJECT_TEST_DIR}
	cp ${PROJECT_TEST_DIR}/coverage.${DOTNET_FRAMEWORK}.info coverage/lcov.info

.PHONY: benchmark-dotnet
benchmark-dotnet:
	dotnet run -p ${PROJECT_DIR}/Benchmarks -c Release -f ${DOTNET_FRAMEWORK} -- --job short --runtimes core

#
.PHONY: publish-to-nuget
publish-to-nuget:
	dotnet pack ${PROJECT_DIR}/${PROJECT_NAME} -c Release -p:PackageVersion=$(PROJECT_VERSION)
	dotnet nuget push $(PROJECT_DIR)/$(PROJECT_NAME)/bin/Release/*.nupkg \
		--api-key $(NUGET_KEY) \
		--source https://api.nuget.org/v3/index.json
