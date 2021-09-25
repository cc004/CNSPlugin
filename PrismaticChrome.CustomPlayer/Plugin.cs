using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Net;
using Terraria.UI.Chat;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PrismaticChrome.CustomPlayer
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {

		private class ConfigFile
		{
			[Description("The server password required to join the server.")]
			public string ServerPassword = "";

			[Description("The port the server runs on.")]
			public int ServerPort = 7777;

			[Description("Maximum number of clients connected at once.\nIf you want people to be kicked with \"Server is full\" set this to how many players you want max and then set Terraria max players to 2 higher.")]
			public int MaxSlots = 8;

			[Description("The number of reserved slots past your max server slots that can be joined by reserved players.")]
			public int ReservedSlots = 20;

			[Description("Replaces the world name during a session if UseServerName is true.")]
			public string ServerName = "";

			[Description("Whether or not to use ServerName in place of the world name.")]
			public bool UseServerName;

			[Description("The path to the directory where logs should be written to.")]
			public string LogPath = "tshock";

			[Description("Whether or not the server should output debug level messages related to system operation.")]
			public bool DebugLogs = true;

			[Description("Prevents users from being able to login before they finish connecting.")]
			public bool DisableLoginBeforeJoin;

			[Description("Allows stacks in chests to go beyond the stack limit during world loading.")]
			public bool IgnoreChestStacksOnLoad;

			[Description("Enable or disable Terraria's built-in world auto save.")]
			public bool AutoSave = true;

			[Description("Enable or disable world save announcements.")]
			public bool AnnounceSave = true;

			[Description("Whether or not to show backup auto save messages.")]
			public bool ShowBackupAutosaveMessages = true;

			[Description("The interval between backups, in minutes. Backups are stored in the tshock/backups folder.")]
			public int BackupInterval;

			[Description("For how long backups are kept in minutes.\neg. 2880 = 2 days.")]
			public int BackupKeepFor = 60;

			[Description("Whether or not to save the world if the server crashes from an unhandled exception.")]
			public bool SaveWorldOnCrash = true;

			[Description("Whether or not to save the world when the last player disconnects.")]
			public bool SaveWorldOnLastPlayerExit = true;

			[Description("Determines the size of invasion events.\nThe equation for calculating invasion size is 100 + (multiplier * (number of active players with greater than 200 health)).")]
			public int InvasionMultiplier = 1;

			[Description("The default maximum number of mobs that will spawn per wave. Higher means more mobs in that wave.")]
			public int DefaultMaximumSpawns = 5;

			[Description("The delay between waves. Lower values lead to more mobs.")]
			public int DefaultSpawnRate = 600;

			[Description("Enables never ending invasion events. You still need to start the event, such as with the /invade command.")]
			public bool InfiniteInvasion;

			[Description("Sets the PvP mode. Valid types are: \"normal\", \"always\" and \"disabled\".")]
			public string PvPMode = "normal";

			[Description("Prevents tiles from being placed within SpawnProtectionRadius of the default spawn.")]
			public bool SpawnProtection = true;

			[Description("The tile radius around the spawn tile that is protected by the SpawnProtection setting.")]
			public int SpawnProtectionRadius = 10;

			[Description("Enable or disable anti-cheat range checks based on distance between the player and their block placements.")]
			public bool RangeChecks = true;

			[Description("Prevents non-hardcore players from connecting.")]
			public bool HardcoreOnly;

			[Description("Prevents softcore players from connecting.")]
			public bool MediumcoreOnly;

			[Description("Disables any placing, or removal of blocks.")]
			public bool DisableBuild;

			[Description("If enabled, hardmode will not be activated by the Wall of Flesh or the /starthardmode command.")]
			public bool DisableHardmode;

			[Description("Prevents the dungeon guardian from being spawned while sending players to their spawn point instead.")]
			public bool DisableDungeonGuardian;

			[Description("Disables clown bomb projectiles from spawning.")]
			public bool DisableClownBombs;

			[Description("Disables snow ball projectiles from spawning.")]
			public bool DisableSnowBalls;

			[Description("Disables tombstone dropping during death for all players.")]
			public bool DisableTombstones = true;

			[Description("Forces the world time to be normal, day, or night.")]
			public string ForceTime = "normal";

			[Description("Disables the effect of invisibility potions while PvP is enabled by turning the player visible to the other clients.")]
			public bool DisableInvisPvP;

			[Description("The maximum distance, in tiles, that disabled players can move from.")]
			public int MaxRangeForDisabled = 10;

			[Description("Whether or not region protection should apply to chests.")]
			public bool RegionProtectChests;

			[Description("Whether or not region protection should apply to gem locks.")]
			public bool RegionProtectGemLocks = true;

			[Description("Ignores checks to see if a player 'can' update a projectile.")]
			public bool IgnoreProjUpdate;

			[Description("Ignores checks to see if a player 'can' kill a projectile.")]
			public bool IgnoreProjKill;

			[Description("Allows players to break temporary tiles (grass, pots, etc) where they cannot usually build.")]
			public bool AllowCutTilesAndBreakables;

			[Description("Allows ice placement even where a user cannot usually build.")]
			public bool AllowIce;

			[Description("Allows the crimson to spread when a world is in hardmode.")]
			public bool AllowCrimsonCreep = true;

			[Description("Allows the corruption to spread when a world is in hardmode.")]
			public bool AllowCorruptionCreep = true;

			[Description("Allows the hallow to spread when a world is in hardmode.")]
			public bool AllowHallowCreep = true;

			[Description("How many NPCs a statue can spawn within 200 pixels(?) before it stops spawning.\nDefault = 3.")]
			public int StatueSpawn200 = 3;

			[Description("How many NPCs a statue can spawn within 600 pixels(?) before it stops spawning.\nDefault = 6.")]
			public int StatueSpawn600 = 6;

			[Description("How many NPCs a statue can spawn before it stops spawning.\nDefault = 10.")]
			public int StatueSpawnWorld = 10;

			[Description("Prevent banned items from being spawned or given with commands.")]
			public bool PreventBannedItemSpawn;

			[Description("Prevent players from interacting with the world while they are dead.")]
			public bool PreventDeadModification = true;

			[Description("Prevents players from placing tiles with an invalid style.")]
			public bool PreventInvalidPlaceStyle = true;

			[Description("Forces Christmas-only events to occur all year.")]
			public bool ForceXmas;

			[Description("Forces Halloween-only events to occur all year.")]
			public bool ForceHalloween;

			[Description("Allows groups on the banned item allowed list to spawn banned items even if PreventBannedItemSpawn is set to true.")]
			public bool AllowAllowedGroupsToSpawnBannedItems;

			[Description("The number of seconds a player must wait before being respawned. Cannot be longer than normal value now. Use at your own risk.")]
			public int RespawnSeconds = 5;

			[Description("The number of seconds a player must wait before being respawned if there is a boss nearby. Cannot be longer than normal value now. Use at your own risk.")]
			public int RespawnBossSeconds = 10;

			[Description("Whether or not to announce boss spawning or invasion starts.")]
			public bool AnonymousBossInvasions = true;

			[Description("The maximum HP a player can have, before equipment buffs.")]
			public int MaxHP = 500;

			[Description("The maximum MP a player can have, before equipment buffs.")]
			public int MaxMP = 200;

			[Description("Determines the range in tiles that a bomb can affect tiles from detonation point.")]
			public int BombExplosionRadius = 5;

			[Description("The default group name to place newly registered users under.")]
			public string DefaultRegistrationGroupName = "default";

			[Description("The default group name to place unregistered players under.")]
			public string DefaultGuestGroupName = "guest";

			[Description("Remembers where a player left off, based on their IP. Does not persist through server restarts.\neg. When you try to disconnect, and reconnect to be automatically placed at spawn, you'll be at your last location.")]
			public bool RememberLeavePos;

			[Description("Number of failed login attempts before kicking the player.")]
			public int MaximumLoginAttempts = 3;

			[Description("Whether or not to kick mediumcore players on death.")]
			public bool KickOnMediumcoreDeath;

			[Description("The reason given if kicking a mediumcore players on death.")]
			public string MediumcoreKickReason = "Death results in a kick";

			[Description("Whether or not to ban mediumcore players on death.")]
			public bool BanOnMediumcoreDeath;

			[Description("The reason given if banning a mediumcore player on death.")]
			public string MediumcoreBanReason = "Death results in a ban";

			[Description("Enable or disable the whitelist based on IP addresses in the whitelist.txt file.")]
			public bool EnableWhitelist;

			[Description("The reason given when kicking players for not being on the whitelist.")]
			public string WhitelistKickReason = "You are not on the whitelist.";

			[Description("The reason given when kicking players that attempt to join while the server is full.")]
			public string ServerFullReason = "Server is full";

			[Description("The reason given when kicking players that attempt to join while the server is full with no reserved slots available.")]
			public string ServerFullNoReservedReason = "Server is full. No reserved slots open.";

			[Description("Whether or not to kick hardcore players on death.")]
			public bool KickOnHardcoreDeath;

			[Description("The reason given when kicking hardcore players on death.")]
			public string HardcoreKickReason = "Death results in a kick";

			[Description("Whether or not to ban hardcore players on death.")]
			public bool BanOnHardcoreDeath;

			[Description("The reason given when banning hardcore players on death.")]
			public string HardcoreBanReason = "Death results in a ban";

			[Description("Enables kicking banned users by matching their IP Address.")]
			public bool EnableIPBans = true;

			[Description("Enables kicking banned users by matching their client UUID.")]
			public bool EnableUUIDBans = true;

			[Description("Enables kicking banned users by matching their Character Name.")]
			public bool EnableBanOnUsernames;

			[Description("If GeoIP is enabled, this will kick users identified as being under a proxy.")]
			public bool KickProxyUsers = true;

			[Description("Require all players to register or login before being allowed to play.")]
			public bool RequireLogin;

			[Description("Allows users to login to any account even if the username doesn't match their character name.")]
			public bool AllowLoginAnyUsername = true;

			[Description("Allows users to register a username that doesn't necessarily match their character name.")]
			public bool AllowRegisterAnyUsername;

			[Description("The minimum password length for new user accounts. Can never be lower than 4.")]
			public int MinimumPasswordLength = 4;

			[Description("The hash algorithm used to encrypt user passwords. Valid types: \"sha512\", \"sha256\" and \"md5\". Append with \"-xp\" for the xp supported algorithms.")]
			public string HashAlgorithm = "sha512";

			[Description("Determines the BCrypt work factor to use. If increased, all passwords will be upgraded to new work-factor on verify. The number of computational rounds is 2^n. Increase with caution. Range: 5-31.")]
			public int BCryptWorkFactor = 7;

			[Description("Prevents users from being able to login with their client UUID.")]
			public bool DisableUUIDLogin;

			[Description("Kick clients that don't send their UUID to the server.")]
			public bool KickEmptyUUID;

			[Description("Disables a player if this number of tiles is painted within 1 second.")]
			public int TilePaintThreshold = 15;

			[Description("Whether or not to kick users when they surpass the TilePaint threshold.")]
			public bool KickOnTilePaintThresholdBroken;

			[Description("The maximum damage a player/NPC can inflict.")]
			public int MaxDamage = 1175;

			[Description("The maximum damage a projectile can inflict.")]
			public int MaxProjDamage = 1175;

			[Description("Whether or not to kick users when they surpass the MaxDamage threshold.")]
			public bool KickOnDamageThresholdBroken;

			[Description("Disables a player and reverts their actions if this number of tile kills is exceeded within 1 second.")]
			public int TileKillThreshold = 60;

			[Description("Whether or not to kick users when they surpass the TileKill threshold.")]
			public bool KickOnTileKillThresholdBroken;

			[Description("Disables a player and reverts their actions if this number of tile places is exceeded within 1 second.")]
			public int TilePlaceThreshold = 32;

			[Description("Whether or not to kick users when they surpass the TilePlace threshold.")]
			public bool KickOnTilePlaceThresholdBroken;

			[Description("Disables a player if this number of liquid sets is exceeded within 1 second.")]
			public int TileLiquidThreshold = 50;

			[Description("Whether or not to kick users when they surpass the TileLiquid threshold.")]
			public bool KickOnTileLiquidThresholdBroken;

			[Description("Whether or not to ignore shrapnel from crystal bullets for the projectile threshold count.")]
			public bool ProjIgnoreShrapnel = true;

			[Description("Disable a player if this number of projectiles is created within 1 second.")]
			public int ProjectileThreshold = 50;

			[Description("Whether or not to kick users when they surpass the Projectile threshold.")]
			public bool KickOnProjectileThresholdBroken;

			[Description("Disables a player if this number of HealOtherPlayer packets is sent within 1 second.")]
			public int HealOtherThreshold = 50;

			[Description("Whether or not to kick users when they surpass the HealOther threshold.")]
			public bool KickOnHealOtherThresholdBroken;

			[Description("Specifies which string starts a command.\nNote: Will not function properly if the string length is bigger than 1.")]
			public string CommandSpecifier = "/";

			[Description("Specifies which string starts a command silently.\nNote: Will not function properly if the string length is bigger than 1.")]
			public string CommandSilentSpecifier = ".";

			[Description("Disables sending logs as messages to players with the log permission.")]
			public bool DisableSpewLogs = true;

			[Description("Prevents OnSecondUpdate checks from writing to the log file.")]
			public bool DisableSecondUpdateLogs;

			[Description("The chat color for the superadmin group.\n#.#.# = Red/Blue/Green\nMax value: 255")]
			public int[] SuperAdminChatRGB = new int[3]
			{
		255,
		255,
		255
			};

			[Description("The superadmin chat prefix.")]
			public string SuperAdminChatPrefix = "(Super Admin) ";

			[Description("The superadmin chat suffix.")]
			public string SuperAdminChatSuffix = "";

			[Description("Whether or not to announce a player's geographic location on join, based on their IP.")]
			public bool EnableGeoIP;

			[Description("Displays a player's IP on join to users with the log permission.")]
			public bool DisplayIPToAdmins;

			[Description("Changes in-game chat format: {0} = Group Name, {1} = Group Prefix, {2} = Player Name, {3} = Group Suffix, {4} = Chat Message.")]
			public string ChatFormat = "{1}{2}{3}: {4}";

			[Description("Changes the player name when using chat above heads. Starts with a player name wrapped in brackets, as per Terraria's formatting.\nSame formatting as ChatFormat without the message.")]
			public string ChatAboveHeadsFormat = "{2}";

			[Description("Whether or not to display chat messages above players' heads.")]
			public bool EnableChatAboveHeads;

			[Description("The RGB values used for the color of broadcast messages.\n#.#.# = Red/Blue/Green\nMax value: 255")]
			public int[] BroadcastRGB = new int[3]
			{
		127,
		255,
		212
			};

			[Description("The type of database to use when storing data (either \"sqlite\" or \"mysql\").")]
			public string StorageType = "sqlite";

			[Description("The path of sqlite db.")]
			public string SqliteDBPath = "tshock.sqlite";

			[Description("The MySQL hostname and port to direct connections to.")]
			public string MySqlHost = "localhost:3306";

			[Description("The database name to connect to when using MySQL as the database type.")]
			public string MySqlDbName = "";

			[Description("The username used when connecting to a MySQL database.")]
			public string MySqlUsername = "";

			[Description("The password used when connecting to a MySQL database.")]
			public string MySqlPassword = "";

			[Description("Whether or not to save logs to the SQL database instead of a text file.\nDefault = false.")]
			public bool UseSqlLogs;

			[Description("Number of times the SQL log must fail to insert logs before falling back to the text log.")]
			public int RevertToTextLogsOnSqlFailures = 10;

			[Description("Enable or disable the REST API.")]
			public bool RestApiEnabled;

			[Description("The port used by the REST API.")]
			public int RestApiPort = 7878;

			[Description("Whether or not to log REST API connections.")]
			public bool LogRest;

			[Description("Whether or not to require token authentication to use the public REST API endpoints.")]
			public bool EnableTokenEndpointAuthentication;

			[Description("The maximum REST requests in the bucket before denying requests. Minimum value is 5.")]
			public int RESTMaximumRequestsPerInterval = 5;

			[Description("How often in minutes the REST requests bucket is decreased by one. Minimum value is 1 minute.")]
			public int RESTRequestBucketDecreaseIntervalMinutes = 1;
			
		}

		public override string Name => "PrismaticChrome.CustomPlayer";

        public Plugin(Main game) : base(game)
        {
        }

        private static Dictionary<string, string[]> permCache = new Dictionary<string, string[]>();
		
        private static string[] GetPermissionWithCache(string account)
        {
            if (string.IsNullOrEmpty(account)) return new string[0];
            if (!permCache.ContainsKey(account))
			{
                using (var query = Db.Get<Customized>(account))
                {
                    permCache[account] = query.Single().permission?.Split('\n') ?? new string[0];
                }
            }

            return permCache[account];
        }

        internal static void ClearCache()
        {
            permCache.Clear();
        }

        public override void Initialize()
        {
            PlayerHooks.PlayerPermission += PlayerHooks_PlayerPermission;
            var handlers = ServerApi.Hooks.ServerChat;
            var registrations = (IEnumerable) handlers.GetType()
                .GetField("registrations", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(handlers);
            var registratorfield = registrations.GetType().GenericTypeArguments[0]
                .GetProperty("Registrator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var handlerfield = registrations.GetType().GenericTypeArguments[0]
                .GetProperty("Handler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var registration in registrations)
            {
                var handler = (HookHandler<ServerChatEventArgs>) handlerfield.GetValue(registration);
                var plugin = (TerrariaPlugin) registratorfield.GetValue(registration);
                if (plugin is TShock)
                {
                    TShock.Log.ConsoleInfo("TShock server chat handled forced to de-register");
                    ServerApi.Hooks.ServerChat.Deregister(plugin, handler);
                    ServerApi.Hooks.ServerChat.Register(plugin, OnChat);
				}
            }
			CommandHelper.Register<Commands>("custom");
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

            string prefix, suffix;
            Color? color;

            using (var query = tsplayer.Get<Customized>())
            {
                var data = query.Single();
                prefix = data.prefix;
                suffix = data.suffix;
                color = string.IsNullOrEmpty(data.color) ? null : new Color?(new Color(uint.Parse(data.color, NumberStyles.HexNumber)));
            }

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
            var perms = GetPermissionWithCache(args.Player?.Account?.Name);
            if (perms.Contains(args.Permission)) args.Result = PermissionHookResult.Granted;
            if (perms.Contains("!" + args.Permission)) args.Result = PermissionHookResult.Denied;
        }
    }
}
