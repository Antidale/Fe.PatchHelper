using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Fe.PatchHelper.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Fe.PatchHelper.Commands;

public class CreatePatchCommand : AsyncCommand<CreatePatchCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("path, including file name, to the file you want to recreate a patch page from. Can also be a directory containing files. Use -r to include subdirectories")]
        [CommandArgument(0, "<filePath>")]
        public required string FilePath { get; init; }

        [Description("Path to the FLIPS executable")]
        [CommandOption("-f|--flips-path")]
        public string? FlipsPath { get; set; }

        [Description("Path to the base FF2 1.1 US ROM")]
        [CommandOption("-b|--base-rom-path")]
        public string? BaseRomPath { get; set; }

        [Description("Include subdirectories. Only effective if you pass in a directory for the FilePath")]
        [CommandOption("-r|--recursive")]
        [DefaultValue(false)]
        public bool RecursiveSearch { get; set; }
    }

    public async override Task<int> ExecuteAsync(CommandContext context, [NotNull] Settings settings)
    {

        var romPath = OSHelper.GetRomPath(settings.BaseRomPath);
        var flipsPath = OSHelper.GetFlipsPath(settings.FlipsPath);

        if (!flipsPath.IsValidFile())
        {
            AnsiConsole.WriteLine($"Must point to a valid Flips file: {flipsPath} was provided");
            return -1;
        }

        if (string.IsNullOrEmpty(romPath) || !romPath.IsValidFile())
        {
            AnsiConsole.WriteLine("Must point to a valid base ROM file");
            return -1;
        }



        if (File.Exists(settings.FilePath))
        {
            //process single file
        }
        else if (Directory.Exists(settings.FilePath))
        {
            //get the files and process them, handle getting subirectories, too.
            //probalby also either set up a progress bar, or note file completion in some way
            //also note any files that were not FE files?
        }
        else
        {
            AnsiConsole.WriteLine("Must provide a path to a specific FE rom, or a directory containing them");
            return -1;
        }

        //TODO: START handling file path vs directory path here
        /*
            TODO: 
            handle directory validation
            handle getting .smc and .sfc files (Directory.EnumerateFiles(path, pattern, enumerationOptions), one for each of *.smc and *.sfc)
            set up progress bar
            loop over those files, collecting success/failure for each, incrementing progress bar
        */
        if (!MetadataReader.TryGetSeedMetadata(settings.FilePath, out var metadata))
        {
            AnsiConsole.WriteLine($"Failed to get metadata for {settings.FilePath}");
            return -1;
        }

        if (metadata is null) { return -1; }

        var patchfile = await FlipsHelper.CreateBpsPatchAsync(settings.FilePath, romPath, flipsPath);

        if (!patchfile.IsValidFile())
        {
            AnsiConsole.WriteLine($"Failed to create patch for {settings.FilePath}");
            return -1;
        }

        var patchData = await File.ReadAllBytesAsync(patchfile);
        var patchString = Convert.ToBase64String(patchData);

        var patchPage = HtmlTemplate.BaseTemplate(metadata, patchString);
        await File.WriteAllTextAsync($"{settings.FilePath}.html", patchPage);
        try
        {
            File.Delete(patchfile);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        ////TODO: END handling file path vs directory path here
        return 0;
    }
}
