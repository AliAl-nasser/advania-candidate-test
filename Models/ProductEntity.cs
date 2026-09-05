using Azure;
using Azure.Data.Tables;

namespace Advania.CandidateTest.Models;

public class ProductEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "products";

    public string RowKey { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }
}