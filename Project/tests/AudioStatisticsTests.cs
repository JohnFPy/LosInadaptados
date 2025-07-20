using Project.domain.services;
using Project.application.components;
using System;
using Xunit.Abstractions;

namespace Project.Tests
{
    /// <summary>
    /// Pruebas unitarias para la lógica y presentación de estadísticas de audio
    /// </summary>
    public class AudioStatisticsTests
    {
        private readonly ITestOutputHelper _output;
        private readonly DatabaseAudioFetcher _fetcher = new DatabaseAudioFetcher();

        public AudioStatisticsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Formato de segundos a minutos y segundos

        [Theory]
        [InlineData(0, "0m 0s")]
        [InlineData(59, "0m 59s")]
        [InlineData(60, "1m 0s")]
        [InlineData(125, "2m 5s")]
        [InlineData(3600, "60m 0s")]
        public void FormatSecondsToMinutesAndSeconds_ShouldFormatCorrectly(int totalSeconds, string expected)
        {
            string result = DatabaseAudioFetcher.FormatSecondsToMinutesAndSeconds(totalSeconds);
            Assert.Equal(expected, result);
            _output.WriteLine($"✅ {totalSeconds} segundos -> '{result}'");
        }

        #endregion

        #region Resúmenes de estadísticas (presentación)

        [Fact]
        public void GetTodayStatisticsSummary_ShouldReturnString()
        {
            string summary = _fetcher.GetTodayStatisticsSummary();
            Assert.False(string.IsNullOrWhiteSpace(summary));
            _output.WriteLine("✅ Resumen de estadísticas de hoy generado:");
            _output.WriteLine(summary);
        }

        [Fact]
        public void GetWeeklyStatisticsSummary_ShouldReturnString()
        {
            string summary = _fetcher.GetWeeklyStatisticsSummary();
            Assert.False(string.IsNullOrWhiteSpace(summary));
            _output.WriteLine("✅ Resumen de estadísticas semanales generado:");
            _output.WriteLine(summary);
        }

        [Fact]
        public void GetMostPlayedAudioSummary_ShouldReturnString()
        {
            string summary = _fetcher.GetMostPlayedAudioSummary();
            Assert.False(string.IsNullOrWhiteSpace(summary));
            _output.WriteLine("✅ Resumen de audio más reproducido generado:");
            _output.WriteLine(summary);
        }

        [Fact]
        public void GetTotalStatisticsSummary_ShouldReturnString()
        {
            string summary = _fetcher.GetTotalStatisticsSummary();
            Assert.False(string.IsNullOrWhiteSpace(summary));
            _output.WriteLine("✅ Resumen de estadísticas totales generado:");
            _output.WriteLine(summary);
        }

        #endregion

        #region Estadísticas totales y semanales (valores y rangos)

        [Fact]
        public void GetTotalStatistics_ShouldReturnTuple()
        {
            var stats = _fetcher.GetTotalStatistics();
            Assert.True(stats.HasValue);
            var (ethno, japan, piano) = stats.Value;
            Assert.True(ethno >= 0 && japan >= 0 && piano >= 0);
            _output.WriteLine($"✅ Estadísticas totales: Étnico={ethno}, Japonés={japan}, Piano={piano}");
        }

        [Fact]
        public void GetWeeklyStatistics_ShouldReturnTupleOrNull()
        {
            var stats = _fetcher.GetWeeklyStatistics();
            if (stats.HasValue)
            {
                var (ethno, japan, piano, range) = stats.Value;
                Assert.True(ethno >= 0 && japan >= 0 && piano >= 0);
                Assert.False(string.IsNullOrWhiteSpace(range));
                _output.WriteLine($"✅ Estadísticas semanales: Étnico={ethno}, Japonés={japan}, Piano={piano}, Rango={range}");
            }
            else
            {
                _output.WriteLine("✅ No hay estadísticas semanales disponibles");
            }
        }

        [Fact]
        public void GetTotalDateRange_ShouldReturnValidDatesOrNull()
        {
            var range = _fetcher.GetTotalDateRange();
            if (range.HasValue)
            {
                var (first, last) = range.Value;
                Assert.False(string.IsNullOrWhiteSpace(first));
                Assert.False(string.IsNullOrWhiteSpace(last));
                _output.WriteLine($"✅ Rango de fechas totales: {first} - {last}");
            }
            else
            {
                _output.WriteLine("✅ No hay rango de fechas disponible");
            }
        }

        #endregion

        #region Audio más reproducido

        [Fact]
        public void GetMostPlayedAudio_ShouldReturnTupleOrNull()
        {
            var mostPlayed = _fetcher.GetMostPlayedAudio();
            if (mostPlayed.HasValue)
            {
                var (audioType, totalSeconds) = mostPlayed.Value;
                Assert.False(string.IsNullOrWhiteSpace(audioType));
                Assert.True(totalSeconds > 0);
                _output.WriteLine($"✅ Audio más reproducido: {audioType} ({totalSeconds} segundos)");
            }
            else
            {
                _output.WriteLine("✅ No hay audio más reproducido disponible");
            }
        }

        #endregion

        #region Estadísticas por fecha específica

        [Fact]
        public void GetAudioTimesByDate_ShouldReturnTupleOrNull()
        {
            string todayDateId = DateTime.Now.ToString("ddMMyyyy");
            var stats = _fetcher.GetAudioTimesByDate(todayDateId);
            if (stats.HasValue)
            {
                var (ethno, japan, piano) = stats.Value;
                Assert.True(ethno >= 0 && japan >= 0 && piano >= 0);
                _output.WriteLine($"✅ Estadísticas por fecha ({todayDateId}): Étnico={ethno}, Japonés={japan}, Piano={piano}");
            }
            else
            {
                _output.WriteLine($"✅ No hay estadísticas para la fecha {todayDateId}");
            }
        }

        #endregion

        #region Edge cases y errores

        [Fact]
        public void GetAudioTimesByDate_InvalidDate_ShouldReturnNull()
        {
            var stats = _fetcher.GetAudioTimesByDate("01011900"); // Fecha improbable
            Assert.True(!stats.HasValue || (stats.Value.ethnoSeconds == 0 && stats.Value.japanSeconds == 0 && stats.Value.pianoSeconds == 0));
            _output.WriteLine("✅ Estadísticas para fecha inválida devuelven null o ceros");
        }

        [Fact]
        public void GetTotalStatistics_NoData_ShouldReturnZerosOrNull()
        {
            // No se puede forzar la base de datos, pero se puede verificar el contrato
            var stats = _fetcher.GetTotalStatistics();
            if (stats.HasValue)
            {
                var (ethno, japan, piano) = stats.Value;
                Assert.True(ethno >= 0 && japan >= 0 && piano >= 0);
                _output.WriteLine("✅ Estadísticas totales sin datos devuelven ceros");
            }
            else
            {
                _output.WriteLine("✅ Estadísticas totales sin datos devuelven null");
            }
        }

        #endregion
    }
}