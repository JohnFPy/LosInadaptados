using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Project.application.services
{
    public static class emotionColorMapper
    {
        public static readonly Dictionary<string, string> EmotionColors = new()
        {
            { "Feliz", "#FFD700" },       // Yellow
            { "Triste", "#1E90FF" },      // Blue
            { "Enojado", "#FF4500" },     // Red
            { "Preocupado", "#8A2BE2" },  // Purple
            { "Serio", "#3CB371" },       // Green
        };

        public static string GetColor(string emotionName, bool isPersonalized = false)
        {
            if (!isPersonalized && EmotionColors.TryGetValue(emotionName, out var color))
                return color;

            // Custom emotion color (or default if not found)
            return GenerateColorFromName(emotionName);
        }

        private static string GenerateColorFromName(string input)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input.ToLowerInvariant()));

            // Usamos los primeros 3 bytes para RGB
            int r = hash[0];
            int g = hash[1];
            int b = hash[2];

            // Aseguramos que el color no sea muy oscuro ni muy claro
            r = Clamp(r, 50, 200);
            g = Clamp(g, 50, 200);
            b = Clamp(b, 50, 200);

            return $"#{r:X2}{g:X2}{b:X2}";
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }
}
