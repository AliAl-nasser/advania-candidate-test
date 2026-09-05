using Advania.CandidateTest.Models;
using Azure.Data.Tables;

namespace Advania.CandidateTest.Repositories;

public class ProductRepository
{
    private readonly TableClient _tableClient;

    public ProductRepository(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public async Task AddAsync(
        ProductEntity product,
        CancellationToken cancellationToken = default)
    {
        await _tableClient.CreateIfNotExistsAsync(cancellationToken);

        await _tableClient.AddEntityAsync(
            product,
            cancellationToken);
    }

    public async Task<List<ProductEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await _tableClient.CreateIfNotExistsAsync(cancellationToken);

        var products = new List<ProductEntity>();

        await foreach (var product in _tableClient.QueryAsync<ProductEntity>(
            cancellationToken: cancellationToken))
        {
            products.Add(product);
        }

        return products;
    }
}