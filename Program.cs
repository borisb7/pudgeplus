using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using Ensage;

using Ensage.Common;

using SharpDX;

using SharpDX.Direct3D9;

using Ensage.Common.Extensions;

using Pudge_Plus.Classes;

using System.Diagnostics;

using System.Net;

using System.Net.NetworkInformation;

using System.Globalization;

using System.IO;



namespace Pudge_Plus

{

    class Program

    {

        

        static void Main(string[] args)

        {

            

            GlobalClasses.MakeConfig();

            

           // var whatthefuckamIdoing = new Variables.CustomInteger(ref Variables.Settings.Basic_ESP_Value);

           

            ESP.Draw.Interface.Add("Combo Status", ref Variables.Settings.Combo_Status_Value, "Curent Lethality of Combo", 0, 1, Variables.Settings.OnOff);

            ESP.Draw.Interface.Add("Maximum Damage Output", ref Variables.Settings.Maximum_Damage_Output_Value,"Maximum Available Damage", 0, 1, Variables.Settings.OnOff);

            ESP.Draw.Interface.Add("Auto-Hook", ref Variables.Settings.Auto_Hook_Value,"Press 'e' To Hook", 0, 1, Variables.Settings.OnOff);

            ESP.Draw.Interface.Add("Auto-Combo", ref Variables.Settings.Auto_Combo_Value, "Auto execute combos", 0, 1, Variables.Settings.OnOff);

            ESP.Draw.Interface.Add("Prediction Box", ref Variables.Settings.Prediction_Box_Value,"Predicted location of enemy", 0, 1, Variables.Settings.OnOff);

         

          

           

           

            

            

            Game.OnWndProc += Game_OnWndProc; //Keystroke Reader

            Drawing.OnDraw += Drawing_OnDraw; //Graphical Drawer

         

            

        }



                }

                

            }

        }

    }

}
