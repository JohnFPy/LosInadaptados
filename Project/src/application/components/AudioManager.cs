using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.presentation.components
{
    /// <summary>
    /// Singleton class that manages global audio playback to ensure only one audio plays at a time
    /// </summary>
    public class AudioManager
    {
        private static AudioManager? _instance;
        private static readonly object _lock = new object();
        
        private readonly List<WeakReference<Audio>> _audioInstances = new List<WeakReference<Audio>>();
        private Audio? _currentlyPlayingAudio;

        // Callback registry for audio state changes
        private readonly Dictionary<Audio, Action> _audioStoppedCallbacks = new Dictionary<Audio, Action>();

        private AudioManager() { }

        /// <summary>
        /// Gets the singleton instance of AudioManager
        /// </summary>
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

        /// <summary>
        /// Registers an Audio instance with the manager
        /// </summary>
        /// <param name="audio">The Audio instance to register</param>
        /// <param name="onAudioStopped">Optional callback when this audio is stopped by the manager</param>
        public void RegisterAudio(Audio audio, Action? onAudioStopped = null)
        {
            if (audio == null) return;

            lock (_lock)
            {
                // Clean up dead references first
                CleanupDeadReferences();

                // Add the new audio instance
                _audioInstances.Add(new WeakReference<Audio>(audio));

                // Register callback if provided
                if (onAudioStopped != null)
                {
                    _audioStoppedCallbacks[audio] = onAudioStopped;
                }
            }
        }

        /// <summary>
        /// Unregisters an Audio instance from the manager
        /// </summary>
        /// <param name="audio">The Audio instance to unregister</param>
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
                    return true; // Remove dead references too
                });

                // Remove callback
                _audioStoppedCallbacks.Remove(audio);

                // If this was the currently playing audio, clear the reference
                if (ReferenceEquals(_currentlyPlayingAudio, audio))
                {
                    _currentlyPlayingAudio = null;
                }
            }
        }

        /// <summary>
        /// Requests to play audio. This will stop any currently playing audio first.
        /// </summary>
        /// <param name="requestingAudio">The Audio instance requesting to play</param>
        /// <returns>True if the audio can start playing, false otherwise</returns>
        public bool RequestPlayAudio(Audio requestingAudio)
        {
            if (requestingAudio == null) return false;

            lock (_lock)
            {
                // Stop any currently playing audio
                StopCurrentlyPlayingAudio();

                // Set the new currently playing audio
                _currentlyPlayingAudio = requestingAudio;
                return true;
            }
        }

        /// <summary>
        /// Notifies the manager that an audio has stopped playing
        /// </summary>
        /// <param name="stoppedAudio">The Audio instance that stopped</param>
        public void NotifyAudioStopped(Audio stoppedAudio)
        {
            if (stoppedAudio == null) return;

            lock (_lock)
            {
                if (ReferenceEquals(_currentlyPlayingAudio, stoppedAudio))
                {
                    _currentlyPlayingAudio = null;
                }
            }
        }

        /// <summary>
        /// Gets the currently playing audio instance
        /// </summary>
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

        /// <summary>
        /// Checks if any audio is currently playing
        /// </summary>
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

        /// <summary>
        /// Stops all currently playing audio
        /// </summary>
        public void StopAllAudio()
        {
            lock (_lock)
            {
                StopCurrentlyPlayingAudio();
            }
        }

        private void StopCurrentlyPlayingAudio()
        {
            if (_currentlyPlayingAudio != null)
            {
                var audioToStop = _currentlyPlayingAudio;
                try
                {
                    audioToStop.StopAudio();

                    // Call the registered callback if exists
                    if (_audioStoppedCallbacks.TryGetValue(audioToStop, out var callback))
                    {
                        try
                        {
                            callback?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in audio stopped callback: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping currently playing audio: {ex.Message}");
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
            
            // Clean up dead callback references
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