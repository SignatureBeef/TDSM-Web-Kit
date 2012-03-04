using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server;
using Terraria_Server.Commands;
using WebKit.Server.Auth;
using WebKit.Server.Utility;

namespace WebKit
{
	public abstract class BaseWebKit : BasePlugin
	{
		public void Server(ISender sender, ArgumentList args)
		{
			var webKit = (WebKit)args.Plugin;
			if (webKit == null)
				throw new CommandError("Unable to get WebKit instance.");

			var restart = args.TryPop("restart");
			var stop	= args.TryPop("stop");
			var start	= args.TryPop("start");

			if (restart)
			{
				WebKit.Log("Stopping Http server...");
				webKit.WebServer.StopServer();
				WebKit.Log("Starting Http server...");
				webKit.WebServer.StartServer();
				WebKit.Log("Http server started.");
			}
			else if (stop)
			{
				WebKit.Log("Stopping Http server...");
				webKit.WebServer.StopServer();
				WebKit.Log("Http server stopped.");
			}
			else if (start)
			{
				WebKit.Log("Starting Http server...");
				webKit.WebServer.StartServer();
				WebKit.Log("Http server started.");

				//Wow...that was a lot of http.
			}
			else
				throw new CommandError("Unknown http option.");
		}

		public void Connection(ISender sender, ArgumentList args)
		{
			var webKit = args.Plugin as WebKit;
			var view = args.TryPop("view");
			var kick = args.TryPop("kick");

			if (kick && sender is WebSender)
			{
				sender.sendMessage("Unauthorized access.");
				return;
			}

			int id = webKit.WebSessions.Count;
			if (kick && (!args.TryParseOne<Int32>(out id) || id >= webKit.WebSessions.Count))
				throw new CommandError("Please enter a valid slot number");

			if (view)
			{
				var max = webKit.WebSessions.Count;
				if (max == 0)
				{
					sender.sendMessage("No connections.");
					return;
				}

				for (var i = 0; i < max; i++)
				{
					var pair = webKit.WebSessions.ElementAt(i);
					var name = pair.Key;
					var val = pair.Value;
					if (!Authentication.IsOutOfSession(name, val.LastUpdate, val.IpAddress, webKit))
					{
						sender.sendMessage(
							String.Format("[{0}] - {1}@{2}", i, name, val.IpAddress)
						);
					}
				}
			}
			else if (kick)
			{
				var max = webKit.WebSessions.Count;
				if (max == 0)
				{
					sender.sendMessage("No connection to kick.");
					return;
				}

				for (var i = 0; i < max; i++)
				{
					if (i == id)
					{
						var pair = webKit.WebSessions.ElementAt(i);
						var name = pair.Key;
						var val = pair.Value;
						if (!Authentication.IsOutOfSession(name, val.LastUpdate, val.IpAddress, webKit))
						{
							webKit.KickList.Add(pair);

							sender.sendMessage(
								String.Format("[{0}] - {1}@{2} added to kick queue.", i, name, val.IpAddress)
							);
							return;
						}
					}
				}

				sender.sendMessage("Failed to add the seleted identity at slot `" + id + "` to the kick queue.");
			}
			else
				throw new CommandError(String.Empty);
		}

		public void Users(ISender sender, ArgumentList args)
		{
			if (sender is WebSender)
			{
				sender.sendMessage("Unauthorized access.");
				return;
			}

			var reloadConfig = args.TryPop("reloadconfig");
			var add = args.TryPop("add");
			var remove = args.TryPop("remove");
			var webKit = args.Plugin as WebKit;

			if (reloadConfig)
			{
				sender.sendMessage("Reloading WebKit credential list...");
				webKit.CredentialList = Authentication.GetCredentials();
				sender.sendMessage("Complete.");
			}
			else if (add)
			{
				string user, pass;
				if (args.TryParseTwo<String, String>(out user, out pass))
				{
					var hashed = Authentication.ComputeHash(user, pass, webKit.Properties.ServerId);
					if (Authentication.AddUserCredential(user, hashed))
						sender.sendMessage("User `" + user + "` successfully added.");
					else
						sender.sendMessage("Failed to add `" + user + "` to the list.", 255, 355, 0, 0);
				}
				else
					throw new CommandError("Please specify a username and password.");
			}
			else if (remove)
			{
				string user;
				if (args.TryParseOne<String>(out user))
				{
					if (Authentication.CleanOutUser(user))
						sender.sendMessage("User `" + user + "` has been removed.");
					else
						sender.sendMessage("Failed to remove `" + user + "`.", 255, 355, 0, 0);
				}
				else
					throw new CommandError("Please specify a username.");
			}
			else
				throw new CommandError(String.Empty);
		}
	}
}
