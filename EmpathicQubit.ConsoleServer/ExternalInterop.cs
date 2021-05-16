using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmpathicQbt.ConsoleServer {
    public class ExternalInterop {

        private Configuration config = null;
        private SkyrimInterop skyrimInterop = null;

        private readonly HashSet<string> BATCH_FILENAMES = new HashSet<string>() { "wotv", "ivrqs" };
        private readonly long FILE_CHANGE_DEBOUNCE_TIME_TICKS = 10000 * 200; // 200 ms

        private string configFileName = null;
        private FileSystemWatcher configFileWatcher;
        private bool isConfigFileChanged = false;


        public ExternalInterop(Configuration config, SkyrimInterop skyrimInterop) {
            this.config = config;
            this.skyrimInterop = skyrimInterop;
        }

        public void Start() {
            ListenForConfigFile();
        }

        public void Stop() {
            if(configFileWatcher != null)
            {
                configFileWatcher.EnableRaisingEvents = false;
            }
        }
        private void ListenForConfigFile()
        {
            try
            {
                string filePath = config.GetIniFilePath();
                if (filePath == null) {
                    throw new Exception("Configuration file does not exist.");
                }

                configFileName = Path.GetFileName(filePath).ToLower();

                configFileWatcher = new FileSystemWatcher();
                configFileWatcher.Path = Path.GetDirectoryName(filePath);
                configFileWatcher.Changed += Watcher_ConfigFileChanged;
                configFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
                Trace.TraceInformation("Watching for config file at {0}", configFileWatcher.Path);
                configFileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to watch for config files: {0}", ex.ToString());
            }
        }

        private void Watcher_ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            if (!isConfigFileChanged &&
                (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created))
            {
                string filename = e.Name.ToLower();
                if (filename.Equals(configFileName))
                {
                    Trace.TraceInformation("Config file {0} changed", filename);

                    // Wait for the configuration file to be saved
                    Thread.Sleep(1000);

                    isConfigFileChanged = true;
                    Stop();
                    skyrimInterop.Stop();
                }
            }
        }

        public bool IsConfigFileChanged() {
            return isConfigFileChanged;
        }
    }
}
