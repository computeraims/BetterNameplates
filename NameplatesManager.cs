using BetterNameplates.Models;
using BetterNameplates.Utils;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterNameplates
{
    class NameplatesManager : MonoBehaviour
    {
        public HyperCommand NameplateCommand;
        public static List<CSteamID> enabledNameplatesList;
        public static List<CSteamID> disabledNameplatesList;

        public void Awake()
        {
            Console.WriteLine("NameplatesManager loaded");
            ChatManager.onChatted += OnChatted;
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnPlayerDisconnected;
            NameplateCommand = new CommandNameplate();
            enabledNameplatesList = new List<CSteamID>();
            disabledNameplatesList = new List<CSteamID>();
        }

        private void OnPlayerConnected(SteamPlayer player)
        {
            if (Main.Config.nameplatesEnabled)
            {
                enabledNameplatesList.Add(player.playerID.steamID);
            } else
            {
                player.player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                disabledNameplatesList.Add(player.playerID.steamID);
            }
        }

        private void OnPlayerDisconnected(SteamPlayer player)
        {
            if (enabledNameplatesList.Contains(player.playerID.steamID))
            {
                enabledNameplatesList.Remove(player.playerID.steamID);
            } else if (disabledNameplatesList.Contains(player.playerID.steamID))
            {
                disabledNameplatesList.Remove(player.playerID.steamID);
            }
        }

        private void OnChatted(SteamPlayer player, EChatMode Mode, ref Color Color, ref bool isRich, string text, ref bool isVisible)
        {
            if (text[0] == '/')
            {
                isVisible = false;
                string[] InputArray = text.Split(' ');
                InputArray[0] = InputArray[0].Substring(1);
                string[] args = InputArray.Skip(1).ToArray();

                if (InputArray[0].ToLower() == "nameplate")
                {
                    NameplateCommand.execute(player.playerID.steamID, args);
                }
            }
        }

        public class CommandNameplate : HyperCommand
        {
            public CommandNameplate()
            {
                Name = "nameplate";
                Description = "Toggle your nameplate";
                Usage = "";
            }

            public override void execute(CSteamID executor, string[] args)
            {
                SteamPlayer player = PlayerTool.getSteamPlayer(executor);

                if (Main.Config.allowToggle || player.isAdmin)
                {
                    if (enabledNameplatesList.Contains(player.playerID.steamID))
                    {
                        enabledNameplatesList.Remove(player.playerID.steamID);
                        disabledNameplatesList.Add(player.playerID.steamID);
                        UnityThread.executeInUpdate(() =>
                        {
                            ChatManager.say(player.playerID.steamID, $"Nameplate disabled", Color.cyan);
                        });
                    }
                    else
                    {
                        disabledNameplatesList.Remove(player.playerID.steamID);
                        enabledNameplatesList.Add(player.playerID.steamID);
                        UnityThread.executeInUpdate(() =>
                        {
                            ChatManager.say(player.playerID.steamID, $"Nameplate enabled", Color.cyan);
                        });
                    }
                }
            }
        }

        public void Update()
        {
            foreach (SteamPlayer player in Provider.clients)
            {
                RaycastInfo thingLocated = TraceRay(player, 10f, RayMasks.PLAYER | RayMasks.PLAYER_INTERACT);

                if (thingLocated.player != null)
                {
                    if (thingLocated.player.name == player.player.name)
                    {
                        return;
                    }

                    if (enabledNameplatesList.Contains(thingLocated.player.channel.owner.playerID.steamID) && !player.player.isPluginWidgetFlagActive(EPluginWidgetFlags.ShowInteractWithEnemy))
                    {
                        player.player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                    } else if (disabledNameplatesList.Contains(thingLocated.player.channel.owner.playerID.steamID) && player.player.isPluginWidgetFlagActive(EPluginWidgetFlags.ShowInteractWithEnemy))
                    {
                        if (player.isAdmin && Main.Config.adminOverride)
                        {
                            return;
                        }

                        player.player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                    }
                }
            }
        }

        public RaycastInfo TraceRay(SteamPlayer player, float distance, int masks)
        {
            return DamageTool.raycast(new Ray(player.player.look.aim.position, player.player.look.aim.forward), distance, masks);
        }
    }
}
