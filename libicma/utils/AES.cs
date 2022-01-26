using System.Security.Cryptography;

namespace libicma.utils
{
    public static class AES
    {
        /// <summary>
        /// Encrypt a byte array using AES 256
        /// </summary>
        /// <param name="key">256 bit key</param>
        /// <param name="secret">byte array that need to be encrypted</param>
        /// <returns>Encrypted array</returns>
        public static byte[] EncryptByteArray(byte[] key, byte[] iv, byte[] secret)
        {

            using var cryptor = new AesManaged();
            cryptor.Mode = CipherMode.CBC;
            cryptor.Padding = PaddingMode.PKCS7;
            cryptor.KeySize = 256;
            cryptor.BlockSize = 128;
            using var encryptor = cryptor.CreateEncryptor(key, iv);
            return encryptor.TransformFinalBlock(secret, 0, secret.Length);
        }

        /// <summary>
        /// Decrypt a byte array using AES 256
        /// </summary>
        /// <param name="key">key in bytes</param>
        /// <param name="iv">iv in bytes</param>
        /// <param name="secret">the encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public static byte[] DecryptByteArray(byte[] key, byte[] iv, byte[] secret)
        {
            using var aes = new AesManaged();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            using var decryptor = aes.CreateDecryptor(key, iv);
            return decryptor.TransformFinalBlock(secret, 0, secret.Length);
        }
    }
}
