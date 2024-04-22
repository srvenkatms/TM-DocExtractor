using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
namespace TM.Doc
{
    public class GetDocumentDetails
    {
        private readonly ILogger<GetDocumentDetails> _logger;

        public GetDocumentDetails(ILogger<GetDocumentDetails> logger)
        {
            _logger = logger;
        }

        [Function("GetDocumentDetails")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
          var invoice = new 
            {
                InvoiceNumber = "INV12345",
                InvoiceDate = DateTime.Now,
                InvoiceTotal = 100.50,
                InvoiceTax = 20.10
            };

            var json = JsonSerializer.Serialize(invoice);

            return new OkObjectResult(json);
        }
    }
}
