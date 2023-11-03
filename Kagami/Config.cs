namespace Kagami
{
    internal class Config
    {
        public NeteaseConfig netease { get; set; }
        public ReplicateConfig replicate { get; set; }

        public string OpenAIKey { get; set; }
        public string OpenAIKey2 { get; set; }
        public string MagicStr { get; set; }
        public string Infer { get; set; }
        public string DiscordToken { get; set; }
        public string ChannelId { get; set; }
        public string ServerId { get; set; }
    }

    public class ReplicateConfig
    {
        public string token { get; set; }
    }
}