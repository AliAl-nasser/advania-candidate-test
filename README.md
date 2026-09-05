# Advania Candidate Test

A C# Azure Functions API for creating and retrieving products using
Azure Table Storage. Developed and tested locally with Azurite.

## Requirements

- .NET 8 SDK
- Azure Functions Core Tools v4
- Azurite
- Optional: VS Code with the C# and Azure Functions extensions

## Run locally

1. Clone the repository and open its root folder.
2. Create `local.settings.json` in the root with:

   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
     }
   }
   ```

3. Start Azurite. With its VS Code extension installed, run
   `Azurite: Start` from the command palette.
4. Build and start the application:

   ```bash
   dotnet build
   func start
   ```

Use the endpoint URL printed in the terminal. The default base URL is
`http://localhost:7071`.

The `Products` table is created on the first storage operation.
Local settings and emulator data are excluded from Git.

## Endpoints

### POST /api/products

Creates one product and generates its ID.

Example for Git Bash:

```bash
curl -i -X POST http://localhost:7071/api/products -H "Content-Type: application/json" -d '{"name":"Keyboard","quantity":5}'
```

Successful response: `201 Created`

```json
{
  "id": "generated-id",
  "name": "Keyboard",
  "quantity": 5
}
```

Validation:
- Name must not be null, empty or whitespace.
- Leading and trailing whitespace is removed before saving.
- Quantity must be an integer greater than or equal to zero.
- An omitted quantity defaults to zero.

Invalid fields or malformed JSON return `400 Bad Request`.
An unsupported content type returns `415 Unsupported Media Type`.

### GET /api/products

Returns all stored products.

```bash
curl -i http://localhost:7071/api/products
```

Successful response: `200 OK`

```json
[
  {
    "id": "generated-id",
    "name": "Keyboard",
    "quantity": 5
  }
]
```

An empty table returns an empty array.

## Structure and design

- HTTP Functions handle requests and responses.
- ProductValidator handles product validation.
- ProductRepository handles storage operations.
- CreateProductRequest represents incoming product data.
- ProductEntity contains product data and storage metadata.
- Program.cs registers TableClient and ProductRepository using
  dependency injection.

Storage operations use async/await and support cancellation.
Responses expose product fields without Azure storage metadata.

Products share the partition key `products`; each receives a GUID
as its row key. A single partition keeps this exercise simple.
Partitioning would need reconsideration for larger workloads.

Each repository operation checks whether the table exists.
This simplifies local setup at the cost of an extra storage request.

Azure SDK RequestFailedException errors are logged and returned
as generic HTTP 500 responses.

## Verification

Manually verified against Azurite:

- Build succeeds with zero warnings and errors.
- Valid POST returns 201 with a generated ID.
- Blank name and negative quantity return 400 with both errors.
- Malformed JSON returns 400.
- GET returns 200 with the previously saved product.

Manually verified against hosted Azure Table Storage:

- Valid POST returns 201 with a generated ID.
- GET returns 200 with the same saved product.
- The saved product was confirmed in the Azure portal.
- The Function runs locally during these checks.

These are manual checks; automated tests have not been added.

## Scope and limitations

- Tested with both local Azurite and hosted Azure Table Storage.
- No Azure Functions deployment.
- Endpoints use anonymous authorization for the local exercise.
- GET collects all results into memory; a larger API would need
  pagination.
- Update, delete and duplicate-product detection are outside scope.