namespace Project.domain.models
{
    public class userEmotionLog
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string Date { get; set; }
        public long? IdEmotion { get; set; }
        public long? IdPersonalizedEmotion { get; set; }
    }
}
