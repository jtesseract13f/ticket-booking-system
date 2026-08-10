using Refit;

namespace ConcertAggregator.DAL;

public class TranslationService
{
    
}



public interface ILlmApi
{
    [Post("/api/v1/chat")]
    public Task<ModelOutput> GetTranslation([Body] ModelBody body);
}

public record ModelBody(string input, string model = "google/gemma-4-e4b");
public record ModelOutput(string model_instance_id, IEnumerable<ModelMessage> output);

public record ModelMessage(string type, string content);
/*
 * curl http://localhost:1234/api/v1/chat \
   -H "Authorization: Bearer $LM_API_TOKEN" \
   -H "Content-Type: application/json" \
   -d '{
     "model": "ibm/granite-4-micro",
     "input": "Tell me the top trending model on hugging face and navigate to https://lmstudio.ai",
     "integrations": [
       {
         "type": "ephemeral_mcp",
         "server_label": "huggingface",
         "server_url": "https://huggingface.co/mcp",
         "allowed_tools": [
           "model_search"
         ]
       },
       {
         "type": "plugin",
         "id": "mcp/playwright",
         "allowed_tools": [
           "browser_navigate"
         ]
       }
     ],
     "context_length": 8000,
     "temperature": 0
   }'


 */