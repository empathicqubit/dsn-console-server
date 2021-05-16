using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace EmpathicQbt.ConsoleServer
{
    public class InputForwarder
    {
        private BlockingCollection<string> outputQueue = new BlockingCollection<string>();
        private Thread outputThread = null;
        private StreamWriter inputWriter = null;
        private Process subProcess = null;
        bool isOutputTerminated = false;

        public void Start()
        {
            outputThread = new Thread(ReadLineFromProcess);
            var filename = Configuration.ResolveFilePath("DragonbornSpeaksNaturally.Original.exe");

            Trace.TraceInformation("Starting {0}", filename);
            subProcess = Process.Start(new ProcessStartInfo()
            {
                FileName = filename,
                Arguments = "\"" + String.Join("\" \"", System.Environment.GetCommandLineArgs().Skip(1)) + "\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Console.OutputEncoding,
                StandardErrorEncoding = Console.OutputEncoding,
            });
            subProcess.Start();
            inputWriter = subProcess.StandardInput;
            inputWriter.AutoFlush = true;
            outputThread.Start();
        }

        private void ReadLineFromProcess()
        {
            while (true)
            {
                string input = subProcess.StandardOutput.ReadLine();

                // input will be null when Skyrim terminated (stdin closed)
                if (input == null)
                {
                    isOutputTerminated = true;
                    Trace.TraceInformation("Skyrim is terminated, console server will quit.");

                    // Notify the SkyrimInterop thread to exit
                    outputQueue.Add(null);

                    break;
                }

                outputQueue.Add(input);
            }
        }

        public bool IsInputTerminated() {
            return isOutputTerminated;
        }

        public void WriteLine(string line) {
            inputWriter.WriteLine(line);
            inputWriter.Flush();
            inputWriter.BaseStream.Flush();
        }

        public void Stop()
        {
            if(subProcess != null)
            {
                subProcess.Kill();
            }
        }

        public string ReadLine() {
            return outputQueue.Take();
        }
    }
}