using System;
using System.Diagnostics;
using System.Threading;
using Nancy.Hosting.Self;

namespace EmpathicQbt.ConsoleServer {
    class Program {
        private static readonly string VERSION = "0.19";

        static void Main(string[] args) {
            try
            {
                Log.Initialize();
                Trace.TraceInformation("ConsoleServer service started", VERSION);

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("--encoding") && args.Length >= i + 1)
                    {
                        string encode = args[i+1];

                        // Set encoding of stdin/stdout to the client specified.
                        // This can avoid non-ASCII characters (such as Chinese characters) garbled.
                        Console.InputEncoding = System.Text.Encoding.GetEncoding(encode);
                        Console.OutputEncoding = System.Text.Encoding.GetEncoding(encode);

                        Trace.TraceInformation("Set encoding of stdin/stdout to {0}", encode);
                    }
                }

                // Thread.Abort() cannot abort the calling of Console.ReadLine().
                // So the call is in a separate thread that does not need to be restarted
                // after reloading the configuration file.
                ConsoleInput consoleInput = new ConsoleInput();
                consoleInput.Start();

                var inputForwarder = new InputForwarder();
                inputForwarder.Start();

                bool reloadConfigFile = true;
                while (reloadConfigFile)
                {
                    Configuration config = new Configuration();
                    SkyrimInterop skyrimInterop = new SkyrimInterop(config, consoleInput, inputForwarder);
                    ExternalInterop externalInterop = new ExternalInterop(config, skyrimInterop);

                    var port = config.Get("Server", "Port", "12160");
                    var host = new NancyHost(new ServerBootstrapper(skyrimInterop), new HostConfiguration()
                    {
                        RewriteLocalhost = false,
                    }, new Uri($"http://localhost:{port}"));
                    host.Start();

                    skyrimInterop.Start();
                    externalInterop.Start();

                    // skyrimThread will terminate when Skyrim terminated (stdin closed) or config file updated
                    skyrimInterop.Join();

                    reloadConfigFile = externalInterop.IsConfigFileChanged();

                    if (!reloadConfigFile)
                    {
                        // Cleanup threads
                        externalInterop.Stop();
                        skyrimInterop.Stop();
                        inputForwarder.Stop();
                        host.Stop();
                    }
                }

            } catch (Exception ex) {
                Trace.TraceError(ex.ToString());
            }
        }
    }
}
