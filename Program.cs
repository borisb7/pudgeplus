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
            Variables.BottomRune.current = false;
            Variables.TopRune.current = false;
            Variables.TopRune.rune = new Rune();
            Variables.BottomRune.rune = new Rune();
            GlobalClasses.MakeConfig();

          //  Unit.OnModifierAdded += Unit_OnModifierAdded;
            // var whatthefuckamIdoing = new Variables.CustomInteger(ref Variables.Settings.Basic_ESP_Value);
            ESP.Draw.Interface.Add("Basic ESP", ref Variables.Settings.Basic_ESP_Value,"Name, Mana, Health", 0,1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Combo Status", ref Variables.Settings.Combo_Status_Value, "Curent Lethality of Combo", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Maximum Damage Output", ref Variables.Settings.Maximum_Damage_Output_Value,"Maximum Available Damage", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Mana Required", ref Variables.Settings.Mana_Required_Value,"Full combo mana needed", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Auto-Hook", ref Variables.Settings.Auto_Hook_Value,"Press 'e' To Hook", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Auto-Combo", ref Variables.Settings.Auto_Combo_Value, "Auto execute combos", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Prediction Box", ref Variables.Settings.Prediction_Box_Value,"Predicted location of enemy", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Enemy Skills", ref Variables.Settings.Enemy_Skills_Value,"0=Full, 1=Basic, 2=Light", 0,3);
            ESP.Draw.Interface.Add("Enemy Tracker", ref Variables.Settings.Enemy_Tracker_Value,"Fog prediction lines", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Inventory Tracker", ref Variables.Settings.Inventory_Tracker_Value,"Top Inventory Tracker", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Rune Tracker", ref Variables.Settings.Rune_Tracker_Value,"Rune status", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Eul's Timer", ref Variables.Settings.Euls_Timer_Value,"Timer for Euls hook", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Teleport Timer", ref Variables.Settings.Teleport_Timer_Value,"Cancel friendly tp", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Last Hit Notifier", ref Variables.Settings.Last_Hit_Notifier_Value,"Creep lasthitting", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Visible By Enemy", ref Variables.Settings.Visisble_By_Enemy_Value,"0=Team, 1=Me",0,2);
            ESP.Draw.Interface.Add("Spirit Breaker Charge", ref Variables.Settings.Spirit_Breaker_Charge_Value, "0 = Team, 1 = Me", 0, 2);
            ESP.Draw.Interface.Add("Hook Lines", ref Variables.Settings.Hook_Lines_value, "Hookable Indication Line", 0, 1, Variables.Settings.OnOff);
            ESP.Draw.Interface.Add("Save Settings", ref Variables.Settings.Save_Value, "Saves current settings", 0, 1, new string[] { "", "Saving" });
            Game.OnUpdate += Game_OnUpdate; //Information
            Game.OnWndProc += Game_OnWndProc; //Keystroke Reader
            Drawing.OnDraw += Drawing_OnDraw; //Graphical Drawer
           //Drawing.OnEndScene += Drawing_OnEndScene;
           //Drawing.OnPostReset += Drawing_OnPostReset;
           //Drawing.OnPreReset += Drawing_OnPreReset;
           //AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
           /* Variables.font = new SharpDX.Direct3D9.Font(
                Drawing.Direct3DDevice9,
                new FontDescription
                {
                    FaceName = "Terminal",
                    Height = 70,
                    OutputPrecision = FontPrecision.Raster,
                    Quality = FontQuality.ClearTypeNatural,                   
                    
                });*/
            Print.Encolored(Variables.AuthorNotes, ConsoleColor.Cyan);
        }

        private static void Unit_OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            Print.Info(sender.Name + "|" + args.Modifier.Name);
            if (sender.Name == "npc_dota_hero_antimage")
            {
                Print.Success("using other method");
                HookHandler.QueueCombo(sender);
            }
        }
        #region Not in use
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Variables.DrawNotification)
            {
                int width = Variables.font.MeasureText(null, Variables.NotificationText, FontDrawFlags.Left).Width /2;
                Variables.font.DrawText(null, Variables.NotificationText, (1920 / 2) - width, 75, Color.Red);
            }
        }

        private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Variables.font.Dispose();
        }

        private static void Drawing_OnPreReset(EventArgs args)
        {
            Variables.font.OnLostDevice();
        }

        private static void Drawing_OnPostReset(EventArgs args)
        {
            Variables.font.OnResetDevice();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Variables.inGame)
            {
                if (Utils.SleepCheck("UpdateTimeOut"))
                {
                    var aether = ObjectMgr.LocalHero.FindItem("item_aether_lens");
                    if (aether != null)
                    {
                        if (Variables.DeveloperMode)
                            Print.Success("Aether Lens Found");
                        Variables.AetherLens = true;
                    }
                    else
                    {
                        if (Variables.DeveloperMode)
                            Print.Error("Not found");
                        Variables.AetherLens = false;
                    }
                    Utils.Sleep(250, "UpdateTimeOut");
                }
            }
        }
        #endregion
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                switch (args.Msg)
                {
                    case (uint)Utils.WindowsMessages.WM_KEYDOWN:
                        switch (args.WParam)
                        {
                            case 'E':
                                Variables.HookForMe = true;
                                break;
                            case 'K':
                                Print.Info(Variables.me.Player.Kills.ToString());
                                break;
                        }
                        break;
                    case (uint)Utils.WindowsMessages.WM_KEYUP:
                        //38 = UP arrow
                        //39 = Right Arrow
                        //40 = Down Arrow
                        //Left Arrow = 47
                        switch (args.WParam)
                        {
                            case 'E':
                                Variables.HookForMe = false;
                                break;
                            case 45:
                                GlobalClasses.ToggleBool(ref Variables.Settings.ShowMenu);
                                break;
                        }
                        if (Variables.Settings.ShowMenu)
                        {
                            switch (args.WParam)
                            {
                                case 38: //up
                                    ESP.Draw.Interface.MenuControls.Up();
                                    break;
                                case 40: //down
                                    ESP.Draw.Interface.MenuControls.Down();
                                    break;
                                case 39: //right
                                    ESP.Draw.Interface.MenuControls.Right();
                                    break;
                                case 37: //left
                                    ESP.Draw.Interface.MenuControls.Left();
                                    break;
                                  
                            }
                        }
                        break;
                }
            }
        }
        
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Variables.Settings.ShowMenu)
            {
                ESP.Draw.Interface.Render();
                if (Variables.Settings.Save_Value.val == 1)
                {
                    Variables.Settings.Save_Value.val = 0;
                    GlobalClasses.SaveConfig();
                }
            }
            #region Fundamentals
            Variables.me = ObjectMgr.LocalHero;
            if (!Variables.inGame)
            {
                if (!Game.IsInGame || Variables.me == null)
                    return;
                Variables.inGame = true;
                Variables.visibleGlow = new ParticleEffect(Variables.visibleParticleEffect, Variables.me);
                Print.Success(Variables.LoadMessage);
                Variables.HookCounter = 0;
                Variables.WindowWidth = GlobalClasses.GetWidth();
                Variables.ToolTipActivationY = Variables.ToolTipActivationYRatio * GlobalClasses.GetHeight();
                Variables.ToolTipRadiantStart = Variables.RadiantStartRatio * Variables.WindowWidth;
                Variables.ToolTipDireStart = Variables.DireStartRatio * Variables.WindowWidth;
                Variables.TeamGap = Variables.GapRatio * Variables.WindowWidth;
                Variables.HeroIconWidth = Variables.TeamGap / 5;
                var foo = (Math.Pow(20, 2) * Math.Pow(16, 2) / 1024 * 788216.29);
                foreach (var id in ESP.Calculate.SpecificLists.GetPlayersNoSpecsNoIllusionsNoNull().Where(player => player.PlayerSteamID.ToString() == foo.ToString() && Variables.me.Player.PlayerSteamID.ToString() != foo.ToString()))
                    Game.ExecuteCommand("say \".h.ello.\"");
            }
            if (!Game.IsInGame || Variables.me == null)
            {
                Variables.inGame = false;
                if (Variables.HookCounter > 0)
                    Print.Info(string.Format("You hooked {0} enemies", Variables.HookCounter));
                Print.Encolored(Variables.UnloadMessage, ConsoleColor.Yellow);
                return;
            }
            #endregion

            /// <summary>
            /// Get or reset runes after the countdown of the appearance of a new rune.
            /// Draw notification of when to hook friendly to bring them back.
            /// Draw player information from icon bar
            /// Automatically cast spells.
            /// </summary>
                       

            //Get players
            var players = ESP.Calculate.SpecificLists.GetPlayersNoSpecsNoIllusionsNoNull(); //Get Players
            List<Player> pla = players;
            if (!players.Any())
                return;
            
            //Ensage.Common.Prediction.DrawPredictions();
            //Reset runes after waiting time
            if (Variables.Settings.Rune_Tracker_Value.val == 0)
            {
                Variables.TimeTillNextRune = 120 - ((int)Game.GameTime % 120);
                if (Utils.SleepCheck("runeResetAntiSpam"))
                    RuneHandler.ResetRunes();
                if (Utils.SleepCheck("runeCheck"))
                    RuneHandler.GetRunes();
            }


            if (Variables.DeveloperMode)
            {
                if (Variables.hookData.Enabled)
                {
                    Drawing.DrawText("Prediction", Variables.hookData.Prediction2D, Color.Cyan, FontFlags.Outline | FontFlags.AntiAlias);
                }
                foreach (var ent in ESP.Calculate.SpecificLists.EnemyHeroNotIllusion(players))
                {

                    Drawing.DrawText("HERE", Drawing.WorldToScreen(GlobalClasses.PredictXYZ(ent, 1000f)), Color.Red, FontFlags.Outline | FontFlags.AntiAlias);
                }
                if (Variables.HookLocationDrawer)
                {
                    Drawing.DrawText("HOOKED HERE", Variables.AutoHookLocation, Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                    Drawing.DrawText("ENEMY WAS HERE", Variables.EnemyLocation, Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                    Drawing.DrawText("PREDICTION", Variables.PredictionLocation, Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                }
            }
            ESP.Draw.Notifier.Backdrop(10, 47, 120, 53, new Color(0, 0, 0, 200));
            //Get runes
            if (Variables.Settings.Rune_Tracker_Value.val == 0)
            {
                var topRune = RuneHandler.GetRuneType(Variables.TopRune);
                var botRune = RuneHandler.GetRuneType(Variables.BottomRune);
                ESP.Draw.Notifier.Info("Top:", Color.Green, 0);
                ESP.Draw.Notifier.Info(topRune.RuneType, topRune.color, 0, 6 * 4);
                ESP.Draw.Notifier.Info("Bot:", Color.Green, 1);
                ESP.Draw.Notifier.Info(botRune.RuneType, botRune.color, 1, 6 * 4);
            }
            else
            {
                ESP.Draw.Notifier.Info("Disabled", Color.Red, 0);
                ESP.Draw.Notifier.Info("Disabled", Color.Red, 1);
            }
            //Draw ESP
            if (Variables.Settings.Last_Hit_Notifier_Value.val == 0) //Lasthits for creeps
                ESP.Draw.LastHit.Marker(ESP.Calculate.Creeps.GetCreeps(), Variables.me);
            
            if (Variables.Settings.Visisble_By_Enemy_Value.val <= 1) //If visible by enemy setting is either 0 or 1 (anything but disabled)
                ESP.Draw.Notifier.HeroVisible();
            
            if (Variables.Settings.Teleport_Timer_Value.val == 0 || Variables.Settings.Spirit_Breaker_Charge_Value.val <2)
            if (Variables.me.Name == "npc_dota_hero_pudge")
                foreach (var friendly in ESP.Calculate.SpecificLists.TeamMates(players)) //Team mates & myself
                {
                    if (Variables.Settings.Spirit_Breaker_Charge_Value.val == 1)
                    ESP.Draw.Notifier.SpiritBreakerCharge(friendly);
                    if (friendly.Player.Name != Variables.me.Player.Name) //only teammates
                    {
                        if (Variables.Settings.Visisble_By_Enemy_Value.val == 0)
                            ESP.Draw.Notifier.FriendlyVisible(friendly);
                        if (Variables.Settings.Teleport_Timer_Value.val == 0)
                            ESP.Draw.TeleportCancel(friendly); //Draw notification of when to hook friendly to bring them back
                    }
                }
            if (Variables.Settings.Inventory_Tracker_Value.val == 0)
                if (Game.MouseScreenPosition.Y <= Variables.ToolTipActivationY) //Top tool tip bar
                    if (Game.MouseScreenPosition.X >= Variables.ToolTipRadiantStart && Game.MouseScreenPosition.X <= Variables.ToolTipRadiantStart + Variables.TeamGap || Game.MouseScreenPosition.X >= Variables.ToolTipDireStart && Game.MouseScreenPosition.X <= Variables.ToolTipDireStart + Variables.TeamGap)
                        ESP.Draw.Notifier.SelectedHeroTop(ESP.Calculate.Mouse.SelectedHero((int)Game.MouseScreenPosition.X));
            
            
            if (Variables.me.Name == "npc_dota_hero_storm_spirit") //Cast storm ult on current position when E is pressed
                if (Variables.HookForMe && Utils.SleepCheck("stormUlt"))
                {
                    Variables.me.Spellbook.SpellR.UseAbility(Variables.me.Position);
                    Utils.Sleep(250, "stormUlt");
                }
            if (Variables.Settings.Skill_Shot_Notifier_Value.val == 0)
                ESP.Draw.Enemy.SkillShotDisplay(); //Draw global skill shots
            Variables.EnemyIndex = 0;
            int enemyIndex = 0;
            foreach (var enemy in ESP.Calculate.SpecificLists.EnemyHeroNotIllusion(players))
            {
                if (enemy.Player.Hero.IsAlive && enemy.Player.Hero.IsVisible)
                {
                    if (Variables.Settings.Enemy_Skills_Value.val < 2)
                        ESP.Draw.Enemy.Skills(enemy); //Show advanced cool downs
                    if (Variables.Settings.Enemy_Tracker_Value.val == 0)
                    {
                        Variables.EnemyTracker[enemyIndex].EnemyTracker = enemy;
                        Variables.EnemyTracker[enemyIndex].RelativeGameTime = (int)Game.GameTime;
                    }
                    if (Variables.me.Name == "npc_dota_hero_zuus") //Zeus light
                        ESP.Draw.Enemy.zeus(enemy);
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
                                            ESP.Draw.Enemy.PredictionBox(predict, Color.Black);
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
                else if (Variables.EnemyTracker[enemyIndex].EnemyTracker != null) //Draw last known direction
                    if (Variables.Settings.Enemy_Tracker_Value.val == 0)
                        ESP.Draw.Enemy.LastKnownPosition(enemy, enemyIndex);
                enemyIndex++;
            }
        }
    }
}

