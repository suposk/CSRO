namespace CSRO.Common
{
    public class KeyVaultConfig
    {
        public bool UseKeyVault { get; set; }
        public string CsroVaultName { get; set; } = "CsroVaultNeuDev";

        public string ClientSecretWebApp { get; set; }
        public string ClientSecretApi { get; set; }
        public string ClientSecretAuth { get; set; }
        public string ClientSecretAdo { get; set; }
    }
}
