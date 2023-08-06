using System;
using UnityEngine;

namespace Game.Player.Audio
{
    public class ButtonClickSound : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        private void Start() {
            _audioSource = GetComponent<AudioSource>();
        }

        public void ButtonClick() {
            _audioSource.Play();
        }
    }
}