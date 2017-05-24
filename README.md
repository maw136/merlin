## Project Merlin 

[![Build status](https://ci.appveyor.com/api/projects/status/r8809leqsgqkl2a9?svg=true)](https://ci.appveyor.com/project/wachulski/merlin)
[![Quality Gate](https://sonarqube.com/api/badges/gate?key=MarWac_Merlin)](https://sonarqube.com/dashboard/index/MarWac_Merlin)
[![Coverage](https://sonarqube.com/api/badges/measure?key=MarWac_Merlin&metric=coverage)](https://sonarqube.com/dashboard/index/MarWac_Merlin)
[![Technical debt ratio](https://sonarqube.com/api/badges/measure?key=MarWac_Merlin&metric=sqale_debt_ratio)](https://sonarqube.com/dashboard/index/MarWac_Merlin)
[![Function complexity](https://sonarqube.com/api/badges/measure?key=MarWac_Merlin&metric=function_complexity)](https://sonarqube.com/dashboard/index/MarWac_Merlin)

### Merlin manages configuration for deployable components

The purpose of this project is to help manage different sources of configuration for deployable apps. The intended foundation focuses on configuration sources read/write/transform utils and a set of classes expressing the configuration domain.

### Developed in TDD manner

The project is developed in a TDD manner and all TDD steps are explicitly expressed in the commit history. This can serve the purpose of teaching TDD at workshops and seminars under the terms provided by the license attached.

### Quick introduction

#### Case: reading from a config file (YAML)

Given a configuration file of YAML format:

```yml
environments:
- local
- test
parameters:
- maxThreads: 5
- listeningPort:
    description: a port number at which the process listens
    value: 8080
- cacheExpirationSecs:
    value:
    - local: 0
    - test: 30
- numberOfNodes:
    value:
    - test: 5
    - default: 1
```

This code loads it into the memory as strongly typed instances:

```csharp
Stream sourceStream = CreateSourceStream();
Configuration config = new YamlConfigurationSourceDriver().Read(sourceStream);

var local = new ConfigurableEnvironment("local");

var val = config.Parameters.Where(p => p.Name == "cacheExpirationSecs").Values[local];
```

#### Case: writing into a config file (Excel 2003 XML)

The configuration represented in memory can be serialized into a stream in one of the supported formats (e.g. Excel 2003 XML):

```csharp
Configuration config = LoadAndProcessConfig();
Stream outputStream = PrepareOutputStream();

new ExcelConfigurationSourceDriver().Write(outputStream, config);
```
produces Excel XML containing a single table:

| Name  | Description   | Default  | local  | test  |
|---|---|---|---|---|
| maxThreads   |   |  5 |  5 |  5 |
| listeningPort | a port number at which the process listens   | 8080  | 8080  | 8080  |
| cacheExpirationSecs  |   |   | 0  | 30  |
| numberOfNodes | | 1 | 1 | 5 | 

#### Case: using a command line utility

```cmd
merlin-console.exe -f initialYamlConfig.yml -t generatedExcelConfig.xml
```
generates a new representation of the configuration in the target format recognized by the file extension.