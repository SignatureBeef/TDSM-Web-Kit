using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.Auth
{
    public struct Credential
    {
        public string Username;
        public string Sha1;

        public Credential(string User, string Sha1Hash)
        {
            Username = User;
            Sha1 = Sha1Hash;
        }
    }
}
