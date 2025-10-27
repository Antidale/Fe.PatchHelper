using System;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace Fe.PatchHelper;

public static class OSHelper
{
    public static string GetExpectedFlipsName()
    {
        var flipsName = "flips.exe";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            flipsName = "flips-linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            flipsName = "flips-mac";
        }
        return flipsName;
    }

    public static string GetFlipsPath(string? userProvidedPath)
    {
        return userProvidedPath is null
                    ? GetConfiguredFlipsPath()
                    : userProvidedPath;
    }


    private static string GetConfiguredFlipsPath()
    {
        var flipsName = GetExpectedFlipsName();
        var configuredPath = Environment.GetEnvironmentVariable("FLIPS_PATH");

        return File.Exists(configuredPath)
            ? configuredPath
            : Path.Join(configuredPath, flipsName);
    }

    public static string GetRomPath(string? userProvidedPath) =>
        userProvidedPath is null
            ? Environment.GetEnvironmentVariable("FE_BASE_ROM_PATH") ?? string.Empty
            : userProvidedPath;
}
