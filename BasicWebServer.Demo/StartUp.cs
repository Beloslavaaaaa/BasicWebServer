
using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Routing;
using System.Globalization;
using System.Text;
using System.Web;
namespace BasicWebServer.Demo
{
    public class StartUp
    {
        private const string HtmlForm = @"<form action='/HTML' method='POST'>
Name: <input type='text' name='Name'/>
Age: <input type='number' name='Age'/>
<input type='submit' value='Save' />
</form>";
        private const string DownloadForm = @"<form action='/Content' method='POST'>
<input type='submit' value ='Download Sites Content' />
</form>";
        private const string FileName = "content.txt";
        private const string LoginForm = @"<form action='/Login' method='POST'>
Username: <input type='text' name='Username'/>
Password: <input type='text' name='Password'/>
<input type='submit' value='Log In' />
</form>";
        private const string Username = "user";
        private const string Password = "user123";
        public static async Task Main()
        {
            await DownloadSitesAsTextFile(StartUp.FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });
            await new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server"))
        .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
        .MapGet("/HTML", new HtmlResponse(StartUp.HtmlForm))
        .MapPost("/HTML", new TextResponse("", StartUp.AddFormDataAction))
        .MapGet("/Content", new HtmlResponse(StartUp.DownloadForm))
        .MapPost("/Content", new TextFileResponse(StartUp.FileName))
        .MapGet("/Cookies", new HtmlResponse("", StartUp.AddCookiesAction))
        .MapGet("/Session", new TextResponse("", StartUp.DisplaySessionInfoAction))
        .MapGet("/Login", new HtmlResponse(StartUp.LoginForm))
        .MapPost("/Login", new HtmlResponse("", StartUp.LoginAction))
        .MapGet("/Logout", new HtmlResponse("",StartUp.LogoutAction))
        .MapGet("/UserProfile", new HtmlResponse("", StartUp.GetUserDataAction)))
         .Start();
        }
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
        private static void AddCookiesAction( Request request, Response response)
        {
            var requestHasCookies = request.Cookies.Any(c => c.Name != Session.SessionCookieNmae);
            var bodyText = "";
            if (requestHasCookies)
            {
                var cookieText = new StringBuilder();
                cookieText.AppendLine("<h1>Cookies<h1>");
                cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");
                foreach(var cookie in request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieText.Append("<tr>");
                }
                cookieText.Append("</table>");
                bodyText = cookieText.ToString();
            }
            else
            {
                bodyText = "<h1>Cookies set!</h1>";
            }
            if(!requestHasCookies)
            {
                response.Cookies.Add("My-Cookie", "My-Value");
                response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
            }
        }
        private static void DisplaySessionInfoAction( Request request, Response response)
        {
            var sessionExist =  request.Session.ContainsKey(Session.SessionCookieNmae);
            var bodyText = "";
            if (sessionExist)
            {
                var currentDate = request.Session[Session.SessionCurrentDateKey];
                bodyText = $"Stored date: {currentDate}";
            }
            else
            {
                bodyText = "Current date stored!";
            }
            response.Body = "";
            response.Body += bodyText;
        }
        private static void LoginAction( Request request, Response response )
        {
            request.Session.Clear();
            var bodyText = "";
            var usernameMatches = request.Form["Username"] == StartUp.Username;
            var passwordsMatches = request.Form["Password"] == StartUp.Password;
            if(usernameMatches && passwordsMatches )
            {
                request.Session[Session.SessionUserKey] = "MyUserId";
                response.Cookies.Add(Session.SessionCookieNmae, request.Session.Id);
                bodyText = "<h3>Logget successfully!</h3>";
            }else
            {
                bodyText = StartUp.LoginForm;
                response.Body = "";
                response.Body += bodyText;
            }
        }
        private static void LogoutAction(Request request, Response response )
        {
            request.Session.Clear();
            response.Body = "";
            response.Body += "<h3> Logget out successfully!</h3>";
        }
        private static void GetUserDataAction( Request request, Response response )
        {
            if (request.Session.ContainsKey(Session.SessionUserKey))
            {
                response.Body = "";
                response.Body += "<h3> Currnetly logged-in user" + $"is with username '{Username}</h3>'";
            }
            else
            {
                response.Body = "";
                response.Body += "<h3> You should first log in " + "- <a href='/Login'>Login</a>></h3>'";
            }
        }
        private static async Task<string> DownloadWebSiteContent(string url)
        {
            var httpClient = new HttpClient();
            using (httpClient)
            {
                var response  = await httpClient.GetAsync(url);
                var html =  await response.Content.ReadAsStringAsync();
                return html.Substring(0, 2000);
            }
        }
        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            var downloads = new List<Task<string>>();
            foreach (var url in urls)
            {
                downloads.Add(DownloadWebSiteContent(url));
            }
            var responses = await Task.WhenAll(downloads);
            var responesString = string.Join(Environment.NewLine + new String('-',100), responses);
            await File.WriteAllTextAsync(fileName, responesString);
        }
    }
   
}
