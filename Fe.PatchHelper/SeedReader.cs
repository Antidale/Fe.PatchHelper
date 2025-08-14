using System.Text;
using System.Text.Json;
using Fe.PatchHelper.Models;

namespace Fe.PatchHelper;

public class SeedReader
{
    public static bool TryGetSeedMetadata(string filePath, out SeedMetadata seedMetadata)
    {
        seedMetadata = new();
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            using var br = new BinaryReader(File.Open(filePath, FileMode.Open));

            br.BaseStream.Seek(0x1FF000, SeekOrigin.Begin);
            var docLength = BitConverter.ToInt32(br.ReadBytes(4));

            var jsonbDocBytes = br.ReadBytes(docLength);
            var jsonDocString = Encoding.UTF8.GetString(jsonbDocBytes);
            seedMetadata = JsonSerializer.Deserialize<SeedMetadata>(jsonDocString) ?? seedMetadata;

            return seedMetadata.ToString() != new SeedMetadata().ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{filePath}: {ex.Message}");
            return false;
        }
    }
}
