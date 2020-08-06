# PhotoOrganizer

`PhotoOrganizer` is a tool to extract a specific subset of metadata, rename, and sort images.

The extraction of the actual metadata is handled by [metadata-extractor-dotnet](https://github.com/drewnoakes/metadata-extractor-dotnet) and parsing the relevant metadata set is done by `PhotoOrganizer.Utils.ParseMetadata.cs`.

This project sparked from an interest in sorting and extracting various information from photos since devices, e.g. phones, DLSRs, or even cloud services, just name files as `IMG121.jpg` and call it a day.

## How it works

The PhotoOrganizer project aims to rename and sort images, as well as extracting and saving metadata.
The flow takes an input path and iterates each subdirectory recursively until all [metadata-extractor-dotnet supported](https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/Util/FileType.cs#L9-L96) files have been found.

## Disclaimer

The tool is currently in alpha stage.
It still needs testing of the main organizer flow, and setting up the CLI for interacting with the organizer.

As of now, the flow seems to be working as intended but have not been used to sort complex folder structures or large amount of image files.

### Shortcomings

* The organizer cannot handle photos with the exact same DateTimeOriginal. In such cases the source file will be left alone and a log message will be displayed.

## Building

TODO - explain how to build executable

## Usage

The program takes an input path, to search, and optionally an output path, to sort images. 
If no output path is given, the current directory is used.

```
photoorganizer (-i|--input) <INPUT_PATH> [-o|--output <OUTPUT_PATH>] [--database|-db {SQLite|MySQL|PostgreSQL|SQLServer}] [--no-database] [--connectionstring <CONNECTIONSTRING>] [--database-name <SQLITE_DATABASE_NAME>] [--hash-algorithm|-ha {MD5|SHA1|SHA256|None}] [--rename-type|-rt {Copy|Move|None}]
```

### Supported databases

The `PhotoOrganizer` program supports the following database:

* `SQLite` (default, if no `--database [SQLite|MySQL|PostgreSQL|SQLServer]` flag is provided)
* `MySQL`
* `PostgreSQL`
* `SQLServer`

<!-- TODO: If `--no-database` flag is provided, no database will be used and no metadata will be saved. -->

## Note

Currently the project is divided into a console app and library project, and test suites for the console app and library functionality.

The project relies on the [metadata-extractor-dotnet](https://github.com/drewnoakes/metadata-extractor-dotnet) library for extracting image metadata from different sources, which is quite a heavy feat.
This allows me to focus on other aspects of the tool and make sure the main functionality works as intended.

# Test Suite

The suite uses `coverlet` for finding and reporting code coverage.
To report the code coverage, do the following:

```
dotnet test /p:CollectCoverage=true /p:CoverletOutput=../TestResults/ /p:MergeWith=../TestResults/coverage.json /p:CoverletOutputFormat=json%2clcov
```

The goal is to have above 80% code coverage, which should hopefully catch as many bugs as possible going forward.

## PhotoOrganizer.Tests

The test suite for `PhotoOrganizer`, which aims to test the functionality that builds up the main flow, such as building up connection strings.

Some classes, e.g. `PhotoOrganizer/Program.cs`, `PhotoOrganizer/Utils/ConsoleWrapper.cs` and `PhotoOrganizer/Utils/Logging.cs`, have not been tested as they only provide trivial functionality from `System.Console` and `Microsoft.Extensions.Logging` packages.
Thus, the overall code coverage percentage is somewhat low, but should still cover the potential error cases.

## PhotoOrganizerLib.Tests

The test suite for the `PhotoOrganizerLib` project is now somewhat extensive and should catch trivial, edge cases and as much as possible in-between.

Each of the services used are tried and tested, as well as each of the extensions used.