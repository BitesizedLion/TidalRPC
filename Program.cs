﻿using System;
using System.Net;
using System.Text;
using DiscordRPC;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Button = DiscordRPC.Button;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace TidalRPC
{
    internal static class Program
    {
        static DiscordRpcClient client;
        static NotifyIcon trayIcon;
        static TimeSpan lastActivity;
        static bool rpcEnabled = true;
        static bool showAds = false;

        [STAThread]
        static void Main()
        {
            // Create a new Discord RPC client
            client = new DiscordRpcClient("584458858731405315");

            // Subscribe to events
            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Presence Update!");
            };

            // Connect to the Discord RPC server
            client.Initialize();

            // Update checker
            _ = UpdateManager.CheckForUpdates();

            // Check if coreprops.json exists, if not, create it.
            Utils.CheckCoreProps();

            // Create the tray icon
            CreateTrayIcon();

            // Start the webserver
            _ = StartWebServer();

            // Start the inactivity check
            _ = InactivityCheck();

            // Needed for tray icon
            Application.Run();

            // Disconnect from the Discord RPC server
            client.Dispose();
        }



        static async Task StartWebServer()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(Utils.GetAddress());
            listener.Start();
            Console.WriteLine("Webserver started");

            while (true)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/game_event")
                {
                    // Read the request body
                    string requestBody = new StreamReader(context.Request.InputStream).ReadToEnd();

                    // Deserialize the JSON data
                    var gameEvent = JsonConvert.DeserializeObject<GameEvent>(requestBody);
                    var songData = gameEvent.Data.Frame;

                    trayIcon.ContextMenuStrip.Items[1].Text = songData.State == "playing" ? "Status: playing " + songData.Title : "Status: paused";

                    // Update the presence with the data from the request
                    if (rpcEnabled)
                    {
                        RichPresence presence = new RichPresence()
                        {
                            Details = songData.Title,
                            State = songData.Artist,
                            Timestamps = new Timestamps()
                            {
                                Start = DateTime.UtcNow - TimeSpan.FromSeconds(songData.Time),
                                End = DateTime.UtcNow + TimeSpan.FromSeconds(songData.Duration - songData.Time)
                            },
                            Assets = new Assets()
                            {
                                LargeImageKey = songData.ImageUrl,
                                LargeImageText = songData.Album,
                                SmallImageKey = songData.State
                            }
                        };

                        if (songData.Url != null)
                        {
                            presence.Buttons = new Button[]
                            {
                                new Button() { Label = "Open Song", Url = songData.Url }
                            };
                        };

                        // Untested
                        if(songData.Title == "Advertisement" && !showAds)
                        {
                            client.ClearPresence();
                        }
                        {
                            client.SetPresence(presence);
                        }

                        lastActivity = TimeSpan.FromTicks(DateTime.Now.Ticks);
                    } else { client.ClearPresence(); }
                }

                // Send a response
                var response = Encoding.UTF8.GetBytes("{\"status\": \"OK\"}");

                context.Response.ContentLength64 = response.Length;
                context.Response.OutputStream.Write(response, 0, response.Length);
                context.Response.Close();
            }
        }

        static async Task InactivityCheck()
        {
            while (true)
            {
                TimeSpan elapsedTime = TimeSpan.FromTicks(DateTime.Now.Ticks) - lastActivity;

                if (elapsedTime.TotalSeconds >= 5)
                {
                    client.ClearPresence();
                }

                await Task.Delay(1000);
            }
        }

        // Moved from ContextMenu to ContextMenuStrip because ContextMenu was so limited in what I could change.
        static void CreateTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Visible = true,
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text = "Tidal RPC"
            };

            var menu = new ContextMenuStrip();

            var topItem = new ToolStripMenuItem("Tidal RPC, by BitesizedLion v" + Assembly.GetExecutingAssembly().GetName().Version)
            {
                Enabled = false,
                Image = Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap()
            };

            var statusMenuItem = new ToolStripMenuItem("Status: nothing")
            {
                Enabled = false // We don't want user to be able to click the status
            };

            var toggleRPCMenuItem = new ToolStripMenuItem("Toggle RPC") { Checked = true, CheckOnClick = true };
            toggleRPCMenuItem.Click += (sender, e) => { rpcEnabled = !rpcEnabled; };
            
            var toggleAdsMenuItem = new ToolStripMenuItem("Toggle Ads") { CheckOnClick = true };
            toggleAdsMenuItem.Click += (sender, e) => { showAds = !showAds; };

            var toggleUpdatesMenuItem = new ToolStripMenuItem("Toggle Updates") { Checked = UpdateManager.IsUpdateCheckEnabled(),CheckOnClick = true };
            toggleUpdatesMenuItem.Click += (sender, e) => { UpdateManager.ToggleUpdateCheck(!UpdateManager.IsUpdateCheckEnabled()); }; // sry about this

            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += (sender, e) => Exit();

            menu.Items.Add(topItem);
            menu.Items.Add(statusMenuItem);
            menu.Items.Add(toggleRPCMenuItem);
            menu.Items.Add(toggleAdsMenuItem);
            menu.Items.Add(toggleUpdatesMenuItem);
            menu.Items.Add(exitMenuItem);
            trayIcon.ContextMenuStrip = menu;
        }

        // Exit the program
        static void Exit()
        {
            client.Dispose();
            trayIcon.Visible = false;
            trayIcon.Dispose();
            Environment.Exit(0);
        }

        class GameEvent
        {
            public string Game { get; set; }
            public string Event { get; set; }
            public Data Data { get; set; }
        }

        class Data
        {
            public int Value { get; set; }
            public Frame Frame { get; set; }
        }

        class Frame
        {
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Album { get; set; }
            public string Url { get; set; }
            public string ImageUrl { get; set; }
            public int Duration { get; set; }
            public int Time { get; set; }
            public string State { get; set; }
        }
    }
}