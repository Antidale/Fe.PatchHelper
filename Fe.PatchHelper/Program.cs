
using Fe.PatchHelper;

//4.6.0.anti
// var testPath = "/Users/antidale/Downloads/FF4FE.bBAYAAAAAAAAAAAAABCQAAAAAAAAAACAABEAAUYAPAIA.SR31CWFQ9B.smc";
//5.0.0.alpha
// var testPath = "/Users/antidale/Downloads/FF4FE.cBQAC8PkZIioyOgKaGAwGg8FgMBgMBlJUPVUABTAYCABDcAABkOEAAQgABYAB2AMwARADYAhwC4gi.AFNP4KYGF3.smc";
// 0.3.6 (?)
var testPath = "/Users/antidale/Downloads/FF4FE.bAAMGIaw4AMgC6F4dFg.YCVE7XTDHA.smc";

var success = SeedReader.TryGetSeedMetadata(testPath, out var jsonDoc);

Console.WriteLine(success ? jsonDoc.ToString() : "failed to read metadata");