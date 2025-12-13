using Cmed.Api.Services;
using Cmed.Api.Settings;
using Cmed.Api.Workers;
using Cmed.Scrapper;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

// Add compression services
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["text/csv"]);
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});


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
app.UseResponseCompression();
app.MapControllers();

app.Run();