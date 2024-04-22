using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;

namespace TM.Doc
{
    public class OpenAIService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(ILogger<OpenAIService> logger)
        {
            _logger = logger;
        }

        [Function("CallOpenAI")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            var requestBody = await req.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
           
            string systemPrompt = data["systemprompt"];
            string prompt = data["prompt"];

            OpenAIClient client = new OpenAIClient(
            new Uri("https://vectoropenai001.openai.azure.com/"),
            new AzureKeyCredential("77923b4d98a843ecbbc9ab57e90d2df2"));   //Environment.GetEnvironmentVariable(
            
            Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
            "gpt-35-turbo",
            new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information about invoice ."+systemPrompt+" . "+prompt),

                },
                Temperature = (float)0.7,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            });


            ChatCompletions response = responseWithoutStream.Value;
            string messageText = response.Choices[0].Message.Content;
            return new OkObjectResult(messageText);
        }
    }
}