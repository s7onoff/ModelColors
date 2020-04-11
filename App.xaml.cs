using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RCModelColors
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string databaseName = "ModelColors.db";
        public static string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string databasePath = System.IO.Path.Combine(myDocumentsFolder, databaseName);

        public static string teklaModelPath = myDocumentsFolder;
        public static string teklaModelName = "TeklaModelName";


        public static string[] arrayOfPlatePrefixes = new string[] {};
        //"BL", "BPL", "FB", "FL", "FLT", "FPL", "PL", "PLATE","—", "ПВ", "Полоса", "Риф", "ЧРиф"
        public static string[] ignoredPrefixes = new string[] { };

        public bool ignorePlates = false;
    }
}
