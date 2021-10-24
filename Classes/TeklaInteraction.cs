using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tekla.Structures.Filtering;
using Tekla.Structures.Filtering.Categories;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;

namespace RCModelColors.Classes
{
    public class TeklaInteraction
    {
        public Model Model { get; set; }
        public DBInteraction DBInteraction { get; set; }
        public string[] PlatePrefixesArray { get; set; }
        public string[] IgnoredPrefixesArray { get; set; }
        public string ModelName { get; set; }
        public string ModelPath { get; set; }
        public bool PlatesIgnored { get; set; }
        public bool BeamsIgnored { get; set; }
        public string ReportsDirectory { get; set; }

        public TeklaInteraction()
        {
            Model = new Model();
            Connect();
            DBInteraction = new DBInteraction();
        }
        public bool Connect()
        {
            if (Model.GetConnectionStatus())
            {
                ModelName = Model.GetInfo().ModelName;
                ModelPath = Model.GetInfo().ModelPath;
                DBInteraction.DatabasePath = Path.Combine(ModelPath, "ModelColors.db");
                return true;
            }
            else return false;
        }

        public void GetProfiles(bool Selected)
        {
            DBInteraction.Clear();

            string reportFile = Path.Combine(ModelPath, "Report.xsr");

            string reportTemplateFile = Path.Combine(ModelPath, "RCModelColors_Profiles.rpt");

            File.Copy("./Files/RCModelColors_Profiles.rpt", reportTemplateFile, true);

            if (Selected)
            {
                Tekla.Structures.Model.Operations.Operation.CreateReportFromSelected("RCModelColors_Profiles", reportFile, "", "", "");
            }
            else
            {
                Tekla.Structures.Model.Operations.Operation.CreateReportFromAll("RCModelColors_Profiles", reportFile, "", "", "");
            }

            FullFillDB(ProfilesListFromReport(reportFile));

            File.Delete(reportFile);
            File.Delete(reportTemplateFile);
        }

        private List<string> ProfilesListFromReport(string reportPath)
        {
            List<string> profileList = new List<string>();

            using (StreamReader reader = new StreamReader(reportPath))
            {
                string reportFileLine;
                while ((reportFileLine = reader.ReadLine()) != null)
                {
                    if(reportFileLine != "")
                    { 
                        profileList.Add(reportFileLine.Trim());
                    }
                }
            }

            return profileList;
        }

        private void FullFillDB(List<string> profileList)
        {
            foreach (string profileName in profileList)
            {
                // check if it is plate
                if (PlatePrefixesArray.Any(s => profileName.StartsWith(s, StringComparison.CurrentCultureIgnoreCase)) || 
                    Regex.IsMatch(profileName, @"^\d+"))
                {
                    if (PlatesIgnored)
                    {
                        continue;
                    }
                    else
                    {
                        double thickness = PlateThickness(profileName);
                        DBInteraction.AddPropItem("-" + thickness.ToString());
                    }
                }
                else
                {
                    // check if profile should be ignored
                    if (IgnoredPrefixesArray.Any(s => profileName.StartsWith(s, StringComparison.CurrentCultureIgnoreCase)) || BeamsIgnored)
                    {
                        continue;
                    }
                    else
                    {
                        DBInteraction.AddPropItem(profileName);
                    }
                }
            }
        }

        public void CreateFilter(bool applyChecked, string repFileName)
        {
            var attributesPath = Path.Combine(ModelPath, "attributes");

            List<PropItem> list = DBInteraction.ReadDatabase();

            foreach (PropItem item in list)
            {
                //filename
                var filterName = Path.Combine(attributesPath, "@" + item.Name).Replace("*", "_");

                BinaryFilterExpressionCollection expressionCollection;

                if (item.Name.StartsWith("-"))
                { expressionCollection = CreateFilterForPlate(item.Name.Replace("-", "")); }
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

            if (applyChecked)
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

        public BinaryFilterExpressionCollection CreateFilterForBeam(string profileName)
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

        public BinaryFilterExpressionCollection CreateFilterForPlate(string thickness)
        {

            var expressionCollection = new BinaryFilterExpressionCollection();

            foreach (string prefix in PlatePrefixesArray.Append(""))
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

        public void CreateRepresentationFileByText(string repFileName)
        {
            var attributesPath = Path.Combine(ModelPath, "attributes");
            //TODO: Настройку, чтоб выбирать путь для этих файлов
            var repFilePath = Path.Combine(attributesPath, repFileName + ".rep");

            List<PropItem> list = DBInteraction.ReadDatabase();

            File.WriteAllText(repFilePath, "");
            //using (StreamReader profileReader = new StreamReader(profileFilePath))
            //using (StreamReader headReader = new StreamReader(headFilePath))
            using (StreamWriter writer = new StreamWriter(repFilePath, true))
            {
                writer.WriteLine("REPRESENTATIONS ");
                writer.WriteLine("{");
                writer.WriteLine("    Version= 1.04 ");
                writer.WriteLine("    Count= " + (list.Count() + 1).ToString() + " ");


                foreach (PropItem item in list)
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

        public double PlateThickness(string profile)
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

        public string WindowsString(string utfString)
        {
            Encoding srcEncodingFormat = Encoding.UTF8;
            Encoding dstEncodingFormat = Encoding.GetEncoding(1251);

            byte[] originalByteString = srcEncodingFormat.GetBytes(utfString);
            byte[] convertedByteString = Encoding.Convert(srcEncodingFormat, dstEncodingFormat, originalByteString);

            string finalString = dstEncodingFormat.GetString(convertedByteString);
            return finalString;
        }

        //public string GetReportsDirectory()
        //{
        //    string directory = "";
        //    Tekla.Structures.TeklaStructuresSettings.GetAdvancedOption("XS_REPORT_OUTPUT_DIRECTORY", ref directory);
        //    if(directory.StartsWith(".\\"))
        //    {
        //        if(directory.StartsWith("..\\"))
        //        {
        //            ReportsDirectory = Path.Combine(ModelPath, "\\", directory);
        //        }
        //        else
        //        {
        //            ReportsDirectory = Path.Combine(ModelPath, directory.Substring(2));
        //        }
        //    }
        //    else
        //    { 
        //        ReportsDirectory = Path.Combine(directory); 
        //    }
        //    ReportsDirectory = directory;
        //    return directory;
        //}

    }
}
