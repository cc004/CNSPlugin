﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Microsoft.Xna.Framework;
using OTAPI;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Net;
using Terraria.UI.Chat;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace TempGroup
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        public Plugin(Main game) : base(game)
        {
        }

		public override void Initialize()
		{
			PlayerHooks.PlayerPermission += PlayerHooks_PlayerPermission;
			var handlers = ServerApi.Hooks.ServerChat;
			var registrations = (IEnumerable)handlers.GetType()
				.GetField("registrations", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(handlers);
			var registratorfield = registrations.GetType().GenericTypeArguments[0]
				.GetProperty("Registrator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var handlerfield = registrations.GetType().GenericTypeArguments[0]
				.GetProperty("Handler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var registration in registrations)
			{
				var handler = (HookHandler<ServerChatEventArgs>)handlerfield.GetValue(registration);
				var plugin = (TerrariaPlugin)registratorfield.GetValue(registration);
				if (plugin is TShock)
				{
					TShock.Log.ConsoleInfo("TShock server chat handled forced to de-register");
					ServerApi.Hooks.ServerChat.Deregister(plugin, handler);
					ServerApi.Hooks.ServerChat.Register(plugin, OnChat);
				}
			}
		}
		private static void OnChat(ServerChatEventArgs args)
		{
			if (args.Handled)
			{
				return;
			}
			var tsplayer = TShock.Players[args.Who];
			if (tsplayer == null)
			{
				args.Handled = true;
				return;
			}
			if (args.Text.Length > 500)
			{
				tsplayer.Kick("Crash attempt via long chat packet.", true, false, null, false);
				args.Handled = true;
				return;
			}
			string text = args.Text;
			foreach (KeyValuePair<LocalizedText, ChatCommandId> keyValuePair in ChatManager.Commands._localizedCommands)
			{
				if (keyValuePair.Value._name == args.CommandId._name)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text = keyValuePair.Key.Value + " " + text;
						break;
					}
					text = keyValuePair.Key.Value;
					break;
				}
			}
			if ((text.StartsWith(TShock.Config.Settings.CommandSpecifier) || text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)) && !string.IsNullOrWhiteSpace(text.Substring(1)))
			{
				try
				{
					args.Handled = true;
					if (!TShockAPI.Commands.HandleCommand(tsplayer, text))
					{
						tsplayer.SendErrorMessage("Unable to parse command. Please contact an administrator for assistance.");
						TShock.Log.ConsoleError("Unable to parse command '{0}' from player {1}.", new object[]
						{
					text,
					tsplayer.Name
						});
					}
					return;
				}
				catch (Exception ex)
				{
					TShock.Log.ConsoleError("An exception occurred executing a command.");
					TShock.Log.Error(ex.ToString());
					return;
				}
			}
			if (!tsplayer.HasPermission(Permissions.canchat))
			{
				args.Handled = true;
				return;
			}
			if (tsplayer.mute)
			{
				tsplayer.SendErrorMessage("You are muted!");
				args.Handled = true;
				return;
			}

			string prefix = null, suffix = null;
			Color? color = null;

            var temp = TempGroupCache.Get(tsplayer);
            prefix = temp.prefix;
            suffix = temp.suffix;
            color = temp.chatColor;

			if (!TShock.Config.Settings.EnableChatAboveHeads)
			{
				text = string.Format(TShock.Config.Settings.ChatFormat, new object[]
				{
					tsplayer.Group.Name,
					string.IsNullOrEmpty(prefix) ? tsplayer.Group.Prefix : prefix,
					tsplayer.Name,
					string.IsNullOrEmpty(suffix) ? tsplayer.Group.Suffix : suffix,
					args.Text
				});
				bool flag = PlayerHooks.OnPlayerChat(tsplayer, args.Text, ref text);
				args.Handled = true;
				if (flag)
				{
					return;
				}
				if (color == null)
					TShock.Utils.Broadcast(text, tsplayer.Group.R, tsplayer.Group.G, tsplayer.Group.B);
				else
					TShock.Utils.Broadcast(text, color.Value.R, color.Value.G, color.Value.B);
			}
			else
			{
				Player player = Main.player[args.Who];
				string name = player.name;
				player.name = string.Format(TShock.Config.Settings.ChatAboveHeadsFormat, new object[]
				{
					tsplayer.Group.Name,
					string.IsNullOrEmpty(prefix) ? tsplayer.Group.Prefix : prefix,
					tsplayer.Name,
					string.IsNullOrEmpty(suffix) ? tsplayer.Group.Suffix : suffix,
				});
				NetMessage.SendData(4, -1, -1, NetworkText.FromLiteral(player.name), args.Who, 0f, 0f, 0f, 0, 0, 0);
				player.name = name;
				if (PlayerHooks.OnPlayerChat(tsplayer, args.Text, ref text))
				{
					args.Handled = true;
					return;
				}

				var packet = NetTextModule.SerializeServerMessage(NetworkText.FromLiteral(text), color ?? new Color((int)tsplayer.Group.R, (int)tsplayer.Group.G, (int)tsplayer.Group.B), (byte)args.Who);

				NetManager.Instance.Broadcast(packet, args.Who);
				NetMessage.SendData(4, -1, -1, NetworkText.FromLiteral(name), args.Who, 0f, 0f, 0f, 0, 0, 0);
				string text2 = string.Format("<{0}> {1}", string.Format(TShock.Config.Settings.ChatAboveHeadsFormat, new object[]
				{
					tsplayer.Group.Name,
					string.IsNullOrEmpty(prefix) ? tsplayer.Group.Prefix : prefix,
					tsplayer.Name,
					string.IsNullOrEmpty(suffix) ? tsplayer.Group.Suffix : suffix
				}), text);
				if (color == null)
				{
					tsplayer.SendMessage(text2, tsplayer.Group.R, tsplayer.Group.G, tsplayer.Group.B);
					TSPlayer.Server.SendMessage(text2, tsplayer.Group.R, tsplayer.Group.G, tsplayer.Group.B);
				}
				else
				{
					tsplayer.SendMessage(text2, color.Value.R, color.Value.G, color.Value.B);
					TSPlayer.Server.SendMessage(text2, color.Value.R, color.Value.G, color.Value.B);
				}
				TShock.Log.Info("Broadcast: {0}", new object[]
				{
					text2
				});
				args.Handled = true;
			}
		}
		private static void PlayerHooks_PlayerPermission(PlayerPermissionEventArgs args)
        {
            var perms = TempGroupCache.Get(args.Player).permissions;
			if (perms.Contains(args.Permission)) args.Result = PermissionHookResult.Granted;
			if (perms.Contains("!" + args.Permission)) args.Result = PermissionHookResult.Denied;
		}
	}


}
