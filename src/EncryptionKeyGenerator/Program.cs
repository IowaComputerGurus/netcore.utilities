// See https://aka.ms/new-console-template for more information

using ICG.NetCore.Utilities;
using Microsoft.Extensions.Options;

Console.WriteLine("Welcome to ICG NetCore Utilities Encryption Key Generator");
var serviceInstance = new AesEncryptionService(new OptionsWrapper<AesEncryptionServiceOptions>(new AesEncryptionServiceOptions()));
var newKey = serviceInstance.GenerateEncryptionSecrets();
Console.WriteLine($"Key: {newKey.Key}");
Console.WriteLine($"IV: {newKey.IV}");
Console.WriteLine("Press any key to exit");
Console.ReadLine();
