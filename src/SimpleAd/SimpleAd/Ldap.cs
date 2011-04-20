#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System.DirectoryServices;

namespace SimpleAD
{
    public class Ldap
    {
        private const int _DEFAULT_LDAP_PORT = 0x185;

        public static string EncodeFilter(string value) { return EncodeFilter(value, true); }
        public static string EncodeFilter(string value, bool encodeWildcard)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            value = value.Replace(@"\", @"\5c").Replace("/", @"\2f").Replace("(", @"\28").Replace(")", @"\29").Replace("\0", @"\00");
            return (encodeWildcard ? value.Replace("*", @"\2a") : value);
        }

        public static string EncodeDistinguishedName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace("!", @"\21").Replace("*", @"\2A").Replace("/", @"\2F").Replace(":", @"\3A").Replace("?", @"\3F");
        }

        //public static DirectoryEntry GetDirectoryEntry(string server, int port, string pathDN, bool useSsl, string userId, string password)
        //{
        //    return new DirectoryEntry(GetUrl(server, port, pathDN), userId, password, GetAuthenticationTypes(useSsl));
        //}
        //public static string GetUrl(string server, int port, string pathDN)
        //{
        //    // This item is obfuscated and can not be translated.
        //    //if ((server != null) && (port != 0x185)) { }
        //    string str = server.Trim() + ":" + port.ToString() + "/";
        //    return ("LDAP://" + str + EncodeDistinguishedName(pathDN));
        //}

        public static AuthenticationTypes GetAuthenticationTypes(bool useSsl)
        {
            var types = AuthenticationTypes.Secure | AuthenticationTypes.ServerBind | AuthenticationTypes.FastBind | AuthenticationTypes.ReadonlyServer;
            return (!useSsl ? types : types | AuthenticationTypes.Encryption);
        }
    }
}
