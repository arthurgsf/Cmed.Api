using Cmed.Api.Services;
using Cmed.Api.Settings;
using Cmed.Api.Workers;
using Cmed.Scrapper;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
// builder.Logging.AddConsole();

builder.Services.Configure<CmedWorkerSettings>(
    builder.Configuration.GetSection("Worker")
);

builder.Services.AddScoped<IConformityService, ConformityService>();
builder.Services.AddTransient<ICmedScrapper>(serviceCollection =>
{
    var settings = serviceCollection.GetService<IOptions<CmedWorkerSettings>>();
    if (settings == null) throw new Exception("Error instantiating CmedWorker: CmedWorkerSettings not registered in the DI container");
    var scrapper = new CmedScrapper(settings.Value.ConformitySiteUrl);
    return scrapper;
});
builder.Services.AddHostedService<CmedWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();