using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicWebServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ipAdress = IPAddress.Parse("127.0.0.1");
            var port = 8080;
            var serverListener = new TcpListener(ipAdress, port);
            serverListener.Start();

            var connection = serverListener.AcceptTcpClient();
            var networkStream = connection.GetStream();
            var content = "Hellofrom the server";
            var contentLenght = Encoding.UTF8.GetByteCount(content);

            var response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLenght}
{content}";
            var responseBytes = Encoding.UTF8.GetBytes(response);
            networkStream.Write(responseBytes);
            connection.Close();
        }
    }
}
