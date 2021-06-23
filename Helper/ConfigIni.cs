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
        public string exeName;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public ConfigIni(string IniPath = null, string exeName=null)
        {
            this.exeName = exeName ?? Assembly.GetExecutingAssembly().GetName().Name;
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
            Write("useLocal", "no", exeName);
            Write("useOnline", "yes", exeName);
            Write("createUsageStat", "once", exeName);
            Write("want2AutoRun", "once", exeName);

            Write("saveDir", "NULL", "Online");
            Write("bingChina", "yes", "Online");
            Write("alwaysDLBingWallpaper", "yes", "Online");
            Write("dailySpotlight", "yes", "Online");
            Write("dailySpotlightDir", "AUTO", "Online");

            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            Write("imgDir", myPictures, "Local");
            Write("scan", "AUTO", "Local");

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
            iniDict.Add("bingChina", Read("bingChina", "Online"));
            iniDict.Add("dailySpotlight", Read("dailySpotlight", "Online"));
            iniDict.Add("dailySpotlightDir", Read("dailySpotlightDir", "Online"));
            iniDict.Add("alwaysDLBingWallpaper", Read("alwaysDLBingWallpaper", "Online"));
            
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
        public void RunAtStartup()
        {
            // ConfigIni iniFile
            string want2AutoRun = Read("want2AutoRun").ToLower();
            if (want2AutoRun.Equals("yes") || want2AutoRun.Equals("once"))
            {
                AutoStartupHelper.CreateAutorunShortcut();
                if (want2AutoRun.Equals("once"))
                {
                    Write("want2AutoRun", "yes/no");
                }
            }
            else if (want2AutoRun.Equals("no"))
            {
                if (!AutoStartupHelper.IsAutorun())
                {
                    return;
                }
                Console.WriteLine("You don't want this program run at Windows startup.");
                AutoStartupHelper.RemoveAutorunShortcut();
            }
        }
        public void CreateUsageText(string textFile, string usageText = null)
        {
            if (GetCfgFromIni()["createUsageStat"].ToLower().Equals("no"))
            {
                return;
            }
            if (GetCfgFromIni()["createUsageStat"].ToLower().Equals("once"))
            {
                UpdateIniItem("createUsageStat", "no", exeName);
            }
            if (File.Exists(textFile))
            {
                Console.WriteLine($"File already exists: {textFile}");
                return;
            }
            usageText = usageText ?? GetUsageText();
            File.WriteAllText(textFile, usageText);
            if (!File.Exists("LICENSE.txt")) {
                File.WriteAllText("LICENSE.txt", GetLicenceText());
            }
            Console.WriteLine($"Created usage file: {textFile}");
        }
        private string GetUsageText()
        {
            string usageText = "Usage for DailyWallpaper.exe\n" +
                "AUTHOR: HDC <jared.dcx@gmail.com>\n" +
                "-----------------------------------------\n" +
                "Notice: there is only ONE file you need to configure: config.ini, \n" +
                "        it should be with DailyWallpaper.exe\n" +
                "-----------------------------------------\n" +
                "here is a sample of config.ini:\n" +
                "\n" +
                "[DailyWallpaper]\n" +
                "useLocal=no\n" +
                "useOnline=yes\n" +
                "createUsageStat=no\n" +
                "want2AutoRun=yes/no\n" +
                "[Online]\n" +
                "saveDir=C:\\Users\\jared\\Pictures\\DailyWallpaper\n" +
                "bingChina=yes\n" +
                "alwaysDLBingWallpaper=yes\n" +
                "dailySpotlight=yes\n" +
                "dailySpotlightDir=AUTO\n" +
                "[Local]\n" +
                "imgDir=C:\\Users\\jared\\Pictures\n" +
                "scan=yes\n" +
                "copyFolder=None\n" +
                "want2Copy=no\n" +
                "mTime=NULL\n" +
                "lastImgDir=NULL\n" +
                "[LOG]\n" +
                "wallpaper=C:\\Users\\jared\\Pictures\\DailyWallpaper\\2021-0622_11-44-45.jpeg    2021-06-22 23:52:59\n" +
                "\n" +
                "---------------------\n" +
                "Section DailyWallpaper\n" +
                "1. useLocal                       Use local image, which means use \"Section WallpaperSetter\" feature.\n" +
                "2. useOnline                      Download the image and set it as wallpaper.\n" +
                "3. createUsageStat                Create and usage file flag: yes, once, no\n" +
                "                                    once:   when 'USAGE.TXT' doesn't exist, create once, you can delete, it won't create next time.\n" +
                "                                    yes:   when 'USAGE.TXT' doesn't exist, create\n" +
                "                                    no:     literally.\n" +
                "4. want2AutoRun                   copy .lnk to startup folder: autorun DailyWallpaper.exe when windows starup.\n" +
                "--------\n" +
                "Section Online\n" +
                "1. saveDir                        Where the image will be saved.\n" +
                "2. bingChina                      Download \"bingchina\" 's image and set it as wallpaper\n" +
                "3. alwaysDLBingWallpaper          Always download bingchina wallpaper\n" +
                "4. dailySpotlight                 Copy the image from daily.spotlight folder and set it as wallpaper \n" +
                "                                    [You have to open the feature in Windows10]\n" +
                "5. dailySpotlightDir              Set to AUTO, or set by yourself.\n" +
                "--------\n" +
                "Section Local\n" +
                "1. imgDir:                       The program will scan this folder and select a image as wallpaper\n" +
                "2. scan:                         Controlling the action of SCANNING, it has three options: yes, no, force\n" +
                "                                    yes:   when 'img_dir' has been modified by OS, scan and update '_img_list.txt'\n" +
                "                                    no:    never scan 'img_dir' unless '_img_list.txt' doesn't exist.\n" +
                "                                    force: Mandatory scan 'img_dir' and update '_img_list.txt'\n" +
                "3. copyFolder:                   Copy all suitable pictures to this folder from copy_folder, control by 'want2copy'\n" +
                "4. want2Copy:                    Controlling the action of COPYING, it has two options: yes, no\n" +
                "-----------------------------------------\n" +
                "FOR FREEDOM!";
            return usageText;
        }
        private string GetLicenceText()
        {
            string licenseText = "                                 Apache License\n" +
                    "                           Version 2.0, January 2004\n" +
                    "                        http://www.apache.org/licenses/\n" +
                    "\n" +
                    "   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION\n" +
                    "\n" +
                    "   1. Definitions.\n" +
                    "\n" +
                    "      \"License\" shall mean the terms and conditions for use, reproduction,\n" +
                    "      and distribution as defined by Sections 1 through 9 of this document.\n" +
                    "\n" +
                    "      \"Licensor\" shall mean the copyright owner or entity authorized by\n" +
                    "      the copyright owner that is granting the License.\n" +
                    "\n" +
                    "      \"Legal Entity\" shall mean the union of the acting entity and all\n" +
                    "      other entities that control, are controlled by, or are under common\n" +
                    "      control with that entity. For the purposes of this definition,\n" +
                    "      \"control\" means (i) the power, direct or indirect, to cause the\n" +
                    "      direction or management of such entity, whether by contract or\n" +
                    "      otherwise, or (ii) ownership of fifty percent (50%) or more of the\n" +
                    "      outstanding shares, or (iii) beneficial ownership of such entity.\n" +
                    "\n" +
                    "      \"You\" (or \"Your\") shall mean an individual or Legal Entity\n" +
                    "      exercising permissions granted by this License.\n" +
                    "\n" +
                    "      \"Source\" form shall mean the preferred form for making modifications,\n" +
                    "      including but not limited to software source code, documentation\n" +
                    "      source, and configuration files.\n" +
                    "\n" +
                    "      \"Object\" form shall mean any form resulting from mechanical\n" +
                    "      transformation or translation of a Source form, including but\n" +
                    "      not limited to compiled object code, generated documentation,\n" +
                    "      and conversions to other media types.\n" +
                    "\n" +
                    "      \"Work\" shall mean the work of authorship, whether in Source or\n" +
                    "      Object form, made available under the License, as indicated by a\n" +
                    "      copyright notice that is included in or attached to the work\n" +
                    "      (an example is provided in the Appendix below).\n" +
                    "\n" +
                    "      \"Derivative Works\" shall mean any work, whether in Source or Object\n" +
                    "      form, that is based on (or derived from) the Work and for which the\n" +
                    "      editorial revisions, annotations, elaborations, or other modifications\n" +
                    "      represent, as a whole, an original work of authorship. For the purposes\n" +
                    "      of this License, Derivative Works shall not include works that remain\n" +
                    "      separable from, or merely link (or bind by name) to the interfaces of,\n" +
                    "      the Work and Derivative Works thereof.\n" +
                    "\n" +
                    "      \"Contribution\" shall mean any work of authorship, including\n" +
                    "      the original version of the Work and any modifications or additions\n" +
                    "      to that Work or Derivative Works thereof, that is intentionally\n" +
                    "      submitted to Licensor for inclusion in the Work by the copyright owner\n" +
                    "      or by an individual or Legal Entity authorized to submit on behalf of\n" +
                    "      the copyright owner. For the purposes of this definition, \"submitted\"\n" +
                    "      means any form of electronic, verbal, or written communication sent\n" +
                    "      to the Licensor or its representatives, including but not limited to\n" +
                    "      communication on electronic mailing lists, source code control systems,\n" +
                    "      and issue tracking systems that are managed by, or on behalf of, the\n" +
                    "      Licensor for the purpose of discussing and improving the Work, but\n" +
                    "      excluding communication that is conspicuously marked or otherwise\n" +
                    "      designated in writing by the copyright owner as \"Not a Contribution.\"\n" +
                    "\n" +
                    "      \"Contributor\" shall mean Licensor and any individual or Legal Entity\n" +
                    "      on behalf of whom a Contribution has been received by Licensor and\n" +
                    "      subsequently incorporated within the Work.\n" +
                    "\n" +
                    "   2. Grant of Copyright License. Subject to the terms and conditions of\n" +
                    "      this License, each Contributor hereby grants to You a perpetual,\n" +
                    "      worldwide, non-exclusive, no-charge, royalty-free, irrevocable\n" +
                    "      copyright license to reproduce, prepare Derivative Works of,\n" +
                    "      publicly display, publicly perform, sublicense, and distribute the\n" +
                    "      Work and such Derivative Works in Source or Object form.\n" +
                    "\n" +
                    "   3. Grant of Patent License. Subject to the terms and conditions of\n" +
                    "      this License, each Contributor hereby grants to You a perpetual,\n" +
                    "      worldwide, non-exclusive, no-charge, royalty-free, irrevocable\n" +
                    "      (except as stated in this section) patent license to make, have made,\n" +
                    "      use, offer to sell, sell, import, and otherwise transfer the Work,\n" +
                    "      where such license applies only to those patent claims licensable\n" +
                    "      by such Contributor that are necessarily infringed by their\n" +
                    "      Contribution(s) alone or by combination of their Contribution(s)\n" +
                    "      with the Work to which such Contribution(s) was submitted. If You\n" +
                    "      institute patent litigation against any entity (including a\n" +
                    "      cross-claim or counterclaim in a lawsuit) alleging that the Work\n" +
                    "      or a Contribution incorporated within the Work constitutes direct\n" +
                    "      or contributory patent infringement, then any patent licenses\n" +
                    "      granted to You under this License for that Work shall terminate\n" +
                    "      as of the date such litigation is filed.\n" +
                    "\n" +
                    "   4. Redistribution. You may reproduce and distribute copies of the\n" +
                    "      Work or Derivative Works thereof in any medium, with or without\n" +
                    "      modifications, and in Source or Object form, provided that You\n" +
                    "      meet the following conditions:\n" +
                    "\n" +
                    "      (a) You must give any other recipients of the Work or\n" +
                    "          Derivative Works a copy of this License; and\n" +
                    "\n" +
                    "      (b) You must cause any modified files to carry prominent notices\n" +
                    "          stating that You changed the files; and\n" +
                    "\n" +
                    "      (c) You must retain, in the Source form of any Derivative Works\n" +
                    "          that You distribute, all copyright, patent, trademark, and\n" +
                    "          attribution notices from the Source form of the Work,\n" +
                    "          excluding those notices that do not pertain to any part of\n" +
                    "          the Derivative Works; and\n" +
                    "\n" +
                    "      (d) If the Work includes a \"NOTICE\" text file as part of its\n" +
                    "          distribution, then any Derivative Works that You distribute must\n" +
                    "          include a readable copy of the attribution notices contained\n" +
                    "          within such NOTICE file, excluding those notices that do not\n" +
                    "          pertain to any part of the Derivative Works, in at least one\n" +
                    "          of the following places: within a NOTICE text file distributed\n" +
                    "          as part of the Derivative Works; within the Source form or\n" +
                    "          documentation, if provided along with the Derivative Works; or,\n" +
                    "          within a display generated by the Derivative Works, if and\n" +
                    "          wherever such third-party notices normally appear. The contents\n" +
                    "          of the NOTICE file are for informational purposes only and\n" +
                    "          do not modify the License. You may add Your own attribution\n" +
                    "          notices within Derivative Works that You distribute, alongside\n" +
                    "          or as an addendum to the NOTICE text from the Work, provided\n" +
                    "          that such additional attribution notices cannot be construed\n" +
                    "          as modifying the License.\n" +
                    "\n" +
                    "      You may add Your own copyright statement to Your modifications and\n" +
                    "      may provide additional or different license terms and conditions\n" +
                    "      for use, reproduction, or distribution of Your modifications, or\n" +
                    "      for any such Derivative Works as a whole, provided Your use,\n" +
                    "      reproduction, and distribution of the Work otherwise complies with\n" +
                    "      the conditions stated in this License.\n" +
                    "\n" +
                    "   5. Submission of Contributions. Unless You explicitly state otherwise,\n" +
                    "      any Contribution intentionally submitted for inclusion in the Work\n" +
                    "      by You to the Licensor shall be under the terms and conditions of\n" +
                    "      this License, without any additional terms or conditions.\n" +
                    "      Notwithstanding the above, nothing herein shall supersede or modify\n" +
                    "      the terms of any separate license agreement you may have executed\n" +
                    "      with Licensor regarding such Contributions.\n" +
                    "\n" +
                    "   6. Trademarks. This License does not grant permission to use the trade\n" +
                    "      names, trademarks, service marks, or product names of the Licensor,\n" +
                    "      except as required for reasonable and customary use in describing the\n" +
                    "      origin of the Work and reproducing the content of the NOTICE file.\n" +
                    "\n" +
                    "   7. Disclaimer of Warranty. Unless required by applicable law or\n" +
                    "      agreed to in writing, Licensor provides the Work (and each\n" +
                    "      Contributor provides its Contributions) on an \"AS IS\" BASIS,\n" +
                    "      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or\n" +
                    "      implied, including, without limitation, any warranties or conditions\n" +
                    "      of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A\n" +
                    "      PARTICULAR PURPOSE. You are solely responsible for determining the\n" +
                    "      appropriateness of using or redistributing the Work and assume any\n" +
                    "      risks associated with Your exercise of permissions under this License.\n" +
                    "\n" +
                    "   8. Limitation of Liability. In no event and under no legal theory,\n" +
                    "      whether in tort (including negligence), contract, or otherwise,\n" +
                    "      unless required by applicable law (such as deliberate and grossly\n" +
                    "      negligent acts) or agreed to in writing, shall any Contributor be\n" +
                    "      liable to You for damages, including any direct, indirect, special,\n" +
                    "      incidental, or consequential damages of any character arising as a\n" +
                    "      result of this License or out of the use or inability to use the\n" +
                    "      Work (including but not limited to damages for loss of goodwill,\n" +
                    "      work stoppage, computer failure or malfunction, or any and all\n" +
                    "      other commercial damages or losses), even if such Contributor\n" +
                    "      has been advised of the possibility of such damages.\n" +
                    "\n" +
                    "   9. Accepting Warranty or Additional Liability. While redistributing\n" +
                    "      the Work or Derivative Works thereof, You may choose to offer,\n" +
                    "      and charge a fee for, acceptance of support, warranty, indemnity,\n" +
                    "      or other liability obligations and/or rights consistent with this\n" +
                    "      License. However, in accepting such obligations, You may act only\n" +
                    "      on Your own behalf and on Your sole responsibility, not on behalf\n" +
                    "      of any other Contributor, and only if You agree to indemnify,\n" +
                    "      defend, and hold each Contributor harmless for any liability\n" +
                    "      incurred by, or claims asserted against, such Contributor by reason\n" +
                    "      of your accepting any such warranty or additional liability.\n" +
                    "\n" +
                    "   END OF TERMS AND CONDITIONS\n" +
                    "\n" +
                    "   APPENDIX: How to apply the Apache License to your work.\n" +
                    "\n" +
                    "      To apply the Apache License to your work, attach the following\n" +
                    "      boilerplate notice, with the fields enclosed by brackets \"[]\"\n" +
                    "      replaced with your own identifying information. (Don't include\n" +
                    "      the brackets!)  The text should be enclosed in the appropriate\n" +
                    "      comment syntax for the file format. We also recommend that a\n" +
                    "      file or class name and description of purpose be included on the\n" +
                    "      same \"printed page\" as the copyright notice for easier\n" +
                    "      identification within third-party archives.\n" +
                    "\n" +
                    "   Copyright [yyyy] [name of copyright owner]\n" +
                    "\n" +
                    "   Licensed under the Apache License, Version 2.0 (the \"License\");\n" +
                    "   you may not use this file except in compliance with the License.\n" +
                    "   You may obtain a copy of the License at\n" +
                    "\n" +
                    "       http://www.apache.org/licenses/LICENSE-2.0\n" +
                    "\n" +
                    "   Unless required by applicable law or agreed to in writing, software\n" +
                    "   distributed under the License is distributed on an \"AS IS\" BASIS,\n" +
                    "   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.\n" +
                    "   See the License for the specific language governing permissions and\n" +
                    "   limitations under the License.\n";

            return licenseText;
        }

    }
}
