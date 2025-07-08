using Project.domain.models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Project.infrastucture
{
    public class EmotionLogCRUD
    {
        private readonly connectionSqlite _connection = new connectionSqlite();

        public bool RegisterEmotion(string dateId, string username, long? idEmotion, long? idPersonalizedEmotion)
        {

            return false;
        }

        public (long? idEmotion, long? idPersonalized)? GetEmotionByDate(string dateId, string username)
        {

            return null;
        }

        public bool DeleteEmotionEntry(string dateId, string username)
        {

            return false;
        }

        public string? GetEmotionNameById(long emotionId)
        {
            
            return null;
        }

        public string? GetPersonalizedEmotionNameById(long emotionId, string username)
        {
            
            return null;
        }

        public Dictionary<string, int> GetEmotionFrequencies(string username)
        {

            return null;
        }

        public Dictionary<string, int> GetPersonalizedEmotionFrequencies(string username)
        {

            return null;
        }

        public List<personalizedEmotion> GetAllPersonalizedEmotionsByUser(string username)
        {

            return null;
        }

    }
}
