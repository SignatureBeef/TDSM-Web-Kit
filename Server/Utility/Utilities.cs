// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Threading;
using Terraria_Server;
using Terraria_Server.Language;
using Terraria_Server.Logging;
using Terraria_Server.WorldMod;

namespace WebKit.Server.Utility
{
    public static class Utilities
    {
        public static bool RestartServer(WebKit webKit, string ipOrName)
        {
            /* snip, I 'could' trigger the command, but it requires sender and what not,
             * though the IP should be logged, Fake sender name maybe? 
             * I say it should be logged, thinking in a Server OP sense, that if they dont want 
             * certain OP's to, they can check and whatever.
             */
            try
            {
				webKit.ServerStatus = "Restarting";
				Program.Restarting = true;
				Terraria_Server.Server.notifyOps(Languages.RestartingServer + " [" + ipOrName + "]", true);

				NetPlay.StopServer();
				while (NetPlay.ServerUp) { Thread.Sleep(10); }

				ProgramLog.Log(Languages.StartingServer);
				Main.Initialize();

				Program.LoadPlugins();

				WorldIO.LoadWorld(null, null, World.SavePath);
				Program.updateThread = new ProgramThread("Updt", Program.UpdateLoop);
				NetPlay.StartServer();

				while (!NetPlay.ServerUp) { Thread.Sleep(100); }

				ProgramLog.Console.Print(Languages.Startup_YouCanNowInsertCommands);
				Program.Restarting = false;

                return true;
            }
            catch(Exception e)
            {
                ProgramLog.Log(e);
            }

            return false;
        }

        public static bool StopServer(WebKit webKit, string ipPOrName)
        {
            try
            {
				webKit.ServerStatus = "Exiting";

				Terraria_Server.Server.notifyOps("Exiting on request. [" + ipPOrName + "]", false);
                NetPlay.StopServer();
                Statics.Exit = true;

                return true;
            }
            catch (Exception e)
            {
                ProgramLog.Log(e);
            }

            return false;
        }
    }
}
