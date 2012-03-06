// Project:      TDSM WebKit
// Contributors: DeathCradle
// 
using System;

namespace WebKit.Server.Auth
{
    public enum AuthStatus : int
    {
        MATCH = 0,
        NON_EXISTANT_USER = 1,
        NON_EXISTANT_PASS = 2
    }
}
