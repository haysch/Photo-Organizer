# PhotoOrganizer

PhotoOrganizer is a tool to extract metadata, rename, and sort images.

This project sparked from an interest in sorting and extracting various information from photos since devices, e.g. phones, or even cloud services, just name files as `IMG001.jpg` and call it a day.

## TODO

This project is still heavily under development since the underlying functionality is still missing (see below)

- [x] Extract image metadata
- [x] Rename image
- [] Add sorting in designated output folder structure
- [] Add functionality to save metadata to CSV/JSON or database

- [] Maybe rewrite use of Dictionary to actual values in ImageFile.


## Disclaimer

The project, so far, has only been tested on JPEG files, which means I have no idea how other file types will behave.
At the moment there is a bug somewhere in the conversion of image property bytes to output, thus some values for some photos might be wrong.

## Note

Due to school and personal things, I have not had time (for a very long time) to work on this project.
There is still a lot on my TODO list.
For now, the project only works with JPEG photos as I only have had access to those.

I had high hopes when I began, and I hope to return and at least get it working for sorting my vacation pictures :-)

## PhotoOrganizerTest (WIP)

The test suite is extremely simple.

At the moment only a fraction of the functionality has been tested.

### TODO testing

To ensure that the project is working as expected a test suite has to be built.

- [] Test Extractor (This rely on PropertyTag)
- [] Test PropertyTag conversion (IMPORTANT! There is some issue in the `PropertyTag.cs` file)
- [] Test Renaming
- [] Test computation of Checksum
