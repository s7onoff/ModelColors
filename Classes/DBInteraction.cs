using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace ModelColors.Classes
{
    public class DBInteraction
    {
        public static string DatabasePath { get; set; }

        public void WriteData(List<string> formattedProfilesStrings)
        {
            var profilesItemList = new List<PropItem>();

            foreach (var profileName in formattedProfilesStrings)
            {
                profilesItemList.Add(
                    new PropItem()
                    {
                        Name = profileName,
                        Hue = 0,
                        Saturation = 70,
                        Lightness = 50
                    }
                );
            }

            WriteDatabase( profilesItemList );
        }

        public void Clear()
        {
            File.WriteAllText(DatabasePath, string.Empty);
        }
        private void WriteDatabase(List<PropItem> items)
        {
            var serializer = new SerializerBuilder().Build();

            File.WriteAllText(DatabasePath, serializer.Serialize(items));
        }

        public List<PropItem> ReadDatabase()
        {
            var text = File.ReadAllText(DatabasePath);

            var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

            var result = deserializer.Deserialize<List<PropItem>>(text);

            return result ?? new List<PropItem>();
        }

        public void Store(PropItem item)
        {
            var items = ReadDatabase();
            items.Remove(items.FirstOrDefault(x => x.Name == item.Name));
            items.Add(item);

            WriteDatabase(items);
        }

        public void Delete(PropItem item)
        {
            var items = ReadDatabase();
            items.Remove(items.FirstOrDefault(x => x.Name == item.Name));

            WriteDatabase(items);
        }

        public DBInteraction()
        {
            if (DatabasePath == null)
            {
                string databaseName = "ModelColors_db";
                string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DatabasePath = System.IO.Path.Combine(myDocumentsFolder, databaseName);
                File.WriteAllText(DatabasePath, string.Empty);
            }
        }
    }

//    public class DBInteraction
//    {
//        public static string DatabasePath { get; set; }
//
//        public void AddPropItem(string property)
//        {
//            PropItem propItem = new PropItem()
//            {
//                Name = property,
//                Hue = 0,
//                Saturation = 70,
//                Lightness = 50
//            };
//
//            using (SQLiteConnection connection = new SQLiteConnection(DatabasePath))
//            {
//                connection.CreateTable<PropItem>();
//                if (connection.Find<PropItem>(propItem.Name) == null)
//                    connection.Insert(propItem);
//            }
//        }
//
//        public void Clear()
//        {
//            using (SQLiteConnection connection = new SQLiteConnection(DatabasePath))
//            {
//                connection.CreateTable<PropItem>();
//                int _ = connection.DeleteAll<PropItem>();
//            }
//        }
//
//        public List<PropItem> ReadDatabase()
//        {
//            using (SQLiteConnection connection = new SQLiteConnection(DatabasePath))
//            {
//                connection.CreateTable<PropItem>();
//                List<PropItem> list = (connection.Table<PropItem>().ToList().OrderBy(c => c.Name).ToList());
//                return list;
//            }
//        }
//
//        public void Store(PropItem item)
//        {
//            using (SQLite.SQLiteConnection connection = new SQLiteConnection(DatabasePath))
//            {
//                connection.CreateTable<PropItem>();
//                connection.Update(item);
//            }
//        }
//
//        public void Delete(PropItem item)
//        {
//            using (SQLite.SQLiteConnection connection = new SQLiteConnection(DatabasePath))
//            {
//                connection.CreateTable<PropItem>();
//                connection.Delete(item);
//            }
//        }
//        public DBInteraction()
//        {
//            if(DatabasePath == null)
//            {
//                string databaseName = "ModelColors.db";
//                string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
//                DatabasePath = System.IO.Path.Combine(myDocumentsFolder, databaseName);
//            }
//        }
//    }
}
