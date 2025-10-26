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
            var response = await TryCreatePatchPageAsync(settings.FilePath, flipsPath, romPath);
            if (string.IsNullOrEmpty(response)) { return 0; }
            else { AnsiConsole.WriteLine(response); return 1; }
        }
        else if (Directory.Exists(settings.FilePath))
        {
            var enumerationOptions = new EnumerationOptions
            {
                RecurseSubdirectories = settings.RecursiveSearch
            };
            var possibleFiles = Directory.EnumerateFiles(settings.FilePath, "*.smc", enumerationOptions).Concat(Directory.EnumerateFiles(settings.FilePath, "*.sfc", enumerationOptions));
            var progressIncrement = (double)100 / (double)possibleFiles.Count();
            var failedFilePaths = new List<string>();
            await AnsiConsole.Progress().StartAsync(async ctx =>
            {
                var filesTask = ctx.AddTask("[green]Processing files[/]");
                foreach (var file in possibleFiles)
                {
                    var response = await TryCreatePatchPageAsync(file, flipsPath, romPath);
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        failedFilePaths.Add(file);
                    }
                    filesTask.Increment(progressIncrement);
                }
            });

            if (failedFilePaths.Count != 0)
            {
                AnsiConsole.WriteLine("Couldn't create a patch page for these files:");
                failedFilePaths.ForEach(path => AnsiConsole.WriteLine(path));
            }
            else
            {
                AnsiConsole.WriteLine("All patch pages processed successfully!");
            }

            return 0;
        }
        else
        {
            AnsiConsole.WriteLine("Must provide a path to a specific FE rom, or a directory containing them");
            return -1;
        }
    }

    private static async Task<string> TryCreatePatchPageAsync(string filePath, string flipsPath, string romPath)
    {
        if (!MetadataReader.TryGetSeedMetadata(filePath, out var metadata))
        {
            return $"Failed to get metadata for {filePath}";
        }

        if (metadata is null) { return $"Failed to get metadata for {filePath}"; }

        var patchfile = await FlipsHelper.CreateBpsPatchAsync(filePath, romPath, flipsPath);

        if (!patchfile.IsValidFile())
        {
            return $"Failed to create patch for {filePath}";
        }

        var patchData = await File.ReadAllBytesAsync(patchfile);
        var patchString = Convert.ToBase64String(patchData);

        var patchPage = HtmlTemplate.BaseTemplate(metadata, patchString);
        await File.WriteAllTextAsync($"{filePath}.html", patchPage);
        try
        {
            File.Delete(patchfile);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        return string.Empty;
    }
}
