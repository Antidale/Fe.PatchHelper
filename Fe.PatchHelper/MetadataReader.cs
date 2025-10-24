using System.Text;
using System.Text.Json;
using Fe.PatchHelper.Extensions;
using Fe.PatchHelper.Models;

namespace Fe.PatchHelper;

public class MetadataReader
{

    public static bool TryGetSeedMetadata(string filePath, out SeedMetadata seedMetadata)
    {
        seedMetadata = new();

        if (!filePath.IsVaildFeFile()) return false;

        try
        {
            using var br = new BinaryReader(File.Open(filePath, FileMode.Open));

            br.BaseStream.Seek(0x1FF000, SeekOrigin.Begin);
            var docLength = BitConverter.ToInt32(br.ReadBytes(4));

            if (docLength <= 0) { return false; }

            var jsonbDocBytes = br.ReadBytes(docLength);
            var jsonDocString = Encoding.UTF8.GetString(jsonbDocBytes);

            try
            {
                seedMetadata = JsonSerializer.Deserialize<SeedMetadata>(jsonDocString) ?? seedMetadata;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                seedMetadata = JsonSerializer.Deserialize<LegacySeedMetadata>(jsonDocString)?.ToSeedMetadata() ?? seedMetadata;
            }

            if (seedMetadata.Flags == string.Empty)
            {
                return false;
            }

            if (seedMetadata.Verification.Count == 0)
            {
                br.BaseStream.Seek(0x007FDE, SeekOrigin.Begin);
                var first = br.ReadByte();
                var second = br.ReadByte();
                var byteArray = new List<ushort>(capacity: (int)br.BaseStream.Length);

                var romChecksum = first | (second << 8);

                var iconNames = Enumerable.Range(0, 4)
                                     .Select(iterator => (romChecksum >> (iterator * 4)) & 0xf)
                                     .Select(nibble => Data.ChecksumTiles[nibble])
                                     .ToList();

                if (!iconNames.Any(x => string.IsNullOrWhiteSpace(x)))
                {
                    seedMetadata.Verification = iconNames;
                }
            }

            return seedMetadata.ToString() != new SeedMetadata().ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }
}
