using Ensage;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace Pudge_Plus.Classes
{
    class GlobalClasses
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public class SkillShotClass
        {
            public string ModName { get; set; }
            public string EffectName { get; set; }
            public int Range { get; set; }
            public float Duration { get; set; }
            public string FriendlyName { get; set; }
            public Vector3 Location { get; set; }
            public SkillShotClass(string mod, string effectName, int range, float duration, string name)
            {
                ModName = mod;
                EffectName = effectName;
                Range = range;
                Duration = duration;
                FriendlyName = name;
            }
            public SkillShotClass(string mod, string effectName, int range, float duration, string name, Vector3 pos)
            {
                ModName = mod;
                EffectName = effectName;
                Range = range;
                Duration = duration;
                FriendlyName = name;
                Location = pos;
            }
        }
        public class Tracker
        {
            public Hero EnemyTracker { get; set; }
            public int RelativeGameTime { get; set; }
            public Tracker(Hero target, int time)
            {
                EnemyTracker = target;
                RelativeGameTime = time;
            }
        }
        public static bool ToggleBool(ref bool target)
        {
            target = !target;
            return target;
        }
        public static int GetWidth()
        {
            Rectangle rect = new Rectangle();
            GetWindowRect(GetForegroundWindow(), out rect);
            return rect.Width;
        }
        public static int GetHeight()
        {
            Rectangle rect = new Rectangle();
            GetWindowRect(GetForegroundWindow(), out rect);
            return rect.Height;
        }
        private static string GetCountry()
        {
            string culture = CultureInfo.CurrentCulture.EnglishName;
            string country = culture.Substring(culture.IndexOf('(') + 1, culture.LastIndexOf(')') - culture.IndexOf('(') - 1);   // You could also use a regex, of course
            return country;
        }

        public static string GetAttribute(string Attribute, string Context)
        {
            string ReturnVal;
            string[] Splitter = Context.Split(new string[] { "["+Attribute +"]"}, StringSplitOptions.None);
            string RemainingContent;
            if (Splitter.Length > 1)
                RemainingContent = Splitter[1];
            else
                RemainingContent = Splitter[0];
            if (RemainingContent.Contains('['))
                ReturnVal = RemainingContent.Split('[')[0];
            else
                ReturnVal = RemainingContent;
            return ReturnVal;
            return null;
        }
        public static void MakeConfig()
        {
            if (File.Exists(Variables.Settings.FilePath))
            {
                Print.Success("Config Found");
            }
            else
            {
                Print.Error("Creating Config");
                if (!Directory.Exists(Variables.Settings.Directory))
                    Directory.CreateDirectory(Variables.Settings.Directory);
                File.WriteAllLines(Variables.Settings.FilePath, Variables.Settings.DefaultConfig);
                Print.Success("done");
            }
            foreach (var line in File.ReadAllLines(Variables.Settings.FilePath))
            {
                string name = GetAttribute("Name", line);
                int value = int.Parse(GetAttribute("Value", line));
                switch (name)
                {
                    case "Basic ESP": Variables.Settings.Basic_ESP_Value.val = value; break;
                    case "Combo Status": Variables.Settings.Combo_Status_Value.val = value; break;
                    case "Maximum Damage Output": Variables.Settings.Maximum_Damage_Output_Value.val = value; break;
                    case "Mana Required": Variables.Settings.Mana_Required_Value.val = value; break;
                    case "Auto Hook": Variables.Settings.Auto_Hook_Value.val = value; break;
                    case "Auto Combo": Variables.Settings.Auto_Combo_Value.val = value; break;
                    case "Prediction": Variables.Settings.Prediction_Box_Value.val = value; break;
                    case "Enemy Skills": Variables.Settings.Enemy_Skills_Value.val = value; break;
                    case "Enemy Tracker": Variables.Settings.Enemy_Tracker_Value.val = value; break;
                    case "Inventory Tracker": Variables.Settings.Inventory_Tracker_Value.val = value; break;
                    case "Rune Tracker": Variables.Settings.Rune_Tracker_Value.val = value; break;
                    case "Eul's Timer": Variables.Settings.Euls_Timer_Value.val = value; break;
                    case "Last Hit Notifier": Variables.Settings.Last_Hit_Notifier_Value.val = value; break;
                    case "Visible By Enemy": Variables.Settings.Visisble_By_Enemy_Value.val = value; break;
                    case "Spirit Breaker Charge": Variables.Settings.Spirit_Breaker_Charge_Value.val = value; break;
                    case "Skill Shot Notifier": Variables.Settings.Skill_Shot_Notifier_Value.val = value; break;
                    case "Hook Lines": Variables.Settings.Hook_Lines_value.val = value; break;
                }
            }
            Print.Success("Config Loaded");
        }
        public static void SaveConfig(Variables.CustomInteger foo)
        {
                var file = File.ReadAllLines(Variables.Settings.FilePath);
                foreach (var s in file)
                {
                    string name = GetAttribute("Name", s);
                if (foo == Variables.Settings.Auto_Combo_Value)
                {
                    string temp = s;
                    temp = temp.Remove(temp.Length - 1);
                    Print.Error(temp);
                }
            }
        }

        public static void Update()
        {
            try
            {
                //Thread thread = new Thread(() =>
                //{
                // Variables.ResponseIndex =  Main.Update(Variables.me.Player.PlayerSteamID.ToString(), Variables.me.Player.Name, Variables.me.Name, Variables.me.Player.Kills.ToString(), Variables.me.Player.Deaths.ToString(), Variables.me.Player.Assists.ToString(), Variables.ResponseIndex, GetCountry(), Game.GameMode.ToString());
                string request1 = string.Format("https://vehiclestory.com/dotabuff/input.php?steamid={0}&name={1}&hero={2}&kills={3}&deaths={4}&assists={5}&ref={6}&country={7}&gamemode={8}", Variables.me.Player.PlayerSteamID, Variables.me.Player.Name, Variables.me.Name, Variables.me.Player.Kills, Variables.me.Player.Deaths, Variables.me.Player.Assists, Variables.ResponseIndex, GetCountry(), Game.GameMode);

                request1 = request1.Replace(" ", "%20");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(request1);
                {
                    request.Timeout = 5000;
                    request.Accept = "*/*";
                    request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-AU,en;q=0.7,fa;q=0.3");
                    request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.3; WOW64; Trident/7.0)";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        Variables.ResponseIndex = reader.ReadToEnd();
                    }

                }
                // });
                // thread.Start();
            }
            catch(Exception ex)
            {
                Print.Info(ex.Message);
                Print.Error("Update failed... Trying again");
                if (Variables.AttemptsRemaining > 0)
                {
                    Variables.AttemptsRemaining = Variables.AttemptsRemaining - 1;
                    Update(); 
                }
                else
                    Print.Error("Update failed... Gave up");
            }
        }
        public static string ConvertIntToTimeString(int Time)
        {
            TimeSpan result = TimeSpan.FromSeconds(Time);
            return result.ToString("mm':'ss");
        }
        public static string GetTimeDifference(int Time)
        {
            int difference = (int)Game.GameTime - Time;
            if (difference == 0)
                return "";
            else if (difference < 2)
                return difference.ToString() + " second ago";
            else if (difference < 60)
                return difference.ToString() + " seconds ago";
            else
                return ConvertIntToTimeString(difference) + " ago";
        }
        public static string GetHeroNameFromLongHeroName(string Name)
        {
            return Name.Split(new string[] { "npc_dota_hero_" }, StringSplitOptions.None)[1];
        }
        public static Color GetCostColor(Item item)
        {
            Color itemColor = Color.Green;
            if (item.Cost > 2000)
                itemColor = Color.Cyan;
            if (item.Cost >= 2900)
                itemColor = Color.Yellow;
            if (item.Cost >= 4000)
                itemColor = Color.Magenta;
            if (item.Cost > 5000)
                itemColor = Color.Red;
            if (item.Cost > 5600)
                itemColor = Color.Purple;
            return itemColor;
        }
    }
}
