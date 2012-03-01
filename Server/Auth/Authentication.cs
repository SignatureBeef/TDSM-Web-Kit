using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria_Server.Logging;
using System.Security.Cryptography;

namespace WebKit.Server.Auth
{
    public class Authentication
    {
        public const string CREDENTIAL_LIST = "credentials.txt";

        public static string CredentialPath
        {
            get
            {
                return WebKit.PluginPath + Path.DirectorySeparatorChar + CREDENTIAL_LIST;
            }
        }

        public static void Init()
        {
            if (!File.Exists(CredentialPath))
            {
                string user = "admin", pass = HashString(user);
                string Header = "#Format: username:sha1hash\n#Lines starting with '#' will not be read\n\n" +
                    user + ":" + pass;

                try
                {
                    File.WriteAllText(CredentialPath, Header);
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
            int Line = 0;
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
                            WebKit.Log("Incorrect data in Auth list, Line {0}", Line);
                        }
                    }
                }
                Line++;
            }
            return credentials;
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

		public static bool CompareHash(string input, string Hashed)
		{
			//string hash = HashString(toHash);
			return input.ToUpper().Equals(Hashed.ToUpper());
		}

        public static string GetUserHash(string User, WebKit WebKit)
        {
            foreach (Credential cred in WebKit.CredentialList.Where(X => X.Username == User))
            {
                return cred.Sha1;
            }
            return null;
        }

        public static AuthStatus IsCredentialsTheSame(string User, string password, WebKit WebKit)
        {
            string userHash = GetUserHash(User, WebKit);

			if (userHash == null)
				return AuthStatus.NON_EXISTANT_USER;
			else
			{
				var hashed = ComputeHash(User, password, WebKit.Properties.ServerId);

				if (CompareHash(hashed, userHash))
					return AuthStatus.MATCH;
			}

            return AuthStatus.NON_EXISTANT_PASS;
        }

        public static bool InSession(string IPaddress, WebKit WebKit)
        {
            Dictionary<String, DateTime> sessions = WebKit.WebSessions;
            for (int i = 0; i < sessions.Keys.Count; i++)
            {
                if (sessions.Keys.ToArray()[i].Equals(IPaddress))
                {
                    if (sessions.Values.ToArray()[i].ToBinary() >= DateTime.Now.ToBinary())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
