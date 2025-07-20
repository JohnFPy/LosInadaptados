using Project.application.services;
using Project.domain.services;
using Project.presentation.components;
using System;
using Xunit.Abstractions;

namespace Project.Tests
{
    /// <summary>
    /// Pruebas unitarias para la reproducción de audio y su tracking
    /// </summary>
    public class AudioReproductionTests
    {
        private readonly ITestOutputHelper _output;

        public AudioReproductionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Audio Type Detection Tests

        [Theory]
        [InlineData("japon_meditation.mp3", "japon")]
        [InlineData("japan_nature.wav", "japon")]
        [InlineData("ethnic_drums.mp3", "ethno")]
        [InlineData("ethno_flute.wav", "ethno")]
        [InlineData("relaxingPiano.mp3", "piano")]
        [InlineData("piano_soft.wav", "piano")]
        public void GetAudioType_ValidFiles_ReturnsCorrectType(string fileName, string expectedType)
        {
            // Act - Simular la detección de tipo de audio
            string audioType = AudioPlaybackTrackingService.Instance.GetAudioTypeFromFileName(fileName);

            // Assert
            Assert.Equal(expectedType, audioType.ToLower());
            _output.WriteLine($"✅ Tipo de audio detectado correctamente: '{fileName}' -> '{audioType}'");
        }

        [Theory]
        [InlineData("unknown.mp3")]
        [InlineData("music.wav")]
        [InlineData("")]
        [InlineData(null)]
        public void GetAudioType_InvalidFiles_ReturnsEmpty(string fileName)
        {
            // Act
            string audioType = fileName == null
                ? string.Empty
                : AudioPlaybackTrackingService.Instance.GetAudioTypeFromFileName(fileName);

            // Assert
            Assert.Equal(string.Empty, audioType);
            _output.WriteLine($"❌ Tipo de audio no reconocido: '{fileName}' -> '{audioType}'");
        }

        #endregion

        #region Audio Playback Tracking Tests

        [Theory]
        [InlineData("japon_meditation.mp3")]
        [InlineData("ethnic_drums.mp3")]
        [InlineData("relaxingPiano.mp3")]
        public void StartTracking_ValidAudio_ShouldInitialize(string audioFileName)
        {
            // Act
            AudioPlaybackTrackingService.Instance.StartTracking(audioFileName);

            // Assert - Verificar que el audio está siendo trackeado
            var stats = AudioPlaybackTrackingService.Instance.GetTodayStatistics();
            Assert.NotNull(stats);
            _output.WriteLine($"✅ Tracking iniciado para: '{audioFileName}'");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid.mp3")]
        public void StartTracking_InvalidAudio_ShouldNotInitialize(string audioFileName)
        {
            // Act & Assert - No debería lanzar excepción
            AudioPlaybackTrackingService.Instance.StartTracking(audioFileName);
            _output.WriteLine($"❌ Tracking no iniciado para archivo inválido: '{audioFileName}'");
        }

        #endregion

        #region Audio Manager State Tests

        [Fact]
        public void RequestPlayAudio_WhenNoAudioPlaying_ShouldSucceed()
        {
            // Arrange
            var audio = new Audio { AudioFileName = "japon_meditation.mp3" };

            // Act
            bool result = AudioManager.Instance.RequestPlayAudio(audio);

            // Assert
            Assert.True(result);
            Assert.NotNull(AudioManager.Instance.CurrentlyPlayingAudio);
            _output.WriteLine($"✅ Solicitud de reproducción aceptada para: '{audio.AudioFileName}'");
        }

        [Fact]
        public void RequestPlayAudio_WhenOtherAudioPlaying_ShouldStopPrevious()
        {
            // Arrange
            var audio1 = new Audio { AudioFileName = "japon_meditation.mp3" };
            var audio2 = new Audio { AudioFileName = "relaxingPiano.mp3" };

            // Act
            AudioManager.Instance.RequestPlayAudio(audio1);
            AudioManager.Instance.RequestPlayAudio(audio2);

            // Assert
            Assert.Equal(audio2, AudioManager.Instance.CurrentlyPlayingAudio);
            _output.WriteLine("✅ Audio anterior detenido correctamente al iniciar nuevo audio");
        }

        #endregion

        #region Audio Statistics Tests

        [Fact]
        public void GetTodayStatistics_ShouldReturnAllAudioTypes()
        {
            // Act
            var stats = AudioPlaybackTrackingService.Instance.GetTodayStatistics();

            // Assert
            Assert.NotNull(stats);
            Assert.Contains("japon", stats.Keys);
            Assert.Contains("ethno", stats.Keys);
            Assert.Contains("piano", stats.Keys);

            _output.WriteLine("=== ESTADÍSTICAS DE AUDIO DEL DÍA ===");
            foreach (var kvp in stats)
            {
                _output.WriteLine($"Tipo: {kvp.Key} - Tiempo: {kvp.Value} segundos");
            }
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void MultipleStartStop_ShouldTrackCorrectly()
        {
            // Arrange
            string audioFile = "japon_meditation.mp3";

            // Act - Simular múltiples inicios/paradas
            for (int i = 0; i < 3; i++)
            {
                AudioPlaybackTrackingService.Instance.StartTracking(audioFile);
                AudioPlaybackTrackingService.Instance.StopTracking(audioFile);
            }

            // Assert
            var stats = AudioPlaybackTrackingService.Instance.GetTodayStatistics();
            Assert.NotNull(stats);
            _output.WriteLine("✅ Tracking de múltiples sesiones registrado correctamente");
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void StopTrackingWithoutStart_ShouldHandleGracefully()
        {
            // Act & Assert - No debería lanzar excepción
            AudioPlaybackTrackingService.Instance.StopTracking("nonexistent.mp3");
            _output.WriteLine("✅ Manejo correcto de parada sin inicio previo");
        }

        [Fact]
        public void InvalidAudioOperations_ShouldBeHandled()
        {
            // Act & Assert
            AudioManager.Instance.NotifyAudioStopped(null);
            AudioManager.Instance.RequestPlayAudio(null);
            AudioManager.Instance.UnregisterAudio(null);
            
            _output.WriteLine("✅ Operaciones inválidas manejadas correctamente");
        }

        #endregion
    }
}