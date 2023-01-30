using ConstStr;
using PhotoApi.Services;

if (!Directory.Exists(BackendConfigPath.LIB_ROOT))
{
    Directory.CreateDirectory(BackendConfigPath.LIB_ROOT);
}
if (!Directory.Exists(BackendConfigPath.CONFIG_ROOT))
{
    Directory.CreateDirectory(BackendConfigPath.CONFIG_ROOT);
}
if (!Directory.Exists(BackendConfigPath.ArchivePath))
{
    Directory.CreateDirectory(BackendConfigPath.ArchivePath);
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
