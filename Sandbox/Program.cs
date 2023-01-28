// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;

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

internal class Model
{
    public string Name { get; set; }
    public string Description { get; set; }
}