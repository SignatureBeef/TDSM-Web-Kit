// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria_Server.Logging;
using System.Security.Cryptography;

namespace WebKit.Server.Auth
{
    public static class Authentication
    {
        public const string CREDENTIAL_LIST = "credentials.txt";

        public static string CredentialPath
        {
            get
            {
                return WebKit.PluginPath + Path.DirectorySeparatorChar + CREDENTIAL_LIST;
            }
        }

        public static void Init(string serverId)
        {
			var path = CredentialPath;
			if (!File.Exists(path))
            {
				string user = "admin", pass = ComputeHash(user, "admin", serverId);

                try
                {
					File.WriteAllLines(path,
						new string[]
						{
							"#Format: username:sha1hash",
							"#Lines starting with '#' will not be read",
							String.Empty, String.Empty,
							user + ":" + pass
						}
					);
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e);
                }
            }
        }

        public static List<Credential> GetCredentials()
        {
            List<Credential> credentials = new List<Credential>();
            int lineCount = 0;
            foreach (string line in File.ReadAllLines(CredentialPath))
            {
                string cleanedLine = line.Trim();
                if (!cleanedLine.StartsWith("#") && cleanedLine.Contains(":"))
                {
                    string[] parts = cleanedLine.Split(':');
                    if (parts.Length > 1)
                    {
                        string user = parts[0].Trim(), pass = parts[1].Trim();
                        if (user.Length > 0 && pass.Length > 0)
                        {
                            Credential credential = new Credential(user, pass);
                            credentials.Add(credential);
                        }
                        else
                        {
                            WebKit.Log("Incorrect data in Auth list, Line {0}", lineCount);
                        }
                    }
                }
                lineCount++;
            }
            return credentials;
        }

		public static bool AddUserCredential(string user, string hash, string serverId = "")
		{
			try
			{
				var lines = new string[] { user + ":" + hash };
				var path = CredentialPath;

				if (File.Exists(path))
					Init(serverId);
				else
					File.WriteAllLines(path, lines);

				return true;
			}
			catch { }

			return false;
		}

		public static bool CleanOutUser(string user)
		{
			try
			{
				var lines = File.ReadAllLines(CredentialPath);
				using (var ctx = File.OpenWrite(CredentialPath))
				{
					var find = user.ToLower();
					foreach (var line in lines.Where(x => x.Contains(';')))
					{
						var userName = line.Split(':').ElementAt(0).ToLower();
						if (find != userName)
						{
							var bytes = Encoding.ASCII.GetBytes(line);
							ctx.Write(bytes, 0, bytes.Length);
						}
					}
				}
			}
			catch { }

			return false;
		}

        public static string HashString(string hashee)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(hashee));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
				builder.Append(hash[i].ToString("X2"));

            return builder.ToString();
        }

		public static string ComputeHash(string user, string password, string serverId)
		{
			return HashString(user + serverId + password);
		}

		public static bool CompareHash(string input, string hashed)
		{
			return input.ToUpper().Equals(hashed.ToUpper());
		}

        public static string GetUserHash(string user, WebKit webKit)
        {
            foreach (Credential cred in webKit.CredentialList.Where(x => x.Username == user))
				return cred.Sha1;

            return null;
        }

        public static AuthStatus IsCredentialsTheSame(string user, string password, WebKit webKit)
        {
            string userHash = GetUserHash(user, webKit);

			if (userHash == null)
				return AuthStatus.NON_EXISTANT_USER;
			else
			{
				var hashed = ComputeHash(user, password, webKit.Properties.ServerId);

				if (CompareHash(hashed, userHash))
					return AuthStatus.MATCH;
			}

            return AuthStatus.NON_EXISTANT_PASS;
        }
		
		public static bool IsOutOfSession(string name, DateTime last, string ipAddress, WebKit webKit)
		{
			return (DateTime.Now - last).TotalMilliseconds > (webKit.MainUpdateInterval * 2);
		}
    }
}
