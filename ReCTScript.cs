using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ReCT;
using System.IO;
using System.Text.RegularExpressions;
namespace HLL_Viewer.Functionality
{
    public static class ReCTScript
    {
        public static Assembly CompileRScript(string script)
        {
            if (script != "return;")
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + "/temp/program.rct", script);
                ReCT.Program.Main(new string[] { Directory.GetCurrentDirectory() + "/temp/program.rct", "-s", "-f", "-o", Directory.GetCurrentDirectory() + "/temp/" + Core.currentSite + ".dll" });
                Assembly asm = Assembly.LoadFrom(Directory.GetCurrentDirectory() + "/temp/" + Core.currentSite + ".dll");
                System.IO.DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + "/temp/");
                File.Delete(Directory.GetCurrentDirectory() + "/temp/program.rct");

                return asm;
            }
            return null;
        }

        public static string GetRScriptFromPage(string page)
        {
            if (page.Contains("[rs]"))
            {
                if (page.Contains("[/rs]"))
                {
                    int subStringLen = page.IndexOf("[rs]") + page.IndexOf("[/rs]") - 4;
                    var mc = page.Substring(page.IndexOf("[rs]") + 4, subStringLen);
                    mc = mc.Replace("function", "set function");
                    return mc;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Parser Error: No closing ReCTScript tag!");
                Console.ResetColor();
                return "return;";
            }
            return "return;";
        }

        public static string RemoveRscriptFromPage(string page)
        {
            if (page.Contains("[rs]"))
            {
                if (page.Contains("[/rs]"))
                {
                    int subStringLen = page.IndexOf("[rs]") + page.IndexOf("[/rs]") + 5;
                    var mc = page.Substring(page.IndexOf("[rs]"), subStringLen);
                    return page.Replace(mc, "");
                }
                return page;
            }
            return page;
        }
    }
}
