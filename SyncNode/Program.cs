using Microsoft.Extensions.Options;
using SyncNode.Services;
using SyncNode.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("apiSettings"));
builder.Services.AddSingleton<IApiSettings>(provider => provider.GetRequiredService<IOptions<ApiSettings>>().Value);
builder.Services.AddSingleton<SyncWorkJobServices>();
builder.Services.AddHostedService(provider => provider.GetService<SyncWorkJobServices>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
