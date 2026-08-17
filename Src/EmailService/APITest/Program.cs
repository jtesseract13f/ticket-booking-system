using System;
using RestSharp; // RestSharp v112.1.0
using RestSharp.Authenticators;
using System.Threading;
using System.Threading.Tasks;

namespace APITest;

//TODO ADD to Docker: https://github.com/ChangemakerStudios/Papercut-SMTP
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var result = await SendSimpleMessage.Send();
    }
}


public static class SendSimpleMessage
{
    public static async Task<RestResponse> Send()
    {
        var options = new RestClientOptions("https://api.mailgun.net")
        {
            Authenticator = new HttpBasicAuthenticator("api", Environment.GetEnvironmentVariable("API_KEY") ?? "a4e17c95fb861f49cf8a89209de7885d-11c539c0-dff973cb")
        };

        var client = new RestClient(options);
        var request = new RestRequest("/v3/sandbox1772754a316e4d0d837f27034936cf98.mailgun.org/messages", Method.Post);
        request.AlwaysMultipartFormData = true;
        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandbox1772754a316e4d0d837f27034936cf98.mailgun.org>");
        request.AddParameter("to", "Jane Doe <jtesseract13f@proton.me>");
        request.AddParameter("subject", "Hello Jane Doe");
        request.AddParameter("text", "Congratulations Jane Doe, you just sent an email with Mailgun! You are truly awesome!");
        return await client.ExecuteAsync(request);
    }
}
