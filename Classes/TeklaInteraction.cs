using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tekla.Structures;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using Tekla.Structures.Filtering;
using Tekla.Structures.Filtering.Categories;
using System.Diagnostics;

namespace RCModelColors.Classes
{
    static class TeklaInteraction
    {
        static Model model = new Model();
        public static bool Connect()
        {
            if (model.GetConnectionStatus())
            {
                App.teklaModelName = model.GetInfo().ModelName;
                App.teklaModelPath = model.GetInfo().ModelPath;
                App.databasePath = System.IO.Path.Combine(App.teklaModelPath, App.databaseName);
                return true;
            }
            else return false;
        }

        public static void GetAllProfiles()
        {
            DBInteraction.Clear();

            System.Type[] Types = new System.Type[2];
            Types.SetValue(typeof(Part), 0);
            Types.SetValue(typeof(Tekla.Structures.Model.Boolean), 1);

            ModelObjectEnumerator Enum = model.GetModelObjectSelector().GetAllObjectsWithType(Types);

            FullFillDB(Enum);
        }

        public static void GetSelectedProfiles()
        {
            DBInteraction.Clear();

            var selector = new TSMUI.ModelObjectSelector();

            ModelObjectEnumerator Enum = selector.GetSelectedObjects();

            FullFillDB(Enum);
        }

        private static void FullFillDB(ModelObjectEnumerator Enum)
        {
            var sWatch = new Stopwatch();
            List<string> PartStringList = new List<string>();
            
            sWatch.Start();

            HashSet<Part> profilesSet = new HashSet<Part>();
            while (Enum.MoveNext())
            {
                try 
                {
                    Part currentPart = Enum.Current as Part;
                    if (currentPart != null)
                    {
                        profilesSet.Add(currentPart);
                    }
                }
                catch { Console.WriteLine("Something wrong"); }
            }
            
            Console.WriteLine("Profile HashSet fullfilled, time " + sWatch.ElapsedMilliseconds.ToString());

            foreach(Part part in profilesSet)
            {
                if (App.arrayOfPlatePrefixes.Any(s => part.Profile.ProfileString.StartsWith(s, StringComparison.CurrentCultureIgnoreCase)) ||
                            Regex.IsMatch(part.Profile.ProfileString, @"^\d+"))
                {
                    //So, this is plate
                    //double thickness = 0;
                    //part.GetReportProperty("WIDTH", ref thickness);
                    
                    double thickness = PlateThickness(part.Profile.ProfileString);
                    DBInteraction.AddPropItem("-" + thickness.ToString());
                }
                else
                {
                    DBInteraction.AddPropItem(part.Profile.ProfileString);
                }
            }
            sWatch.Stop();
            Console.WriteLine("Database fullfilled, time " + sWatch.ElapsedMilliseconds.ToString());
        }

        public static void CreateFilter(bool applyChecked, string repFileName)
        {
            var attributesPath = Path.Combine(App.teklaModelPath, "attributes");

            List<PropItem> list = DBInteraction.GetItemsList();

            foreach (PropItem item in list)
            {
                //filename
                var filterName = Path.Combine(attributesPath, "@" + item.Name).Replace("*", "_");

                BinaryFilterExpressionCollection expressionCollection;

                if (item.Name.StartsWith("-"))
                { expressionCollection = CreateFilterForPlate(item.Name.Replace("-","")); }
                else
                { expressionCollection = CreateFilterForBeam(item.Name); }

                var filter = new Filter(expressionCollection);

                filter.CreateFile(FilterExpressionFileType.OBJECT_GROUP_SELECTION, filterName);

                try
                { File.Delete(filterName + ".PObjGrp"); }
                catch
                {
                    //do nothing
                }
                finally
                {
                    // Два костыля разом, спасибо тебе, Trimble!
                    File.Move(filterName + ".SObjGrp", filterName + ".PObjGrp");
                    File.WriteAllText(filterName + ".PObjGrp", 
                        File.ReadAllText(filterName + ".PObjGrp", Encoding.UTF8), 
                        Encoding.GetEncoding(1251));
                }
            }

            CreateRepresentationFileByText(repFileName);

            if(applyChecked)
            {
                TSMUI.ModelViewEnumerator ViewEnum = TSMUI.ViewHandler.GetAllViews();
                while (ViewEnum.MoveNext())
                {
                    TSMUI.View view = ViewEnum.Current;
                    view.CurrentRepresentation = repFileName;
                    view.Modify();
                }
            }
        }

        public static BinaryFilterExpressionCollection CreateFilterForBeam(string profileName)
        {

            //One filter expression (one filter string)
            var profile = new PartFilterExpressions.Profile();
            var currentProfileName = new StringConstantFilterExpression(profileName);
            var expression = new BinaryFilterExpression(profile, StringOperatorType.IS_EQUAL, currentProfileName);

            //Filter
            var expressionCollection = new BinaryFilterExpressionCollection();
            expressionCollection.Add(new BinaryFilterExpressionItem(expression, BinaryFilterOperatorType.BOOLEAN_AND));

            return expressionCollection;
        }

