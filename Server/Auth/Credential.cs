// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;

namespace WebKit.Server.Auth
{
    public struct Credential
    {
        public string Username;
        public string Sha1;

		public Credential(string user, string sha1Hash)
		{
			Username = user;
			Sha1 = sha1Hash;
		}
    }
}
