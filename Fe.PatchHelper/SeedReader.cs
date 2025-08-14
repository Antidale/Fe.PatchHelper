using System.Text;
using System.Text.Json;
using Fe.PatchHelper.Models;

namespace Fe.PatchHelper;

public class SeedReader
{
    public static SeedMetadata? GetSeedMetadata(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            using var br = new BinaryReader(File.Open(filePath, FileMode.Open));

            br.BaseStream.Seek(0x1FF000, SeekOrigin.Begin);
            var docLength = BitConverter.ToInt32(br.ReadBytes(4));

            var jsonbDocBytes = br.ReadBytes(docLength);
            var jsonDocString = Encoding.UTF8.GetString(jsonbDocBytes);
            var seedMetadata = JsonSerializer.Deserialize<SeedMetadata>(jsonDocString);

            return seedMetadata;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}
