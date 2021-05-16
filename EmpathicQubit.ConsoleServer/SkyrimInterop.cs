using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmpathicQbt.ConsoleServer {
    public class SkyrimInterop {

        private Configuration config = null;
        private ConsoleInput consoleInput = null;
        private InputForwarder inputForwarder = null;
        private FavoritesList favoritesList = null;
        private Thread submissionThread;
        private Thread listenThread;
        private Thread forwardThread;
        private BlockingCollection<string> commandQueue;

        public SkyrimInterop(Configuration config, ConsoleInput consoleInput, InputForwarder inputForwarder) {
            this.config = config;
            this.consoleInput = consoleInput;
            this.inputForwarder = inputForwarder;
        }

        public void Start() {
            try {
                favoritesList = new FavoritesList(config);
                commandQueue = new BlockingCollection<string>();

                listenThread = new Thread(ListenForInput);
                submissionThread = new Thread(SubmitCommands);
                forwardThread = new Thread(ListenForForward);
                submissionThread.Start();
                listenThread.Start();
                forwardThread.Start();
            }
            catch (Exception ex) {
                Trace.TraceError("Failed to initialize due to error:");
                Trace.TraceError(ex.ToString());
            }
        }

        public void Join() {
            listenThread.Join();
        }

        public void Stop() {
            // Notify threads to exit
            consoleInput.WriteLine(null);
            inputForwarder.WriteLine(null);
            commandQueue.Add(null);
        }

        public void SubmitCommand(string command) {
            commandQueue.Add(sanitize(command));
        }

        private static string sanitize(string command) {
            command = command.Trim();
            return command.Replace("\r", "");
        }

        private void SubmitCommands() {
            while (true) {
                string command = commandQueue.Take();

                // Thread exit signal
                if (command == null) {
                    break;
                }

                Trace.TraceInformation("Sending command: {0}", command);
                Console.Write(command + "\n");
            }
        }

        public IList<Favorite> GetFavorites() => favoritesList.Favorites;

        private void ListenForForward() {
            try {
                while (true) {
                    string input = inputForwarder.ReadLine();

                    // input will be null when Skyrim terminated (stdin closed)
                    if (input == null) {
                        break;
                    }

                    Trace.TraceInformation("Received command to forward: {0}", input);

                    SubmitCommand(input);
                }
            } catch (Exception ex) {
                Trace.TraceError(ex.ToString());
            }
        }

        private void ListenForInput() {
            try {
                // try to restore saved state after reloading the configuration file.
                consoleInput.RestoreSavedState();

                while (true) {
                    string input = consoleInput.ReadLine();

                    // input will be null when Skyrim terminated (stdin closed)
                    if (input == null) {
                        break;
                    }

                    Trace.TraceInformation("Received command: {0}", input);

                    inputForwarder.WriteLine(input);

                    string[] tokens = input.Split('|');
                    string command = tokens[0];
                    if (command.Equals("FAVORITES")) {
                        consoleInput.currentFavoritesList = input;
                        favoritesList.Update(string.Join("|", tokens, 1, tokens.Length - 1));
                    }
                }
            } catch (Exception ex) {
                Trace.TraceError(ex.ToString());
            }
        }
    }
}
