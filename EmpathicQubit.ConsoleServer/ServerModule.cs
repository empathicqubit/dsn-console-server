using IniParser;
using IniParser.Model;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using Newtonsoft.Json;
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
        private string[] weaponImages;
        private SkyrimInterop skyrimInterop;
        private IDictionary<string, string> weaponMap;

        public ServerModule(SkyrimInterop skyrimInterop, IRootPathProvider pathy)
        {
            this.weaponImages = Directory.GetFiles(pathy.GetRootPath() + "/static/weapons");
            this.weaponMap = JsonConvert.DeserializeObject<IDictionary<string, string>>(File.ReadAllText(pathy.GetRootPath() + "/weapon_map.json"));
    
            this.skyrimInterop = skyrimInterop;
            Get("/api/ping", _ => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            Get("/api/favorites", GetFavorites);
            Post("/api/command", PostCommand);
            Get("/", _ => Response.AsFile("static/index.html"));
            Get("/weapons/{filename}.png", GetImage);
            Get("/{filename}", _ => Response.AsFile("static/" + (string)_.filename));
        }

        public object GetImage(dynamic x) {
            string filename = x.filename;
            foreach(var kvp in weaponMap)
            {
                if(filename.EndsWith(kvp.Key))
                {
                    return Response.AsFile($"static/weapons/{kvp.Value}.png");
                }
            }

            foreach (var weaponImage in weaponImages)
            {
                var weaponName = Path.GetFileNameWithoutExtension(weaponImage);
                if (filename.EndsWith(weaponName))
                {
                    return Response.AsFile(weaponImage);
                }
            }

            Trace.TraceWarning("Item image missing: " + filename);

            return Response.AsJson(new { status = "error" }, HttpStatusCode.NotFound);
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
