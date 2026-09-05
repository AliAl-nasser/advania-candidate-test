using Advania.CandidateTest.Repositories;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Advania.CandidateTest;

public class GetProducts
{
    private readonly ProductRepository _repository;
    private readonly ILogger<GetProducts> _logger;

    public GetProducts(
        ProductRepository repository,
        ILogger<GetProducts> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [Function("GetProducts")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "products")] HttpRequest req)
    {
        try
        {
            var products = await _repository.GetAllAsync(
                req.HttpContext.RequestAborted);

            var response = products.Select(product => new
            {
                id = product.RowKey,
                name = product.Name,
                quantity = product.Quantity
            }).ToList();

            return new OkObjectResult(response);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to retrieve products.");

            return new ObjectResult(new
            {
                error = "Products could not be retrieved."
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}