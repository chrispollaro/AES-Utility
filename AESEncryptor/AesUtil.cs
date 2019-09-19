using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace AESEncryptor
{
    [ComVisible(true)]
    [ProgId("AESEncryptor.dll")]
    [Guid("6007B2C2-94A3-464b-8C34-B6644BC88382")]
    public class AesUtil
    {
        // For testing
        public AesUtil()
        {
            Salt = "MinimumOfEightBytes";
            Password = "PO345kdfJXCSJD#wflxj23jweijrwi1Y212ijd";
        }

        public AesUtil(string password, string salt)
        {
            if (password != null)
            {
                if (salt != null && salt.Length < 8)
                {
                    Salt = salt + "addeight";
                }
                else
                {
                    Salt = salt;
                }

                Password = password;
            }
            else
            {
                throw new ArgumentNullException(password);
            }
        }

        public string Decrypt(string encryptedText)
        {
            using (var s = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {
                    Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Password,
                        Encoding.UTF8.GetBytes(Salt));
                    aes.Key = deriveBytes.GetBytes(128 / 8);
                    // Get the initialization vector from the encrypted stream
                    aes.IV = ReadByteArray(s);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    CryptoStream cs = new CryptoStream(s, aes.CreateDecryptor(), CryptoStreamMode.Read);
                    StreamReader reader = new StreamReader(cs, Encoding.Unicode);
                    try
                    {
                        string retval = reader.ReadToEnd();
                        reader.Dispose();
                        cs.Dispose();
                        return retval;
                    }
                    catch (Exception e)
                    {
                        return e.ToString();
                    }
                }
            }
        }

        public string Encrypt(string plainText)
        {
            using (var outputStream = new MemoryStream())
            {
                using (Aes aes = new AesCryptoServiceProvider())
                {
                    Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Password, Encoding.UTF8.GetBytes(Salt));
                    aes.Key = deriveBytes.GetBytes(128 / 8);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    outputStream.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof(int));
                    outputStream.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(outputStream, aes.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        byte[] rawPlaintext = Encoding.Unicode.GetBytes(plainText);
                        cs.Write(rawPlaintext, 0, rawPlaintext.Length);
                        cs.FlushFinalBlock();
                    }
                }

                return Convert.ToBase64String(outputStream.ToArray());
            }
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        private string Salt { get; }

        private string Password { get; }
    }
}
