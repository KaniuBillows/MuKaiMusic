﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Encrypt
{
    public class RsaEncrypt
    {
        public static RsaEncrypt CreateEncrypt()
        {
            return new RsaEncrypt();
        }

        public byte[] Encrypt_NoPadding(byte[] data, string key)
        {
            try
            {
                var encryptEngine = new RsaEngine();
                using (var txtReader = new StringReader(key))
                {
                    var keyParameter = (AsymmetricKeyParameter)new PemReader(txtReader).ReadObject();
                    encryptEngine.Init(true, keyParameter);
                }
                byte[] encrypted = encryptEngine.ProcessBlock(data, 0, data.Length);
                return encrypted;
            }
            catch
            {
                return null;
            }

        }

        public byte[] Encrypt(byte[] data, string key)
        {
            using var rsa = DecodeRSAPublicKey(Convert.FromBase64String(key));
            return rsa.Encrypt(data, false);

        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publickey)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"  
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------  
            MemoryStream mem = new MemoryStream(publickey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading  

            try
            {

                ushort twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)  
                    binr.ReadByte();    //advance 1 byte  
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes  
                else
                    return null;


                byte[] seq = binr.ReadBytes(15); //read the Sequence OID  
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct  
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)  
                    binr.ReadByte();    //advance 1 byte  
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes  
                else
                    return null;

                byte bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next  
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)  
                    binr.ReadByte();    //advance 1 byte  
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes  
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)  
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus  
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes  
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order  
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it  
                    binr.ReadByte();    //skip this null byte  
                    modsize -= 1;   //reduce modulus buffer size by 1  
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes  

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data  
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)  
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----  
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters()
                {
                    Modulus = modulus,
                    Exponent = exponent
                };
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }
    }
}
