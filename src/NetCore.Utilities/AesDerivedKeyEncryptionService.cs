using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ICG.NetCore.Utilities
{
    /// <summary>
    ///     Represents a service that implements symmetric encryption utilizing the <see cref="Rfc2898DeriveBytes"/> process for key creation
    /// </summary>
    public interface IAesDerivedKeyEncryptionService
    {

        /// <summary>
        ///     Encrypts the provided plain-text string into an AES encrypted string, utilizing a configured key and IV value
        /// </summary>
        /// <param name="plainTextInput">The plain text string to be encrypted</param>
        /// <param name="salt">The salt value to be used for key creation</param>
        /// <exception cref="ArgumentNullException">If [plainTextInput] is null</exception>
        /// <exception cref="ArgumentNullException">If {salt} is null</exception>
        /// <exception cref="ArgumentNullException">If {passphrase} is null in the configuration</exception>
        /// <returns>A encrypted string representing the provided plain text string.</returns>
        string Encrypt(string plainTextInput, string salt);

        /// <summary>
        ///     Encrypts the provided plain-text string into an AES encrypted string, utilizing a provided key and IV value
        /// </summary>
        /// <param name="plainTextInput">The plain text string to be encrypted</param>
        /// <param name="salt">The salt value to be used for key creation</param>
        /// <param name="passphrase">The passphrease used for key generation</param>
        /// <exception cref="ArgumentNullException">If [plainTextInput] is null</exception>
        /// <exception cref="ArgumentNullException">If {salt} is null</exception>
        /// <exception cref="ArgumentNullException">If {passphrase} is null</exception>
        /// <returns>A encrypted string representing the provided plain text string.</returns>
        string Encrypt(string plainTextInput, string salt, string passphrase);

        /// <summary>
        ///     Decrypts the provided string from an AES encrypted string back to plain text, utilizing a configured key and IV value
        /// </summary>
        /// <param name="encryptedInput">The encrypted string to be decrypted</param>
        /// <param name="salt">The salt value to be used for key creation</param>
        /// <exception cref="ArgumentNullException">If [encryptedInput] is null</exception>
        /// <exception cref="ArgumentNullException">If {salt} is null</exception>
        /// <exception cref="ArgumentNullException">If {passphrase} is null in the configuration</exception>
        /// <returns>A plain text string of the provided encryption text string.</returns>
        string Decrypt(string encryptedInput, string salt);

        /// <summary>
        ///     Decrypts the provided string from an AES encrypted string back to plain text, utilizing a provided key and IV value
        /// </summary>
        /// <param name="encryptedInput">The encrypted string to be decrypted</param>
        /// <param name="salt">The salt value to be used for key creation</param>
        /// <param name="passphrase">The passphrease used for key generation</param>
        /// <exception cref="ArgumentNullException">If [encryptedInput] is null</exception>
        /// <exception cref="ArgumentNullException">If {salt} is null</exception>
        /// <exception cref="ArgumentNullException">If {passphrase} is null</exception>
        /// <returns>A plain text string of the provided encryption text string.</returns>
        string Decrypt(string encryptedInput, string salt, string passphrase);
    }

    /// <inheritdoc />
    public class AesDerivedKeyEncryptionService : IAesDerivedKeyEncryptionService
    {
        private readonly AesDerivedKeyEncryptionServiceOptions _serviceOptions;

        /// <summary>
        ///     Default constructor with DI
        /// </summary>
        /// <param name="serviceOptions">Configuration options</param>
        public AesDerivedKeyEncryptionService(IOptions<AesDerivedKeyEncryptionServiceOptions> serviceOptions)
        {
            _serviceOptions = serviceOptions.Value;
        }

        /// <inheritdoc />
        public string Encrypt(string plainTextInput, string salt)
        {
            return Encrypt(plainTextInput, salt, _serviceOptions.Passphrase);
        }

        /// <inheritdoc />
        public string Encrypt(string plainTextInput, string salt, string passphrase)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(plainTextInput))
                throw new ArgumentNullException(nameof(plainTextInput));
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentNullException(nameof(passphrase));
            var plainTextBytes = Encoding.Unicode.GetBytes(plainTextInput);
            byte[] encrypted;
            
            using (var aesAlg = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(passphrase, Encoding.Unicode.GetBytes(salt));
                aesAlg.Key = key.GetBytes(32);
                aesAlg.IV = key.GetBytes(16);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                        csEncrypt.Close();
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        /// <inheritdoc />
        public string Decrypt(string encryptedInput, string salt)
        {
            return Decrypt(encryptedInput, salt, _serviceOptions.Passphrase);
        }

        /// <inheritdoc />
        public string Decrypt(string encryptedInput, string salt, string passphrase)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(encryptedInput))
                throw new ArgumentNullException(nameof(encryptedInput));
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentNullException(nameof(passphrase));

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = string.Empty;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(passphrase, Encoding.Unicode.GetBytes(salt));
                aesAlg.Key = key.GetBytes(32);
                aesAlg.IV = key.GetBytes(16);
                var encryptedBytes = Convert.FromBase64String(encryptedInput);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedBytes, 0, encryptedBytes.Length);
                        csDecrypt.Close();
                    }
                    plaintext = Encoding.Unicode.GetString(msDecrypt.ToArray());
                }
            }

            return plaintext;
        }
    }
}
