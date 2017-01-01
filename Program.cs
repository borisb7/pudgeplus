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
        #region Not in use
        

      
            #endregion

         

            //Get players
            var players = ESP.Calculate.SpecificLists.GetPlayersNoSpecsNoIllusionsNoNull(); //Get Players
            List<Player> pla = players;
            if (!players.Any())
                return;

            //Reset runes after waiting time
            


            
        




            Variables.Settings.Skill_Shot_Notifier_Value.val = 0;
            
            if (Variables.Settings.Skill_Shot_Notifier_Value.val == 0)
                ESP.Draw.Enemy.SkillShotDisplay(); //Draw global skill shots
            Variables.EnemyIndex = 0;
            int enemyIndex = 0;
            foreach (var enemy in ESP.Calculate.SpecificLists.EnemyHeroNotIllusion(players))
            {
                if (enemy.Player.Hero.IsAlive && enemy.Player.Hero.IsVisible)
                {
                    Variables.Settings.Enemy_Tracker_Value.val = 0;
                    if (Variables.Settings.Enemy_Tracker_Value.val == 0)
                    {
                        Variables.EnemyTracker[enemyIndex].EnemyTracker = enemy;
                        Variables.EnemyTracker[enemyIndex].RelativeGameTime = (int)Game.GameTime;
                    }
                    
                    if (enemy.Distance2D(ObjectMgr.LocalHero) <= 2000)
                    {
                        ESP.Draw.Enemy.basic(enemy);
                        if (Variables.me.Name == "npc_dota_hero_pudge")
                        {
                            HookHandler.main(enemy);
                            ESP.Draw.Enemy.pudge(enemy);
                            if (ESP.Calculate.Enemy.isMoving(enemy.Position, Variables.EnemyIndex))
                            {
                                try
                                {
                                    if (Variables.Settings.Prediction_Box_Value.val == 0)
                                    {
                                        HookHandler.PredictClass predict = HookHandler.getPrediction(Variables.me, enemy, Variables.PredictMethod);
                                        if (predict.PredictedLocation != Vector2.Zero)
                                            ESP.Draw.Enemy.PredictionBox(predict, Color.Red);
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                if (Variables.Settings.Auto_Hook_Value.val == 0)
                                {
                                    var closest = ESP.Calculate.Enemy.ClosestToMouse(Variables.me, 1400);
                                    if (closest != null && closest.Player.Name == enemy.Player.Name)
                                    {
                                        ESP.Draw.Enemy.Info(enemy, "Locked [e]", 5, Color.DarkOrange, FontFlags.Outline | FontFlags.AntiAlias);
                                        if (Variables.HookForMe && Utils.SleepCheck("hook"))
                                        {
                                            Variables.me.Spellbook.SpellQ.UseAbility(enemy.Position);
                                           // Print.Info(enemy.Name);
                                            Print.Info("Hooking for you.");
                                            Utils.Sleep(1000, "hook");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Variables.EnemiesPos[Variables.EnemyIndex] = enemy.Position;
                    Variables.EnemyIndex++;
                }
                
            }
        }
    }
}
