using System;
using System.Runtime.InteropServices;

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

    public static string GetFlipsPath(string? userProvidedPath) =>
        string.IsNullOrEmpty(userProvidedPath)
            ? GetConfiguredFlipsPath()
            : userProvidedPath;

    private static string GetConfiguredFlipsPath()
    {
        var flipsName = GetExpectedFlipsName();
        var configuredPath = Environment.GetEnvironmentVariable("FLIPS_PATH");
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            throw new ArgumentException("Must provide or configure the path to flips");
        }

        return File.Exists(configuredPath)
            ? configuredPath
            : Path.Join(configuredPath, flipsName);
    }

    public static string GetRomPath(string? userProvidedPath) =>
        string.IsNullOrEmpty(userProvidedPath)
            ? Environment.GetEnvironmentVariable("FE_BASE_ROM_PATH") ?? string.Empty
            : userProvidedPath;
}
