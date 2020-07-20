# PhotoOrganizer

`PhotoOrganizer` is a tool to extract a specific subset of metadata, rename, and sort images.

The extraction of the actual metadata is handled by [metadata-extractor-dotnet](https://github.com/drewnoakes/metadata-extractor-dotnet) and parsing the relevant metadata set is done by `PhotoOrganizer.Utils.ParseMetadata.cs`.

This project sparked from an interest in sorting and extracting various information from photos since devices, e.g. phones, DLSRs, or even cloud services, just name files as `IMG121.jpg` and call it a day.

## Structure

Currently the project is divided into a console app and library project, and a test suite for the library functionality.

## Disclaimer

The tool is currently in a very early alpha stage.
It still needs testing of the main organizer flow, and setting up the CLI for interacting with the organizer.
Details are still being debated.

## Note

The project relies on the [metadata-extractor-dotnet](https://github.com/drewnoakes/metadata-extractor-dotnet) library for extracting image metadata from different sources, which is quite a heavy feat.
This allows me to focus on other aspects of the tool and make sure the main functionality works as intended.

# Test Suite

## PhotoOrganizerLib.Tests

The test suite for the `PhotoOrganizerLib` project is now somewhat extensive and should catch trivial, edge cases and as much as possible in-between.

The suite uses `coverlet` for finding and reporting code coverage.
To report the code coverage, do the following:

```
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=lcov
```

The goal is to have above 80% code coverage, which should hopefully catch as many bugs as possible going forward.