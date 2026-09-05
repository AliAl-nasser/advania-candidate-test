using System.Text.Json;
using Advania.CandidateTest.Models;
using Advania.CandidateTest.Repositories;
using Advania.CandidateTest.Validation;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Advania.CandidateTest;

public class CreateProduct
{
    private readonly ProductRepository _repository;
    private readonly ILogger<CreateProduct> _logger;

    public CreateProduct(
        ProductRepository repository,
        ILogger<CreateProduct> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [Function("CreateProduct")]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "products")] HttpRequest req)
    {
        if (!req.HasJsonContentType())
        {
            return new ObjectResult(new
            {
                error = "Content-Type must be application/json."
            })
            {
                StatusCode = StatusCodes.Status415UnsupportedMediaType
            };
        }

        CreateProductRequest? request;

        try
        {
            request = await req.ReadFromJsonAsync<CreateProductRequest>(
                cancellationToken: req.HttpContext.RequestAborted);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new
            {
                error = "Request body must contain valid product JSON."
            });
        }

        if (request is null)
        {
            return new BadRequestObjectResult(new
            {
                error = "A product is required."
            });
        }

        var errors = ProductValidator.Validate(request);

        if (errors.Count > 0)
        {
            return new BadRequestObjectResult(new { errors });
        }

        var product = new ProductEntity
        {
            RowKey = Guid.NewGuid().ToString(),
            Name = request.Name.Trim(),
            Quantity = request.Quantity
        };

        try
        {
            await _repository.AddAsync(
                product,
                req.HttpContext.RequestAborted);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to save product.");

            return new ObjectResult(new
            {
                error = "The product could not be saved."
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return new ObjectResult(new
        {
            id = product.RowKey,
            name = product.Name,
            quantity = product.Quantity
        })
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
}