        public static BinaryFilterExpressionCollection CreateFilterForPlate(string thickness)
        {

            var expressionCollection = new BinaryFilterExpressionCollection();

            foreach (string prefix in App.arrayOfPlatePrefixes.Append(""))
            {
                var profile = new PartFilterExpressions.Profile();

                //One filter expression (one filter string)
                var currentProfileName = new StringConstantFilterExpression(
                    prefix + thickness + " " + 
                    prefix + thickness + "[*]*" + " " + 
                    prefix + "*[*]" + thickness);
                var expression = new BinaryFilterExpression(profile, StringOperatorType.IS_EQUAL, currentProfileName);
                expressionCollection.Add(new BinaryFilterExpressionItem(expression, BinaryFilterOperatorType.BOOLEAN_OR));
            }

            return expressionCollection;
        }
        
        public static void CreateRepresentationFileByText(string repFileName)
        {
            var attributesPath = Path.Combine(App.teklaModelPath, "attributes");
            var repFilePath = Path.Combine(attributesPath, repFileName + ".rep");

            List<PropItem> list = DBInteraction.GetItemsList();

            File.WriteAllText(repFilePath, "");
            //using (StreamReader profileReader = new StreamReader(profileFilePath))
            //using (StreamReader headReader = new StreamReader(headFilePath))
            using (StreamWriter writer = new StreamWriter(repFilePath, true))
            {
                writer.WriteLine("REPRESENTATIONS ");
                writer.WriteLine("{");
                writer.WriteLine("    Version= 1.04 ");
                writer.WriteLine("    Count= " + (list.Count()+1).ToString()+ " ");


                foreach(PropItem item in list)
                {
                    writer.WriteLine("    SECTION_UTILITY_LIMITS ");
                    writer.WriteLine("    {");
                    writer.WriteLine("        0 ");
                    writer.WriteLine("        0 ");
                    writer.WriteLine("        0 ");
                    writer.WriteLine("        0 ");
                    writer.WriteLine("        }");
                    writer.WriteLine("    SECTION_OBJECT_REP ");
                    writer.WriteLine("    {");
                    writer.WriteLine("        @" + item.Name.ToString().Replace("*", "_") + " ");
                    writer.WriteLine("        " + item.GetTeklaNumber() + " ");
                    writer.WriteLine("        10 ");
                    writer.WriteLine("        }");
                    writer.WriteLine("    SECTION_OBJECT_REP_BY_ATTRIBUTE ");
                    writer.WriteLine("    {");
                    writer.WriteLine("        (SETTINGNOTDEFINED) ");
                    writer.WriteLine("        }");
                    writer.WriteLine("    SECTION_OBJECT_REP_RGB_VALUE ");
                    writer.WriteLine("    {");
                    writer.WriteLine("        " + ((int)item.Red).ToString() + " ");
                    writer.WriteLine("        " + ((int)item.Green).ToString() + " ");
                    writer.WriteLine("        " + ((int)item.Blue).ToString() + " ");
                    writer.WriteLine("        }");
                }
                writer.WriteLine("    SECTION_UTILITY_LIMITS ");
                writer.WriteLine("    {");
                writer.WriteLine("        0 ");
                writer.WriteLine("        0 ");
                writer.WriteLine("        0 ");
                writer.WriteLine("        0 ");
                writer.WriteLine("        }");
                writer.WriteLine("    SECTION_OBJECT_REP ");
                writer.WriteLine("    {");
                writer.WriteLine("        All ");
                writer.WriteLine("        -2 ");
                writer.WriteLine("        10 ");
                writer.WriteLine("        }");
                writer.WriteLine("    SECTION_OBJECT_REP_BY_ATTRIBUTE ");
                writer.WriteLine("    {");
                writer.WriteLine("        (SETTINGNOTDEFINED) ");
                writer.WriteLine("        }");
                writer.WriteLine("    SECTION_OBJECT_REP_RGB_VALUE ");
                writer.WriteLine("    {");
                writer.WriteLine("        0 ");
                writer.WriteLine("        0 ");
                writer.WriteLine("        0 ");
                writer.WriteLine("        }");
                writer.WriteLine("    }");
            }
        }

        public static double PlateThickness(string profile)
        {
            double thickness = 0;

            string numbersAndAsterisk = Regex.Replace(profile, "[^0-9.\\*]", "");

            foreach (string str in numbersAndAsterisk.Split('*'))
            {
                double.TryParse(str, out double thisDimension);
                if (thickness != 0 && thickness < thisDimension)
                {
                    //do nothing
                }
                else
                {
                    thickness = thisDimension;
                }
            }
            return thickness;
        }

        public static string WindowsString(string utfString)
        {
            Encoding srcEncodingFormat = Encoding.UTF8;
            Encoding dstEncodingFormat = Encoding.GetEncoding(1251);

            byte[] originalByteString = srcEncodingFormat.GetBytes(utfString);
            byte[] convertedByteString = Encoding.Convert(srcEncodingFormat, dstEncodingFormat, originalByteString);

            string finalString = dstEncodingFormat.GetString(convertedByteString);
            return finalString;
        }
    }
}
