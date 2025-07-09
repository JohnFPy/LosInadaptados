using Project.domain.models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Project.infrastucture
{
    public class EmotionLogCRUD
    {
        private readonly connectionSqlite _connection = new();

        public bool RegisterEmotion(string dateId, long? idEmotion, long? idPersonalizedEmotion)
        {
            string username = UserSession.GetCurrentUsername();

            return false;
        }

        public (long? idEmotion, long? idPersonalized)? GetEmotionByDate(string dateId)
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

        public bool DeleteEmotionEntry(string dateId)
        {
            string username = UserSession.GetCurrentUsername();

            return false;
        }

        public string? GetEmotionNameById(long emotionId)
        {

            return null;
        }

        public string? GetPersonalizedEmotionNameById(long emotionId)
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

        public Dictionary<string, int> GetEmotionFrequencies()
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

        public Dictionary<string, int> GetPersonalizedEmotionFrequencies()
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

        public List<personalizedEmotion> GetAllPersonalizedEmotionsByUser()
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

    }
}
