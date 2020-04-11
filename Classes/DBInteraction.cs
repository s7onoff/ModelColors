using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace RCModelColors.Classes
{
    static class DBInteraction
    {
        public static void AddPropItem(string property)
        {
            PropItem propItem = new PropItem()
            {
                Name = property,
                Hue = 0,
                Saturation = 70,
                Lightness = 50
            };

            using(SQLite.SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<PropItem>();
                if(connection.Find<PropItem>(propItem.Name) == null)
                    connection.Insert(propItem);
            }
        }

        public static void Clear()
        {
            using (SQLite.SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<PropItem>();
                int _ = connection.DeleteAll<PropItem>();
            }
        }

        public static void ReadDatabase(List<PropItem> list, ref ListView listView)
        {
            //AddPropItem(); //- only for testing
            using (SQLite.SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<PropItem>();
                list = (connection.Table<PropItem>().ToList().OrderBy(c => c.Name).ToList());
            }
            if (list != null)
            {
                listView.ItemsSource = list;
            }
        }

        public static List<PropItem> GetItemsList()
        {
            using (SQLite.SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<PropItem>();
                List<PropItem> list = (connection.Table<PropItem>().ToList().OrderBy(c => c.Name).ToList());
                return list;
            }
        }

        public static void Store(PropItem item)
        {
            using (SQLite.SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<PropItem>();
                connection.Update(item);
            }
        }
    }
}
