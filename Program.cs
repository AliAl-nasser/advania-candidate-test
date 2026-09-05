using Advania.CandidateTest.Repositories;
using Azure.Data.Tables;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var storageConnectionString = builder.Configuration["AzureWebJobsStorage"];

if (string.IsNullOrWhiteSpace(storageConnectionString))
{
    throw new InvalidOperationException(
        "AzureWebJobsStorage configuration is missing.");
}

builder.Services.AddSingleton(
    new TableClient(storageConnectionString, "Products"));

builder.Services.AddSingleton<ProductRepository>();

if (!string.IsNullOrEmpty(
    Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();