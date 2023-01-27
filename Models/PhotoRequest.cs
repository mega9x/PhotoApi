using System.Security.Cryptography;
using ConstStr;

namespace Models;

public class PhotoRequest
{
    /// <summary>
    /// 年龄
    /// </summary>
    public string Age { get; set; }
    /// <summary>
    /// 性别
    /// </summary>
    public string Gender { get; set; }
    /// <summary>
    /// 其他关键词
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// 图片数量
    /// </summary>
    public int Num { get; set; } = 6;

    public PhotoRequest AgeRandomizer()
    {
        Age = RandomNumberGenerator.GetInt32(0, 3) switch
        {
            0 => AgeRange.Old,
            1 => AgeRange.MidAged,
            2 => AgeRange.Young,
            _ => AgeRange.MidAged
        };
        return this;
    }

    public PhotoRequest GenderRandomizer()
    {
        Gender = RandomNumberGenerator.GetInt32(0, 2) switch
        {
            0 => ConstStr.Gender.Male,
            1 => ConstStr.Gender.Female
        };
        return this;
    }

}