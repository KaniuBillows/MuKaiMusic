using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
// ReSharper disable PossibleNullReferenceException

namespace Encrypt
{
    public class AesEncrypt
    {
        public static AesEncrypt CreateEncrypt()
        {
            return new AesEncrypt();
        }
        public byte[] Encrypt(string data, byte[] key, byte[] vector, CipherMode mode)
        {
            if (key is null)
            {
                throw new System.ArgumentNullException(nameof(key));
            }
            if (vector is null)
            {
                throw new System.ArgumentNullException(nameof(vector));
            }
            if (data is null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }
            byte[] encrypted;

            try
            {
                byte[] toEncrypt = Encoding.UTF8.GetBytes(data);
                var rm = new RijndaelManaged
                {
                    Key = key,
                    Mode = mode,
                };
                if (vector.Length > 0)
                {
                    rm.IV = vector;
                }
                ICryptoTransform cTransform = rm.CreateEncryptor();
                encrypted = cTransform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);
            }
            catch (Exception e)
            {
                var me = e.Message;
                encrypted = null;
            }

            return encrypted;
        }
    }
}
