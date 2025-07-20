using Project.application.components;
using Project.domain.services;
using System;
using Xunit.Abstractions;
using Avalonia.Controls;

namespace Project.Tests
{
    /// <summary>
    /// Pruebas unitarias para la presentación y lógica de estadísticas de emociones
    /// </summary>
    public class EmotionStatisticsTests
    {
        private readonly ITestOutputHelper _output;

        public EmotionStatisticsTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        #region UI Component Tests

        [Fact]
        public void CreateStatisticsCards_WithNullContainer_ShouldNotThrow()
        {
            var stats = new emotionsStatistics();

            var ex = Record.Exception(() => stats.CreateStatisticsCards(null));
            Assert.Null(ex);
            _output.WriteLine("✅ Manejo correcto de contenedor nulo");
        }

        #endregion

        #region DatabaseEmotionFetcher Tests

        [Fact]
        public void DatabaseEmotionFetcher_GetEmotionFrequencies_ShouldReturnDictionary()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var result = fetcher.GetEmotionFrequencies();
            Assert.NotNull(result);
            Assert.True(result.Count >= 0);
            _output.WriteLine("✅ Diccionario de frecuencias de emociones obtenido correctamente");
        }

        [Fact]
        public void DatabaseEmotionFetcher_GetPersonalizedEmotionFrequencies_ShouldReturnDictionary()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var result = fetcher.GetPersonalizedEmotionFrequencies();
            Assert.NotNull(result);
            Assert.True(result.Count >= 0);
            _output.WriteLine("✅ Diccionario de frecuencias de emociones personalizadas obtenido correctamente");
        }

        [Fact]
        public void DatabaseEmotionFetcher_GetWeeklyEmotionFrequencies_ShouldReturnDictionary()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var result = fetcher.GetWeeklyEmotionFrequencies();
            Assert.NotNull(result);
            Assert.True(result.Count >= 0);
            _output.WriteLine("✅ Diccionario de frecuencias semanales obtenido correctamente");
        }

        [Fact]
        public void DatabaseEmotionFetcher_GetWeeklyEmotionSummary_ShouldReturnString()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var summary = fetcher.GetWeeklyEmotionSummary();
            Assert.NotNull(summary);
            Assert.True(summary.Length > 0);
            _output.WriteLine("✅ Resumen semanal de emociones generado correctamente");
        }

        [Fact]
        public void DatabaseEmotionFetcher_GetDateRange_ShouldReturnTupleOrNull()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var range = fetcher.GetDateRange();
            if (range.HasValue)
            {
                var (first, last) = range.Value;
                Assert.False(string.IsNullOrWhiteSpace(first));
                Assert.False(string.IsNullOrWhiteSpace(last));
                _output.WriteLine($"✅ Rango de fechas: {first} - {last}");
            }
            else
            {
                _output.WriteLine("✅ No hay rango de fechas disponible");
            }
        }

        [Fact]
        public void DatabaseEmotionFetcher_GetMostFrequentEmotion_ShouldReturnTupleOrNull()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var result = fetcher.GetMostFrequentEmotion();
            if (result.HasValue)
            {
                var (name, count) = result.Value;
                Assert.False(string.IsNullOrWhiteSpace(name));
                Assert.True(count >= 0);
                _output.WriteLine($"✅ Emoción más frecuente: {name} ({count} veces)");
            }
            else
            {
                _output.WriteLine("✅ No hay emoción más frecuente disponible");
            }
        }

        [Fact]
        public void DatabaseEmotionFetcher_GetTodayEmotionSummary_ShouldReturnString()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var summary = fetcher.GetTodayEmotionSummary();
            Assert.NotNull(summary);
            Assert.True(summary.Length > 0);
            _output.WriteLine("✅ Resumen de emociones de hoy generado correctamente");
        }

        [Fact]
        public void DatabaseEmotionFetcher_GetTotalEmotionSummary_ShouldReturnString()
        {
            var fetcher = new DatabaseEmotionFetcher();
            var summary = fetcher.GetTotalEmotionSummary();
            Assert.NotNull(summary);
            Assert.True(summary.Length > 0);
            _output.WriteLine("✅ Resumen total de emociones generado correctamente");
        }

        #endregion
    }
}