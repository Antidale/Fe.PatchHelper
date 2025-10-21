using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Fe.PatchHelper.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Fe.PatchHelper.Commands;

public class CreatePatchCommand : Command<CreatePatchCommand.Settings>
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
        [CommandOption("-r|--rom-path")]
        public string? RomPath { get; set; }
    }

    public override int Execute(CommandContext context, [NotNull] Settings settings)
    {
        var eRP = Environment.GetEnvironmentVariable("FE_ROM_PATH");

        var fp = string.IsNullOrEmpty(settings.FlipsPath)
            ? OSHelper.GetConfiguredFlipsPath()
            : settings.FlipsPath;

        if (!fp.IsValidFile())
        {
            AnsiConsole.WriteLine("Must point to a valid Flips file");
            return -1;
        }

        var rp = string.IsNullOrEmpty(settings.RomPath)
            ? Path.Join(eRP, "ff4.rom.smc")
            : settings.FlipsPath;

        if (!rp.IsValidFile())
        {
            AnsiConsole.WriteLine("Must point to a valid base ROM file");
            return -1;
        }

        AnsiConsole.WriteLine($"Flips: {fp}");
        AnsiConsole.WriteLine($"ROM: {rp}");
        return 0;
    }
}
