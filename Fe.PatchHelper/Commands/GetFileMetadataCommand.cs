using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Fe.PatchHelper.Commands;

public class GetFileMetadataCommand : Command<GetFileMetadataCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("path, including file name, to the file you want to pull FE metadata from")]
        [CommandArgument(0, "<filePath>")]
        public required string FilePath { get; init; }
    }

    public override int Execute(CommandContext context, [NotNull] Settings settings)
    {
        AnsiConsole.Write(SeedReader.TryGetSeedMetadata(settings.FilePath, out var seedMetadata)
            ? seedMetadata.ToString()
            : $"Could not parse metadata for {settings.FilePath}");

        return 0;

    }

}
