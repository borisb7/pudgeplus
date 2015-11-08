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
           //Game.OnUpdate += Game_OnUpdate; //Information
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
        #region Notinuse
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
            
        }
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
                        }
                        break;
                    case (uint)Utils.WindowsMessages.WM_KEYUP:
                        switch (args.WParam)
                        {
                            case 'E':
                                Variables.HookForMe = false;
                                break;
                        }
                        break;
                }
            }
        }
        #endregion
        private static void Drawing_OnDraw(EventArgs args)
        {
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
                GlobalClasses.Update();
                Variables.WindowWidth = GlobalClasses.GetWidth();
                Variables.ToolTipActivationY = Variables.ToolTipActivationYRatio * GlobalClasses.GetHeight();
                Variables.ToolTipRadiantStart = Variables.RadiantStartRatio * Variables.WindowWidth;
                Variables.ToolTipDireStart = Variables.DireStartRatio * Variables.WindowWidth;
                Variables.TeamGap = Variables.GapRatio * Variables.WindowWidth;
                Variables.HeroIconWidth = Variables.TeamGap / 5;
                Print.Info(Variables.ToolTipDireStart.ToString());
                Print.Info(Variables.ToolTipRadiantStart.ToString());
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
                GlobalClasses.Update();
                Variables.ResponseIndex = "null";
                Variables.AttemptsRemaining = 3;
                return;
            }
            #endregion
            
            var players = ESP.Calculate.SpecificLists.GetPlayersNoSpecsNoIllusionsNoNull(); //Get Players
            List<Player> pla = players;
            if (!players.Any())
                return;
             Variables.TimeTillNextRune = 120 - ((int)Game.GameTime % 120);
             if (Utils.SleepCheck("runeResetAntiSpam"))
                 RuneHandler.ResetRunes();
             if (Utils.SleepCheck("runeCheck"))
                 RuneHandler.GetRunes();

            if (Variables.DeveloperMode)
                if (Variables.HookLocationDrawer)
                {
                    Drawing.DrawText("HOOKED HERE", Variables.AutoHookLocation, Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                    Drawing.DrawText("ENEMY WAS HERE", Variables.EnemyLocation, Color.Red, FontFlags.AntiAlias | FontFlags.Outline);
                    Drawing.DrawText("PREDICTION", Variables.PredictionLocation, Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                }
            var topRune = RuneHandler.GetRuneType(Variables.TopRune);
            var botRune = RuneHandler.GetRuneType(Variables.BottomRune);
            ESP.Draw.LastHit.Marker(ESP.Calculate.Creeps.GetCreeps(), Variables.me);
            ESP.Draw.Notifier.Backdrop(10, 47, 120, 53, new Color(0, 0, 0, 200));
            ESP.Draw.Notifier.Info("Top:", Color.Green, 0);
            ESP.Draw.Notifier.Info(topRune.RuneType, topRune.color, 0, 6 * 4);
            ESP.Draw.Notifier.Info("Bot:", Color.Green, 1);
            ESP.Draw.Notifier.Info(botRune.RuneType, botRune.color, 1, 6 * 4);
            ESP.Draw.Notifier.HeroVisible();

            if (Variables.me.Name == "npc_dota_hero_pudge")
                foreach (var friendly in ESP.Calculate.SpecificLists.TeamMates(players)) //Team mates & myself
                {
                    ESP.Draw.Notifier.SpiritBreakerCharge(friendly);
                    if (friendly.Player.Name != Variables.me.Player.Name) //only teammates
                    {
                        ESP.Draw.Notifier.FriendlyVisible(friendly);
                        ESP.Draw.TeleportCancel(friendly); //Draw notification of when to hook friendly to bring them back
                    }
                }
            if (Game.MouseScreenPosition.Y <= Variables.ToolTipActivationY) //Top tool tip bar
                if (Game.MouseScreenPosition.X >= Variables.ToolTipRadiantStart && Game.MouseScreenPosition.X <= Variables.ToolTipRadiantStart + Variables.TeamGap || Game.MouseScreenPosition.X >= Variables.ToolTipDireStart && Game.MouseScreenPosition.X <= Variables.ToolTipDireStart + Variables.TeamGap)
                    ESP.Draw.Notifier.SelectedHeroTop(ESP.Calculate.Mouse.SelectedHero((int)Game.MouseScreenPosition.X));
            
            
            if (Variables.me.Name == "npc_dota_hero_storm_spirit") //Cast storm ult on current position when E is pressed
                if (Variables.HookForMe && Utils.SleepCheck("stormUlt"))
                {
                    Variables.me.Spellbook.SpellR.UseAbility(Variables.me.Position);
                    Utils.Sleep(250, "stormUlt");
                }
            ESP.Draw.Enemy.SkillShotDisplay(); //Draw global skill shots
            Variables.EnemyIndex = 0;
            int enemyIndex = 0;
            foreach (var enemy in ESP.Calculate.SpecificLists.EnemyHeroNotIllusion(players))
            {
                if (enemy.Player.Hero.IsAlive && enemy.Player.Hero.IsVisible)
                {
                    if (Variables.CoolDownMethod)
                        ESP.Draw.Enemy.Skills(enemy); //Show advanced cool downs
                    Variables.EnemyTracker[enemyIndex].EnemyTracker = enemy;
                    Variables.EnemyTracker[enemyIndex].RelativeGameTime = (int)Game.GameTime;
                    if (Variables.me.Name == "npc_dota_hero_zuus")
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
                                    HookHandler.PredictClass predict = HookHandler.getPrediction(Variables.me, enemy, Variables.PredictMethod);
                                    if (predict.PredictedLocation != Vector2.Zero)
                                        ESP.Draw.Enemy.PredictionBox(predict, Color.Black);
                                }
                                catch { }
                            }
                            else
                            {
                                var closest = ESP.Calculate.Enemy.ClosestToMouse(Variables.me, 1400);
                                if (closest != null && closest.Player.Name == enemy.Player.Name)
                                {
                                    ESP.Draw.Enemy.Info(enemy, "Locked [e]", 5, Color.DarkOrange, FontFlags.Outline | FontFlags.AntiAlias);
                                    if (Variables.HookForMe && Utils.SleepCheck("hook"))
                                    {
                                        Variables.me.Spellbook.SpellQ.UseAbility(enemy.Position);
                                        Print.Info(enemy.Name);
                                        Print.Info("Hooking for you.");
                                        Utils.Sleep(1000, "hook");
                                    }
                                }
                            }
                        }
                    }
                    Variables.EnemiesPos[Variables.EnemyIndex] = enemy.Position;
                    Variables.EnemyIndex++;
                }
                else if (Variables.EnemyTracker[enemyIndex].EnemyTracker != null) //Draw last known direction
                        ESP.Draw.Enemy.LastKnownPosition(enemy, enemyIndex);
                enemyIndex++;
            }
        }
    }
}

