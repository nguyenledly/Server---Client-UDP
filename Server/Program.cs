using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Console.Write("Nhap key: ");
                string key = Console.ReadLine();
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, 6789);
                server.Bind(ip);
                EndPoint ep = ip;
                byte[] nhan = new byte[1024];
                int rec = server.ReceiveFrom(nhan, ref ep);
                string s = Encoding.UTF8.GetString(nhan, 0, rec);
                Console.WriteLine("Client gui ban nguyen mau: " + s);
                Console.WriteLine();
                string sDecrypt = Decrypt(s, key);
                Console.WriteLine("Sau khi ma hoa: " + sDecrypt);
                string sgui = "Xin chào Client!";
                string sEncrypt = Encrypt(sgui, key);
                byte[] gui = Encoding.UTF8.GetBytes(sEncrypt);
                server.SendTo(gui, ep);
                server.Close();
            }
            catch
            {
                Console.WriteLine("Nhap sai key!");
            }
        }
        public static string Encrypt(string source, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Encoding.UTF8.GetBytes(source);

            string encoded =
                Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }
        public static string Decrypt(string encodedText, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Convert.FromBase64String(encodedText);

            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }
    }
}
