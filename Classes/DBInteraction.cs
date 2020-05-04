using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace RCModelColors.Classes
{
    public class DBInteraction
    {
        public static string DatabasePath { get; set; }

        public void AddPropItem(string property)
        {
            PropItem propItem = new PropItem()
            {
                Name = property,
                Hue = 0,
                Saturation = 70,
                Lightness = 50
            };

            using (SQLiteConnection connection = new SQLiteConnection(DatabasePath))
            {
                connection.CreateTable<PropItem>();
                if (connection.Find<PropItem>(propItem.Name) == null)
                    connection.Insert(propItem);
            }
        }

        public void Clear()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabasePath))
            {
                connection.CreateTable<PropItem>();
                int _ = connection.DeleteAll<PropItem>();
            }
        }

        public List<PropItem> ReadDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabasePath))
            {
                connection.CreateTable<PropItem>();
                List<PropItem> list = (connection.Table<PropItem>().ToList().OrderBy(c => c.Name).ToList());
                return list;
            }
        }

        public void Store(PropItem item)
        {
            using (SQLite.SQLiteConnection connection = new SQLiteConnection(DatabasePath))
            {
                connection.CreateTable<PropItem>();
                connection.Update(item);
            }
        }

        public DBInteraction()
        {
            if(DatabasePath == null)
            { 
                string databaseName = "ModelColors.db";
                string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DatabasePath = System.IO.Path.Combine(myDocumentsFolder, databaseName);
            }
        }
    }
}
