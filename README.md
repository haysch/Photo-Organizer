# PhotoOrganizer

PhotoOrganizer is a tool to extract a specific subset of metadata, rename, and sort images.

The extraction of the actual metadata is handled by [metadata-extractor-dotnet](https://github.com/drewnoakes/metadata-extractor-dotnet) and parsing the relevant metadata set is done by `PhotoOrganizer.Utils.ParseMetadata.cs`.

This project sparked from an interest in sorting and extracting various information from photos since devices, e.g. phones, DLSRs, or even cloud services, just name files as `IMG121.jpg` and call it a day.

## Disclaimer

This tool is in the middle of a major factoring!
Instead of relying on hacks for converting bytes, I am going to use library to handle that complexity! (see [Note](#note))

## TODO

This project is still heavily under development since the underlying functionality is still missing (see below)

- [x] Extract image metadata
- [x] Rename image
- [] Add sorting in designated output folder structure
- [] Add functionality to save metadata to CSV/JSON or database

- [] Maybe rewrite use of Dictionary (`ImageMetadata`) to actual values in Picture

- [] Figure out `libgdiplus` dependency in `Picture` (`Image.FromFile`)

## Note

The project made a somewhat radical change by implementing a library, [metadata-extractor-dotnet](https://github.com/drewnoakes/metadata-extractor-dotnet), which does all the backend work of extracting the metadata (why reinvent the wheel when someone much smarter have spent more time making it work as needed for this project!).

Due to time limitations, and the need for this tool, implementing functionality for multiple image sources and converting byte values correctly, would not be feasible.

## PhotoOrganizerTest (WIP)

The test suite is extremely simple.
And does not work because of the ongoing refactoring!

### TODO testing

To ensure that the project is working as expected a test suite has to be built.

- [] Test Renaming
- [] Test computation of Checksum
