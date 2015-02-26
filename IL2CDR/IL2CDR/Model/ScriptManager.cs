using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;

namespace IL2CDR.Model
{
    public class ScriptManager
    {
        private const string scriptsSubFolder = @"\Scripts";
        private static Random random = new Random();

        public void LoadScripts()
        {
            string folder = AppDomain.CurrentDomain.GetData("DataDirectory") + scriptsSubFolder;

            var scriptFiles = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);
            foreach (var scriptFileName in scriptFiles)
            {
                Util.Try(() => {
                    Log.WriteInfo("Loading script {0}...", scriptFileName);
                    var scriptObject = CSScript.Evaluator.LoadFile(scriptFileName);

                    //Is known script ?
                    if (scriptObject is IActionScript)
                    {
                        var script = scriptObject as IActionScript;
                    }
                
                });
            }
        }
    }
}
