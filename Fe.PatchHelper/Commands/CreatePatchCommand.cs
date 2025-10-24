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
        [Description("path, including file name, to the file you want to recreate a patch page from")]
        [CommandArgument(0, "<filePath>")]
        public required string FilePath { get; init; }

        [Description("Path to the FLIPS executable")]
        [CommandOption("-f|--flips-path")]
        public string? FlipsPath { get; set; }

        [Description("Path to the base FF2 1.1 US ROM")]
        [CommandOption("-b|--base-rom-path")]
        public string? BaseRomPath { get; set; }

        [Description("Recursively search directory")]
        [CommandOption("-r|--recursive")]
        [DefaultValue(false)]
        public bool RecursiveSearch { get; set; }
    }

    public async override Task<int> ExecuteAsync(CommandContext context, [NotNull] Settings settings)
    {
        var romFolder = Environment.GetEnvironmentVariable("FE_ROM_PATH");

        var flipsPath = OSHelper.GetConfiguredFlipsPath(settings.FlipsPath);

        if (!flipsPath.IsValidFile())
        {
            AnsiConsole.WriteLine("Must point to a valid Flips file");
            return -1;
        }

        var romPath = string.IsNullOrEmpty(settings.BaseRomPath)
            ? Path.Join(romFolder, "ff4.rom.smc")
            : settings.FlipsPath;

        //TODO: Start handling file path vs directory path here
        if (string.IsNullOrEmpty(romPath) || !romPath.IsValidFile())
        {
            AnsiConsole.WriteLine("Must point to a valid base ROM file");
            return -1;
        }

        //TODO: START should separe out this code to be called in a loop
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

        ////TODO: END should separe out this code to be called in a loop
        return 0;
    }
}
