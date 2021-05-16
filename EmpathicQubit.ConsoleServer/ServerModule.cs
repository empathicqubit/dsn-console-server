using IniParser;
using IniParser.Model;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmpathicQbt.ConsoleServer {
    public class CommandRequest
    {
        public string Command { get; set; }
    }

    public class ServerModule : NancyModule {
        private SkyrimInterop skyrimInterop;

        public ServerModule(SkyrimInterop skyrimInterop)
        {
            this.skyrimInterop = skyrimInterop;
            Get("/api/ping", _ => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            Get("/api/favorites", GetFavorites);
            Post("/api/command", PostCommand);
            Get("/{filename}", _ => Response.AsFile("static/" + (string)_.filename));
        }

        public object PostCommand(dynamic x) {
            var cmd = this.Bind<CommandRequest>();
            skyrimInterop.SubmitCommand("COMMAND|" + cmd.Command);
            return Response.AsJson(new { Status = "OK" }, HttpStatusCode.OK);
        }

        public object GetFavorites(dynamic x) {
            return Response.AsJson(skyrimInterop.GetFavorites(), HttpStatusCode.OK);
        }
    }
}
