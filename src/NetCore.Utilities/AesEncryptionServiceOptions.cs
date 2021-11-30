namespace ICG.NetCore.Utilities
{
    /// <summary>
    ///     Configuration options for use with the <see cref="AesEncryptionService" />
    /// </summary>
    public class AesEncryptionServiceOptions
    {
        /// <summary>
        ///     The secret key used for symmetric encryption and decryption methods
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     The initialization vector used for symmetric encryption and decryption methods
        /// </summary>
        public string IV { get; set; }
    }
}
