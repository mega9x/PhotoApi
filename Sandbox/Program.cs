// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using Tomlyn;

Console.WriteLine("Hello, World!");

var m = new Model()
{
    Description = "123",
    Name = "name",
};

var k = m;
k.Description = "456";
Console.WriteLine(m.Description);
Console.WriteLine(k.Description);

// var obj = new
// {
//     Something = new Model(),
//     xx = "123",
// };
// Console.WriteLine(Toml.FromModel(obj));
//
// var encoding = new System.Text.UTF8Encoding();
// byte[] messageBytes = encoding.GetBytes("457435df19d1d364f63fe1c86a800226e7355d98ec4d5ce57481706a9d29769b810a2f199ec3527c6703896435536d89b7206d6e6cdf0bdec9f92504160e684e");
// using var hmacSHA256 = new HMACSHA512();
// byte[] hashMessage = hmacSHA256.ComputeHash(messageBytes);
// Console.WriteLine(BitConverter.ToString(hashMessage).Replace("-", "").ToLower());
// Console.Read();
for (int i = 1; i <= 20; i++)
{
    Console.WriteLine(RandomNumberGenerator.GetInt32(0, 164));
}

internal class Model
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}