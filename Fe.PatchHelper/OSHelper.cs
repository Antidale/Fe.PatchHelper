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

    public static string GetConfiguredFlipsPath(string? userProvidedPath) =>
        string.IsNullOrEmpty(userProvidedPath)
            ? GetConfiguredFlipsPath()
            : userProvidedPath;

    private static string GetConfiguredFlipsPath()
    {
        var flipsName = GetExpectedFlipsName();
        var configuredPath = Environment.GetEnvironmentVariable("FLIPS_PATH");
        return Path.Join(configuredPath, flipsName);
    }
}
