namespace MainSite.Models
{
    public class CiphersViewModel
    {
        public string PlainTextInput { get; set; }
        public string EncryptedOutput { get; set; }

        public string EncryptedInput { get; set; }
        public string DecryptedOutput { get; set; }

        public string PlainTextPasswordInput { get; set; }
        public string HashedPasswordOutput { get; set; }

        public string VerifyPasswordInput { get; set; }
        public string HashedPasswordInput { get; set; }
        public bool? VerificationResult { get; set; }

    }
}