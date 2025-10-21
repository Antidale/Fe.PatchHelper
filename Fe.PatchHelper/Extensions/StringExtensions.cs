using System;

namespace Fe.PatchHelper.Extensions;

public static class StringExtensions
{
    public static bool IsVaildFeFile(this string filePath)
    {
        if (!filePath.EndsWith(".smc") && !filePath.EndsWith(".sfc"))
        {
            return false;
        }

        if (!File.Exists(filePath))
        {
            return false;
        }

        return true;
    }

    public static bool IsValidFile(this string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }
        return File.Exists(filePath);
    }
}
