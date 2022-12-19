using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;

namespace jgrunas
{
    class Program
    {
        private static TextWriter _O = Console.Out;
        private static TextWriter _E = Console.Error;
        private static string G = @"Usage:
 $>jgrun [exePath] [options] [domain] [username] [password] [encKey]

Parameters:
  exePath  - complete path of the file to be executed
  options  - commandline arguments
  domain   - user domain
  username - user name or id
  password - user password. if encrypted, specify the generated encKey
  encKey   - base64-string encryption key used to encrypt the password            
";
        static int Main(string[] aR)
        {
            if (aR.Length >= 5) return RP(aR);
                
            if(aR.Length==2)
            {
                if (aR[1].Length == 32) return XP(0, EP(aR[1], aR[0]) + " " + Convert.ToBase64String(Encoding.UTF8.GetBytes(XR(aR[1], AG()))));
                return XP(-1, "Key length must be 32chars.");
            }

            if (aR.Length == 1) {
                if(aR[0].Equals("?")) return XP(0, G);
                if (aR[0].Equals("??")) return XP(0, "jgrun password enckey(32chars)");
            }
            return XP(-2,"Invalid parameter count!");
        }

        private static int RP(string[] a)
        {
            try
            {
                string v_NOWINDOW = Environment.GetEnvironmentVariable("JG_NOWINDOW");
                string v_NOWAIT = Environment.GetEnvironmentVariable("JG_NOWAIT");
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo n = p.StartInfo;
                System.Security.SecureString s = new System.Security.SecureString();
                n.UseShellExecute = false;
                n.FileName = a[0];
                n.Arguments = a[1];
                n.Domain = a[2];
                n.UserName = a[3];
                string w = a.Length == 6 ? DP(XR(Encoding.UTF8.GetString(Convert.FromBase64String(a[5])), AG()), a[4]) : a[4];
                foreach (char c in w) s.AppendChar(c);
                w = "";
                n.Password = s;
                if(v_NOWINDOW != null) n.CreateNoWindow = true;
                p.Start();
                if (v_NOWAIT != null) return 0;
                p.WaitForExit();
                return p.ExitCode;
            }
            catch (Exception e)
            {
                return XP(-1,e.InnerException == null ? e.Message : e.InnerException.Message);
            }
        }
        private static int XP(int i, string t)
        {
            if (i == 0) _O.WriteLine(t); else _E.WriteLine(t);
            return i;
        }

        private static string EP(string k, string t)
        {
            byte[] iv = new byte[16];
            byte[] ar;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(k);
                aes.IV = iv;
                ICryptoTransform aC = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream mS = new MemoryStream())
                {
                    using (CryptoStream cS = new CryptoStream((Stream)mS, aC, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sW = new StreamWriter((Stream)cS))
                        {
                            sW.Write(t);
                        }
                        ar = mS.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(ar);
        }
        private static string DP(string k, string t)
        {
            byte[] iv = new byte[16];
            byte[] et = Convert.FromBase64String(t);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(k);
                aes.IV = iv;
                ICryptoTransform dC = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream mS = new MemoryStream(et))
                {
                    using (CryptoStream cS = new CryptoStream((Stream)mS, dC, CryptoStreamMode.Read))
                    {
                        using (StreamReader sR = new StreamReader((Stream)cS))
                        {
                            return sR.ReadToEnd();
                        }
                    }
                }
            }
        }
        private static string XR(string k, string g)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < g.Length; i++)
                sb.Append((char)(g[i] ^ k[(i % k.Length)]));
            return sb.ToString();
        }
        private static string AG()
        {
            var aG = (GuidAttribute)(typeof(Program).Assembly).GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return aG.Value.Replace("-", "");
        }

    }
}
