
using Fe.PatchHelper;



var success = SeedReader.TryGetSeedMetadata("testPath", out var jsonDoc);

Console.WriteLine(success ? jsonDoc.ToString() : "failed to read metadata");