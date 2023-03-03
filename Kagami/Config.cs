namespace Kagami
{
    internal class Config
    {
        public NeteaseConfig netease { get; set; }
        public ReplicateConfig replicate { get; set; }

        public string OpenAIKey { get; set; }
    }

    public class ReplicateConfig
    {
        public string token { get; set; }
    }
}