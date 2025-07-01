using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Project.application.services;
using Project.domain.services;

namespace Project.presentation.components
{
    /// Singleton audio manager
    public class AudioManager
    {
        private static AudioManager? _instance;
        private static readonly object _lock = new object();

        private readonly List<WeakReference<Audio>> _audioInstances = new List<WeakReference<Audio>>();
        private Audio? _currentlyPlayingAudio;

        // Callback para cambios en el estado del audio
        private readonly Dictionary<Audio, Action> _audioStoppedCallbacks = new Dictionary<Audio, Action>();

        private AudioManager() { }

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AudioManager();
                    }
                }
                return _instance;
            }
        }

        /// Registra una instancia de audio
        public void RegisterAudio(Audio audio, Action? onAudioStopped = null)
        {
            if (audio == null) return;

            lock (_lock)
            {
                CleanupDeadReferences();
                _audioInstances.Add(new WeakReference<Audio>(audio));

                // Registra callback
                if (onAudioStopped != null)
                {
                    _audioStoppedCallbacks[audio] = onAudioStopped;
                }
            }
        }

        /// Desregistra una instancia de audio
        public void UnregisterAudio(Audio audio)
        {
            if (audio == null) return;

            lock (_lock)
            {
                _audioInstances.RemoveAll(wr =>
                {
                    if (wr.TryGetTarget(out var target))
                    {
                        return ReferenceEquals(target, audio);
                    }
                    return true;
                });

                // Remove callback
                _audioStoppedCallbacks.Remove(audio);

                // Si es el actual, se detiene el tracking (esto registrará automáticamente en la BD)
                if (ReferenceEquals(_currentlyPlayingAudio, audio))
                {
                    AudioPlaybackTrackingService.Instance.StopTracking(_currentlyPlayingAudio.AudioFileName);
                    _currentlyPlayingAudio = null;
                }
            }
        }

        public bool RequestPlayAudio(Audio requestingAudio)
        {
            if (requestingAudio == null) return false;

            lock (_lock)
            {
                StopCurrentlyPlayingAudio();
                _currentlyPlayingAudio = requestingAudio;

                // Start tracking the new audio
                AudioPlaybackTrackingService.Instance.StartTracking(_currentlyPlayingAudio.AudioFileName);

                return true;
            }
        }

        public void NotifyAudioStopped(Audio stoppedAudio)
        {
            if (stoppedAudio == null) return;

            lock (_lock)
            {
                if (ReferenceEquals(_currentlyPlayingAudio, stoppedAudio))
                {
                    // Stop tracking (esto registrará automáticamente en la BD)
                    AudioPlaybackTrackingService.Instance.StopTracking(_currentlyPlayingAudio.AudioFileName);
                    _currentlyPlayingAudio = null;
                }
            }
        }

        /// Gets the currently playing audio instance
        public Audio? CurrentlyPlayingAudio
        {
            get
            {
                lock (_lock)
                {
                    return _currentlyPlayingAudio;
                }
            }
        }

        public bool IsAnyAudioPlaying
        {
            get
            {
                lock (_lock)
                {
                    return _currentlyPlayingAudio != null;
                }
            }
        }

        public void StopAllAudio()
        {
            lock (_lock)
            {
                StopCurrentlyPlayingAudio();
            }
        }

        /// Gets today's audio playback statistics
        public Dictionary<string, int> GetTodayStatistics()
        {
            return AudioPlaybackTrackingService.Instance.GetTodayStatistics();
        }

        /// Creates a local backup of tracking data (call on app shutdown)
        public void SaveTrackingData()
        {
            // Detener cualquier audio en reproducción para registrar el tiempo
            if (_currentlyPlayingAudio != null)
            {
                AudioPlaybackTrackingService.Instance.StopTracking(_currentlyPlayingAudio.AudioFileName);
                _currentlyPlayingAudio = null;
            }

            // Crear backup local sin forzar registro en BD
            AudioPlaybackTrackingService.Instance.CreateLocalBackup();
        }

        private void StopCurrentlyPlayingAudio()
        {
            if (_currentlyPlayingAudio != null)
            {
                var audioToStop = _currentlyPlayingAudio;
                try
                {
                    // Stop tracking (esto registrará automáticamente en la BD)
                    AudioPlaybackTrackingService.Instance.StopTracking(audioToStop.AudioFileName);

                    audioToStop.StopAudio();

                    if (_audioStoppedCallbacks.TryGetValue(audioToStop, out var callback))
                    {
                        try
                        {
                            callback?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error in audio stopped callback: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error stopping currently playing audio: {ex.Message}");
                }
                finally
                {
                    _currentlyPlayingAudio = null;
                }
            }
        }

        private void CleanupDeadReferences()
        {
            _audioInstances.RemoveAll(wr => !wr.TryGetTarget(out _));
            var keysToRemove = _audioStoppedCallbacks.Keys.Where(audio =>
                !_audioInstances.Any(wr => wr.TryGetTarget(out var target) && ReferenceEquals(target, audio))
            ).ToList();

            foreach (var key in keysToRemove)
            {
                _audioStoppedCallbacks.Remove(key);
            }
        }
    }
}