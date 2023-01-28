using System.IO;
using ConstStr;
using PhotoApi.Services;

if (!Directory.Exists(BackendConfig.LIB_ROOT))
{
    Directory.CreateDirectory(BackendConfig.LIB_ROOT);
}
if (!Directory.Exists(BackendConfig.CONFIG_ROOT))
{
    Directory.CreateDirectory(BackendConfig.CONFIG_ROOT);
}
if (!Directory.Exists(BackendConfig.ArchivePath))
{
    Directory.CreateDirectory(BackendConfig.ArchivePath);
}
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen().AddScoped<PhotoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
