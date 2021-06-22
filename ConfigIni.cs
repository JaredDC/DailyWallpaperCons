using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace DailyWallpaper
{    public class ConfigIni
    {
        private string iniPath;
        public string exeName = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public ConfigIni(string IniPath = null)
        {
            iniPath = new FileInfo(IniPath ?? "config.ini").FullName;
            if (!File.Exists(iniPath))
            {
                CreateDefIni();
            }
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? exeName, Key, "", RetVal, 255, iniPath);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? exeName, Key, Value, iniPath);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? exeName);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? exeName);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        public void CreateDefIni()
        {
            Write("useLocal", "yes", exeName);
            Write("useOnline", "no", exeName);
            Write("createUsageStat", "once", exeName);
            Write("want2AutoRun", "once", exeName);

            Write("saveDir", "NULL", "Online");
            Write("ngChina", "no", "Online");
            Write("bingChina", "yes", "Online");
            Write("alwaysdlBingWallpaper", "yes", "Online");
            Write("dailySpotlight", "yes", "Online");
            Write("dailySpotlightDir", "AUTO", "Online");

            Write("imgDir", @"C:\Users\jared\Pictures\pic", "Local");
            Write("scan", "yes", "Local");

            Write("copyFolder", "None", "Local");
            Write("want2Copy", "no", "Local");

            Write("mTime", "NULL", "Local");
            Write("lastImgDir", "NULL", "Local");
            // Write("lastImgDirmTime", "NULL", "Local");
            Write("wallpaper", "NULL", "LOG");
        }
        
        private void PrintDict(Dictionary<string, string> dict)
        {
            foreach (string key in dict.Keys)
            {
                Console.WriteLine($"{key}: {dict[key]}");
            }
        }
        
        public void UpdateIniItem(string key=null,string value=null, string section="Local")
        {
            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
            {
                Write(key, value, section);
                Console.WriteLine($"update \"{key}\" -> \"{value}\"");
            }                
        }
        
        public Dictionary<string, string> GetCfgFromIni()
        {
            Dictionary<string, string> iniDict = new Dictionary<string, string>();
            
            // master
            iniDict.Add("useLocal", Read("useLocal", exeName));
            iniDict.Add("useOnline", Read("useOnline", exeName));
            iniDict.Add("createUsageStat", Read("createUsageStat", exeName));
            iniDict.Add("want2AutoRun", Read("want2AutoRun", exeName));

            // online
            iniDict.Add("ngChina", Read("ngChina", "Online"));
            iniDict.Add("bingChina", Read("bingChina", "Online"));
            iniDict.Add("dailySpotlight", Read("dailySpotlight", "Online"));
            iniDict.Add("dailySpotlightDir", Read("dailySpotlightDir", "Online"));
            iniDict.Add("alwaysdlBingWallpaper", Read("alwaysdlBingWallpaper", "Online"));
            
            iniDict.Add("imgDir", Read("imgDir", "Local"));
            iniDict.Add("scan", Read("scan", "Local"));
            iniDict.Add("copyFolder", Read("copyFolder", "Local"));
            iniDict.Add("want2Copy", Read("want2Copy", "Local"));
            iniDict.Add("mTime", Read("mTime", "Local"));
            iniDict.Add("lastImgDir", Read("lastImgDir", "Local"));
            // iniDict.Add("lastImgDirmTime", Read("lastImgDirmTime", "Local"));

            // print
            // PrintDict(iniDict);
            return iniDict;
        }

    }
}
