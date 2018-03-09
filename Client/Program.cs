using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Console.Write("Nhap key: ");
                string key = Console.ReadLine();
                IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 6789);
                EndPoint ep = ip;
                string sgui = "Xin chào Server!";
                string sEncrypt = Encrypt(sgui, key);
                byte[] gui = Encoding.UTF8.GetBytes(sEncrypt);
                client.SendTo(gui, ep);
                byte[] nhan = new byte[1024];
                int rec = client.ReceiveFrom(nhan, ref ep);
                string s = Encoding.UTF8.GetString(nhan, 0, rec);
                Console.WriteLine("Server gui nguyen mau: " + s);
                Console.WriteLine();
                string sDecrypt = Decrypt(s, key);
                Console.WriteLine("Sau khi duoc ma hoa: " + sDecrypt);
                client.Close();
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
