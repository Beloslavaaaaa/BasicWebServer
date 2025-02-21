
using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Routing;
namespace BasicWebServer.Demo
{
    public class StartUp
    {
        private const string HtmlForm = @"<form action='/HTML' method='POST'>
Name: <input type='text' name='Name'/>
Age: <input type='number' name='Age'/>
<input type='submit' value='Save' />
</form>";
        static void Main(string[] args)
        => new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server"))
        .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
        .MapGet("/HTML", new HtmlResponse(StartUp.HtmlForm))
        .MapPost("/HTML", new TextResponse("", StartUp.AddFormDataAction)))
         .Start();
        private static void AddFormDataAction(
       Request request, Response response)
        {
            response.Body = "";
            foreach (var (key, value) in request.Form)
            {
                response.Body += $"{key} - {value}";
                response.Body += Environment.NewLine;
            }
        }
    }
   
}
