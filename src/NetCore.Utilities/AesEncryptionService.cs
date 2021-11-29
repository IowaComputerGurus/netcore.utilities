using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;

namespace ICG.NetCore.Utilities
{
    /// <summary>
    ///     Represents a service that can encrypt and decrypt strings using symmetric AES encryption, using a stored key/iv.
    /// </summary>
    public interface IAesEncryptionService
    {
        /// <summary>
        ///     Encrypts the provided string into an AES encrypted string.
        /// </summary>
        /// <param name="plainTextInput">The plain text string to be encrypted</param>
        /// <exception cref="ArgumentNullException">If [plainTextInput] is null</exception>
        /// <exception cref="ArgumentNullException">If option key is null</exception>
        /// <exception cref="ArgumentNullException">If option IV is null</exception>
        /// <returns>A encrypted string representing the provided plain text string.</returns>
        string Encrypt(string plainTextInput);

        /// <summary>
        ///     Decrypts the provided string from an AES encrypted string back to plain text.
        /// </summary>
        /// <param name="encryptedInput">The encrypted string to be decrypted</param>
        /// <exception cref="ArgumentNullException">If [encryptedInput] is null</exception>
        /// <exception cref="ArgumentNullException">If option key is null</exception>
        /// <exception cref="ArgumentNullException">If option IV is null</exception>
        /// <returns>A plain text string of the provided encryption text string.</returns>
        string Decrypt(string encryptedInput);
    }

    /// <inheritdoc />
    public class AesEncryptionService : IAesEncryptionService
    {
        private readonly AesEncryptionServiceOptions _serviceOptions;

        /// <summary>
        ///     Default constructor with DI
        /// </summary>
        /// <param name="serviceOptions">Configuration options</param>
        public AesEncryptionService(IOptions<AesEncryptionServiceOptions> serviceOptions)
        {
            _serviceOptions = serviceOptions.Value;
        }

        /// <inheritdoc />
        public string Encrypt(string plainTextInput)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(plainTextInput))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(_serviceOptions.Key))
                throw new ArgumentNullException("Key");
            if (string.IsNullOrEmpty(_serviceOptions.IV))
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_serviceOptions.Key);
                aesAlg.IV = Convert.FromBase64String(_serviceOptions.IV);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainTextInput);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        /// <inheritdoc />
        public string Decrypt(string encryptedInput)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(encryptedInput))
                throw new ArgumentNullException("encryptedInput");
            if (string.IsNullOrEmpty(_serviceOptions.Key))
                throw new ArgumentNullException("Key");
            if (string.IsNullOrEmpty(_serviceOptions.IV))
                throw new ArgumentNullException("IV");
            byte[] encrypted;


            // Declare the string used to hold
            // the decrypted text.
            string plaintext = string.Empty;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_serviceOptions.Key);
                aesAlg.IV = Convert.FromBase64String(_serviceOptions.IV);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedInput)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
