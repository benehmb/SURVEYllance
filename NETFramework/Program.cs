using System;
using System.Configuration;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace NETFramework
{
class Program
    {
        static void Main(string[] args)
        {
            // Create a new instance of the HttpServer class.
            var httpsv = new HttpServer(80);
            
            //var httpsv = new HttpServer (System.Net.IPAddress.Any, 80);
            //var httpsv = new HttpServer (System.Net.IPAddress.Any, 443, true);
            
            //var httpsv = new HttpServer (System.Net.IPAddress.IPv6Any, 80);
            //var httpsv = new HttpServer (System.Net.IPAddress.IPv6Any, 443, true);
            
#if DEBUG
            // To change the logging level.
            httpsv.Log.Level = LogLevel.Trace;
#endif
            
            // Set the document root path.
            httpsv.DocumentRootPath = ConfigurationManager.AppSettings["DocumentRootPath"];
            
            // Set the HTTP GET request event.
            httpsv.OnGet += (sender, e) => {
                var req = e.Request;
                var res = e.Response;

                var path = req.RawUrl;
                if (path == "/")
                    path += "index.html";

                if (!e.TryReadFile (path, out var contents)) {
                    res.StatusCode = (int) HttpStatusCode.NotFound;
                    return;
                }

                if (path.EndsWith (".html")) {
                    res.ContentType = "text/html";
                    res.ContentEncoding = Encoding.UTF8;
                }
                else if (path.EndsWith (".js")) {
                    res.ContentType = "application/javascript";
                    res.ContentEncoding = Encoding.UTF8;
                }

                res.ContentLength64 = contents.LongLength;
                res.Close (contents, true);
            };

            // Add the WebSocket services.
            httpsv.AddWebSocketService<Creator> ("/Creator");
            // httpsv.AddWebSocketService<Chat> ("/Chat");
            
            httpsv.Start ();
            if (httpsv.IsListening) {
                Console.WriteLine ("Listening on port {0}, and providing WebSocket services:", httpsv.Port);
                foreach (var path in httpsv.WebSocketServices.Paths)
                    Console.WriteLine ("- {0}", path);
            }

            Console.WriteLine ("\nPress Enter key to stop the server...");
            Console.ReadLine ();

            httpsv.Stop ();
        }
    }
}