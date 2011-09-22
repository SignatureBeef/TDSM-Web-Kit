using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebKit.Server.Auth
{
    public enum AuthStatus : int
    {
        MATCH = 0,
        NON_EXISTANT_USER = 1,
        NON_EXISTANT_PASS = 2
    }
}
