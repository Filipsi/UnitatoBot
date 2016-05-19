
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Configuration {

    public static partial class Config {

        public static partial class Crypto {

            public static string Encrypt(string inText, string key) {
                byte[] bytesBuff = Encoding.Unicode.GetBytes(inText);

                using(Aes aes = Aes.Create()) {

                    Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    aes.Key = crypto.GetBytes(32);
                    aes.IV = crypto.GetBytes(16);

                    using(MemoryStream mStream = new MemoryStream()) {

                        using(CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                            cStream.Write(bytesBuff, 0, bytesBuff.Length);
                            cStream.Close();
                        }

                        inText = Convert.ToBase64String(mStream.ToArray());
                    }

                }

                return inText;
            }

            public static string Decrypt(string cryptTxt, string key) {
                cryptTxt = cryptTxt.Replace(" ", "+");
                byte[] bytesBuff = Convert.FromBase64String(cryptTxt);

                using(Aes aes = Aes.Create()) {

                    Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    aes.Key = crypto.GetBytes(32);
                    aes.IV = crypto.GetBytes(16);

                    using(MemoryStream mStream = new MemoryStream()) {

                        using(CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                            cStream.Write(bytesBuff, 0, bytesBuff.Length);
                            cStream.Close();
                        }

                        cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());

                    }

                }

                return cryptTxt;
            }

        }

    }

}
