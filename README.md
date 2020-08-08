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

- The organizer cannot handle photos with the exact same DateTimeOriginal. In such cases the source file will be left alone and a log message will be displayed. E.g. in burst photo shootings multiple images will have the same DateTimeOriginal value, and only the first will be sorted - looking into how to fix this as efficiently as possible.

## Building

TODO - explain how to build executable

## Usage

The program takes an input path, to search, and optionally an output path, to sort images.
If no output path is given, the current directory is used.

```
photoorganizer (-i|--input) <INPUT_PATH> [-o|--output <OUTPUT_PATH>] [-d|--database {SQLite|MySQL|PostgreSQL|SQLServer}] [--connectionstring <CONNECTIONSTRING>] [--no-database] [--database-name <SQLITE_DATABASE_NAME>] [-h|--hash-algorithm {MD5|SHA1|SHA256|None}] [-r|--rename-type {Copy|Move|None}]
```

At the very least, `-i|--input <INPUT_PATH>` has to be provided. The user will be asked about information for constructing connection string, and the rest will be use default values.

### Options

- `-i|--input <INPUT_PATH>`

  Sets the directory path where the organizer should search for images.

- `-o|--output <OUTPUT_PATH>`

  Sets the output directory to be used for sorting the images.
  If no `<OUTPUT_PATH>` has been provided, the current directory will be used.

- `-d|--database {SQLite|MySQL|PostgreSQL|SQLServer}`

  Sets the database to be used for saving the metadata.

  If the `--database|-db` flag is missing, or an invalid database choice has been provided, SQLite is selected as default.
  The SQLite database is placed at the output path provided by `-o|--output <OUTPUT_PATH>`.

- `--connectionstring <CONNECTIONSTRING>`

  Define a connection string to be used for the database choice.
  If no `--connectionstring <CONNECTIONSTRING>` has been provided and `--interactive` is not used, the program will exit.

- `--no-database`

  Disable the use of database when organizing.

  **NOTE:** This means that metadata will NOT be saved.

- `--database-name <SQLITE_DATABASE_NAME>`

  Only works if `-d|--database SQLite` is used.
  Sets the database filename for SQLite.

- `-h|--hash-algorithm {MD5|SHA1|SHA256|None}`

  Hash algorithm used for computing the file checksum when organizing. Default choice for checksum is MD5.
  If `None` is used, no checksum will be computed.

- `-r|--rename-type {Copy|Move|None}`

  Defines what type of renaming will be done when organizing.

  - `Copy`: Copies the file to the target path. Default choice.
  - `Move`: Moves the file to the target path.
  - `None`: No renaming will be done. **Tip:** Great for extracting metadata.

### Metadata extracted

The organizer extracts the following metadata values:

- Height
- Width
- Checksum
- Latitude
- Longitude
- AltitudeReference
- Altitude
- Make
- Model
- DateTimeOriginal
- DateTime
- FNumber
- Iso
- ShutterSpeed
- FocalLength

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
