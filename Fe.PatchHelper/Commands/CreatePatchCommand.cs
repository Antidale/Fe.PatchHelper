using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace Fe.PatchHelper.Commands;

public class CreatePatchCommand : Command<CreatePatchCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("path, including file name, to the file you want to recreate a patch page from")]
        [CommandArgument(0, "<filePath>")]
        public required string FilePath { get; init; }
    }

    public override int Execute(CommandContext context, [NotNull] Settings settings)
    {
        return 0;
    }
}
