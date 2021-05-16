using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;

namespace EmpathicQbt.ConsoleServer {
    public class Favorite
    {
        public string ItemName { get; set; }
        public long FormId { get; set; }
        public long ItemId { get; set; }
        public bool IsSingleHanded { get; set; }
        public int TypeId { get; set; }
    }

    public class FavoritesList {

        private Configuration config;

        public IList<Favorite> Favorites { get; protected set; } = new List<Favorite>()
        {
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack",
            },
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack2",
            },
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack3",
            },
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack4",
            },
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack5",
            },
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack6",
            },
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack7",
            },
            new Favorite
            {
                ItemId = 666,
                TypeId = 999,
                FormId = 222,
                IsSingleHanded = true,
                ItemName = "Wabbajack8",
            },
        };

        public FavoritesList(Configuration config) {
            this.config = config;
        }

        // Locates and loads item name replacement maps
        // Returns dynamic map/dictionary or null when the replacement map files cannot be located
        public dynamic LoadItemNameMap()
        {
            string filepath = Configuration.ResolveFilePath("item-name-map.json");
            if(File.Exists(filepath))
            {
                return LoadItemNameMap(filepath);
            }
            return null;
        }
        
        // Returns a map/dictionary or throws exception when the file cannot be opened/read
        public dynamic LoadItemNameMap(string path)
        {
            var json = System.IO.File.ReadAllText(path);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Deserialize<dynamic>(json);
        }

        public string MaybeReplaceItemName(dynamic nameMap, string itemName)
        {
            if (nameMap == null)
                return itemName;

            try
            {
                return nameMap[itemName];
            }
            catch (KeyNotFoundException)
            {
                return itemName;
            }
        }

        public void Update(string input) {
            dynamic itemNameMap = LoadItemNameMap();

            Favorites.Clear();
            string[] itemTokens = input.Split('|');
            foreach(string itemStr in itemTokens) {
                try
                {
                    string[] tokens = itemStr.Split(',');
                    string itemName = tokens[0];
                    long formId = long.Parse(tokens[1]);
                    long itemId = long.Parse(tokens[2]);
                    bool isSingleHanded = int.Parse(tokens[3]) > 0;
                    int typeId = int.Parse(tokens[4]);

                    Favorites.Add(new Favorite
                    {
                        FormId = formId,
                        ItemName = itemName,
                        ItemId = itemId,
                        IsSingleHanded = isSingleHanded,
                        TypeId = typeId,
                    });

                    itemName = MaybeReplaceItemName(itemNameMap, itemName);

                    // FIXME Store item info
                } catch(Exception ex) {
                    Trace.TraceError("Failed to parse {0} due to exception:\n{1}", itemStr, ex.ToString());
                }
            }

            PrintToTrace();
        }

        public void PrintToTrace() {
            Trace.TraceInformation("Favorites List:");
            foreach (var favorite in Favorites) {
                Trace.TraceInformation("{0}: {1}: {2}", favorite.ItemName, favorite.TypeId, favorite.IsSingleHanded);
            }
        }
    }
}
