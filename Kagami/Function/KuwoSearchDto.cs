

namespace Kagami.Function
{
    public class KuwoSearchDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("abslist")]
        public KuwoSong[] abslist { get; set; }
    }
}