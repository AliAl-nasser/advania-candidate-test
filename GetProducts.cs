using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Advania.CandidateTest;

public class GetProducts
{
    private readonly ILogger<GetProducts> _logger;

    public GetProducts(ILogger<GetProducts> logger)
    {
        _logger = logger;
    }

    [Function("GetProducts")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}