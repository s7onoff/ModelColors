using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Tekla.Structures;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            TSM.Model CurrentModel = new TSM.Model();
            string dir = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XSDATADIR", ref dir);

            string ApplicationName = "ModelColors.exe";
			string ApplicationPath = Path.Combine(dir, "Environments\\common\\extensions\\ModelColors\\" + ApplicationName);

            Process NewProcess = new Process();

            if (File.Exists(ApplicationPath))
            {
                NewProcess.StartInfo.FileName = ApplicationPath;

                try
                {
                    NewProcess.Start();
                    NewProcess.WaitForExit();
                }
                catch
                {
                    MessageBox.Show(ApplicationName + " failed to start.");
                }
            }
            else
            {
                MessageBox.Show(ApplicationPath + " not found.");
            }
        }
    }
}