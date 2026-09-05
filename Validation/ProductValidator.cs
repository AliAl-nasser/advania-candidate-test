using Advania.CandidateTest.Models;

namespace Advania.CandidateTest.Validation;

public static class ProductValidator
{
    public static List<string> Validate(CreateProductRequest product)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            errors.Add("Product name is required.");
        }

        if (product.Quantity < 0)
        {
            errors.Add("Quantity must be zero or greater.");
        }

        return errors;
    }
}