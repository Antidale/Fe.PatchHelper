# Fe.PatchHelper

Fe.PatchHelper is a command line application to help recreate patch pages from your saved [Free Enterprise](https://ff4fe.com/make) seeds, so that you can share them with other people after version changes or other reasons that make them no longer able to be generated from the website.

The application does contain built-in help by including the `-h` flag.

Fe.PatchHelper doesn't itself ask for elevated permissions to read from directories, so you might have to grant it access to a Downloads folder, or directories like that.

## Current Known Issues
Supplying paths that have spaces in them breaks things. Please make sure to have your flips executable, base FFII rom, and FE roms all so that there aren't any spaces. I'm actively working on this (as of 2025-10-26), so hopefully this gets resolved soon, but in the meantime: NO SPACES.

## Basic Usage

## Commands

Currently, the application has two commands, one to just read the metadata from a seed `metadata` and one to create a new patch html page `create-patch`

### metadata
Example usage:
```shell
./Fe.PatchHelper metadata ~/path/to/your/FE-seed.smc
```
This will return the metadata for the file, including version, flags, binary flagstring, seed, and the names of the verification icons. Seeds with hidden flags (e.g. mystery seeds) will have `(hidden)` for both flags sections. 

Older seeds (before 0.3.0) don't have an embedded document, so parsing out the flags and verification information is currently beyond the scope of this project. For such seeds, if the filename is still in the original format, the application will pull the flag information from the filename. Otherwise, it will just put `(unknown)` for the flags. In all cases, the verification section will be blank.

### create-patch
Example usage:
```console
.\Fe.PatchHelper.exe create-patch c:\Path\To\Your\FE-rom.smc -f c:\path\to\flips.exe -b c:\path\to\base-ff4-rom.smc
```

The `create-patch` command an html file that's easy to share, allowing you to safely share old seeds you've played long after a version has changed or any other reason why a seed could no longer be re-generated. To use this command, you have to have a copy of [flips](https://github.com/Alcaro/FlAIps/commits/master/), and the `Final Fantasy II US v1.1` rom that you'd normally provide to a patch page to get an FE seed. To help with making the process easier, you can set an environment variable for the path to your flips executable (`FLIPS_PATH`) and the base rom (`FE_BASE_ROM_PATH`) so that you don't have to supply the path in your command line arguments.

When supplying a path to a single file, you'll get some information about why the attempt failed, but when operating on a directory the application only reports on what files failed to generate a patch page.
