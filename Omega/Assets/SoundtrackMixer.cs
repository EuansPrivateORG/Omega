using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Core
{
    public class SoundtrackMixer : MonoBehaviour
    {
        [SerializeField] public AudioSource soundtrackSource;
        [SerializeField] public AudioClip menuClip;
        [SerializeField] public List<AudioClip> soundtracks = new List<AudioClip>();

        private bool playingMainMenu = true;
        private float currentSoundtrackLength = 0;
        private float currentSoundTrackTime = 0;
        private int soundtrackNumber = 0;
        private void Start()
        {
            PlayMenu();
        }

        private void Update()
        {
            if (!playingMainMenu)
            {
                currentSoundTrackTime += Time.deltaTime;
                currentSoundtrackLength = soundtrackSource.clip.length;

                if (currentSoundTrackTime > currentSoundtrackLength - 3f)
                {
                    PlayNewSoundTrack();
                }
            }
            else
            {
                currentSoundTrackTime += Time.deltaTime;
                currentSoundtrackLength = soundtrackSource.clip.length;

                if (currentSoundTrackTime > currentSoundtrackLength - 3f)
                {
                    PlayMenu();
                }
            }
        }

        public void PlayNewSoundTrack()
        {
            playingMainMenu = false;
            Debug.Log(soundtrackNumber);
            for (int i = 0; i < soundtracks.Count; i++)
            {
                if(i == soundtrackNumber)
                {
                    Debug.Log(i);
                    soundtrackSource.clip = soundtracks[i];
                    soundtrackNumber++;
                    break;
                }
            }
            if(soundtrackNumber > soundtracks.Count)
            {
                soundtrackNumber = 0;
            }
            soundtrackSource.Play();
            soundtrackSource.volume = 1f;
            currentSoundTrackTime = 0;
        }

        public IEnumerator FadeOut(AudioSource audioSource, AudioClip clipToFadeIn)
        {
            float fadeDuration = 2.0f; // Duration of the fade-out/in in seconds

            // Fade out
            float startVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            // Change the clip
            audioSource.clip = clipToFadeIn;

            // Fade in
            audioSource.Play();
            while (audioSource.volume < startVolume)
            {
                audioSource.volume += startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
        }

        public IEnumerator FadeIn(AudioSource audioSource, AudioClip clipToFadeIn)
        {
            audioSource.clip = clipToFadeIn;
            float fadeDuration = 2.0f; // Duration of the fade-in in seconds

            // Fade in
            audioSource.Play();
            audioSource.volume = 0;
            while (audioSource.volume < 1)
            {
                audioSource.volume += Time.deltaTime / fadeDuration;
                yield return null;
            }
        }

        public void PlayMenu()
        {
            playingMainMenu = true;


            if (soundtrackSource.isPlaying)
            {
                StartCoroutine(FadeOut(soundtrackSource, menuClip));
            }
            else
            {
                StartCoroutine(FadeIn(soundtrackSource, menuClip));
            }

            currentSoundTrackTime = 0;
        }
    }
}
