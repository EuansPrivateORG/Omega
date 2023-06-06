using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Core
{
    public class SoundtrackMixer : MonoBehaviour
    {
        [SerializeField] public AudioSource soundtrackSource;
        [SerializeField] public List<AudioClip> soundtracks = new List<AudioClip>();


        private float currentSoundtrackLength;




    }
}
