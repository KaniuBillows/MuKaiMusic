using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test
{

    class Program
    {
        const string Netease_base62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        static async Task Main(string[] args)
        {
            AesCrypt();
            // AESTest();
        }
        private static RSACryptoServiceProvider DecodeRSAPrivateKey(string privateKey)
        {
            var privateKeyBits = Convert.FromBase64String(privateKey);

            var RSA = new RSACryptoServiceProvider();
            var RSAparams = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            RSA.ImportParameters(RSAparams);
            return RSA;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private static void AESDecrypt()
        {
            var toDecrypt = "C+oZvsOvHwiIyaBZHcVc98YZO+7EOcn1vkgSZfAUqqT7AuFB2PgPjCCBGZ3rcCbzrNieFZ3zYYFPHu9hroR/Ww==";
            var array = Convert.FromBase64String(toDecrypt);
            using var aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            aes.Key = Convert.FromBase64String("NU90YjNiUDdDWFhGdHNwdg==");
            aes.Padding = PaddingMode.PKCS7;
            using var transform = aes.CreateDecryptor();
            var plainBytes = transform.TransformFinalBlock(array, 0, array.Length);
            Console.WriteLine(Encoding.UTF8.GetString(plainBytes));
        }

        private static void RsaTest()
        {
            using TextReader reader = new StringReader("-----BEGIN PRIVATE KEY-----\nMIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALW0tb72Unv+Mew1Cddxuup9yJBtzlh5Ih/ur3Dlob+njzzH0AL3gbREe2hEjPyb4ZSU7WZee07U4W+GuLzKCig94Z9Wc7rYVO5yRj7ZZIdHc3bRAxaMIlyR1m01OjSMriflAtbjKj2a7iv8gi9PFy65s/Ool5ZB1bzXcROke+JfAgMBAAECgYAQf+NM/tMInVFtMHe/T314k4llzALDt+QoYCUgLEnevFleXIp/6nelpd5K3oaq3YDDXC6ecA/kXaJG3UXjJrl7PMlHEicvx7C1N8jvhqwkQZj2Hb4x/Ku3elkVDfKBv6a/lnGAL4PiIs5cDXEFFuAIHM+wSa5JJke8Igb2pRkI6QJBAN+w33cOe+oODfCB1FQKBLLr6ZdIbF8o+eWY1gditamGkvI1spEybkcTRyMmwpRVYiPCe2qD+kAAspL0h3au290CQQDP82l5WIPv2PhRUoi8hFOdo9Oonbmq/I/3zJCjmdIpz9CbK38LX16BN7uadjO8NuUG1Lq7fgsOI+rU5MtToKFrAkBSbOVbBH5Kl5UxW1W3Bd4HZAC2Gxy7z+iNXsz2+buA1TLOh1TeRdv22raq0DyA1hmMnbxGWo1bCibDGdHUvMgNAkEAqzKXyODIbfNQK5MpIxpFxGgVXqHLMyuOUtFE2otObEG8WaLriq9PHE6H0lgxq2f+ESudN7JC47dKI07g3c5GVQJBAMq5gqx0lZ7b+AQbzs7U4urwKGCIPuyali93I32JT21F+nzLoMB1vjlAYmB5lG70BDtZUxOM42gAuvpN5zCHIo8=\n-----END PRIVATE KEY-----");
            PemObject pemObject = new PemReader(reader).ReadPemObject();
            AsymmetricKeyParameter pkey = PrivateKeyFactory.CreateKey(pemObject.Content);
            var pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
            pkcs1Encoding.Init(false, pkey);
            string crypted = "pi8mkEl8ZhzX2zEa4b1ucMvm8HQvJNuRmy4hTWqb1zxkhHnkOR+CPNqrdTGdyFIjp/3J8mlNWhI6QYNbX7XOKkeW3dM0IomlI7/KlcdFRGbpVmTwz9Wi8xGq30ekMKxqwCvgIiqO2RXd56ry4DeeCcnI678MbzfsEjFKT71e4n0=";
            byte[] bytes = Convert.FromBase64String(crypted);
            byte[] s = pkcs1Encoding.ProcessBlock(bytes, 0, bytes.Length);
            Console.WriteLine(Encoding.UTF8.GetString(s));
        }


        public static void AesCrypt()
        {
            var toEncrypt = Encoding.UTF8.GetBytes("{\"userName\":\"1056879522@qq.com\",\"password\":\"Asdfghjkl-8179\"}");
            using var aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            aes.Key = Convert.FromBase64String("NU90YjNiUDdDWFhGdHNwdg==");
            aes.Padding = PaddingMode.PKCS7;
            using var transform = aes.CreateEncryptor();
            var data = transform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);
            Console.WriteLine(Convert.ToBase64String(data));
        }
    }


}
