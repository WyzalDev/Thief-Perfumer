using System;
using UnityEngine;

namespace Game.Scripts.Audio
{
    [Serializable]
    public class Sound
    {
        public string name;
        
        public float volume;

        public AudioClip clip;
    }
}
