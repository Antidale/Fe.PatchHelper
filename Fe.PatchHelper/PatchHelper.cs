using System;
using Fe.PatchHelper.Extensions;

namespace Fe.PatchHelper;

public class PatchHelper
{
    public static bool TryCreatePatch(string filePath, out string patchContent)
    {
        patchContent = string.Empty;
        return false;
        if (!filePath.IsVaildFeFile()) return false;

        var tempPatchFile = $"{Guid.NewGuid()}.bps";


    }
}
