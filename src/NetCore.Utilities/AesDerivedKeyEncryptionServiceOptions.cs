namespace ICG.NetCore.Utilities
{
    /// <summary>
    ///     Configuration options for use with the <see cref="AesEncryptionService" />
    /// </summary>
    public class AesDerivedKeyEncryptionServiceOptions
    {
        /// <summary>
        ///     The passphrase to utilize for encryption
        /// </summary>
        public string Passphrase { get; set; }
    }
}
