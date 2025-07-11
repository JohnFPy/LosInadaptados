using System;
using System.Collections.Generic;
using System.Diagnostics;
using Project.infrastucture;

namespace Project.domain.services
{
    public class EmotionStatisticsProcessor
    {
        private static EmotionStatisticsProcessor? _instance;
        private static readonly object _lock = new object();
        private readonly EmotionLogCRUD _emotionLogCRUD;
        private readonly DatabaseEmotionFetcher _fetcher;

        private EmotionStatisticsProcessor()
        {
            _emotionLogCRUD = new EmotionLogCRUD();
            _fetcher = new DatabaseEmotionFetcher();
        }

        public static EmotionStatisticsProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new EmotionStatisticsProcessor();
                    }
                }
                return _instance;
            }
        }

        public (long? idEmotion, long? idPersonalized)? GetTodayEmotion()
        {
            var dateId = AudioCRUD.GetTodayDateId();
            return _emotionLogCRUD.GetEmotionByDate(dateId);
        }

        public void ShowBasicInsights()
        {
            var username = UserSession.GetCurrentUsername();

            Debug.WriteLine("=======================================");
            Debug.WriteLine("INSIGHTS EMOCIONALES:");

            var std = _fetcher.GetEmotionFrequencies();
            var per = _fetcher.GetPersonalizedEmotionFrequencies();

            if (std.Count > 0)
            {
                var top = GetTopEmotion(std);
                Debug.WriteLine($"Más frecuente (estándar): {top.Key} ({top.Value} veces)");
            }

            if (per.Count > 0)
            {
                var top = GetTopEmotion(per);
                Debug.WriteLine($"Más frecuente (personalizada): {top.Key} ({top.Value} veces)");
            }
        }

        private KeyValuePair<string, int> GetTopEmotion(Dictionary<string, int> dict)
        {
            string top = null;
            int max = 0;

            foreach (var kvp in dict)
            {
                if (kvp.Value > max)
                {
                    max = kvp.Value;
                    top = kvp.Key;
                }
            }

            return new KeyValuePair<string, int>(top, max);
        }
    }
}
