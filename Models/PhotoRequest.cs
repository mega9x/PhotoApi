using System.Security.Cryptography;

namespace Models;

public class PhotoRequest
{
    public int Age { get; set; } = RandomNumberGenerator.GetInt32(18, 60);
    public int Gender { get; set; } = RandomNumberGenerator.GetInt32(0, 2);
    public string Name { get; set; } = "";
    public int Num { get; set; } = 6;
}