using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
	

	private void Awake() {
		EventBus.Bus.AddListener(EventId.OnClick, PlaySound);
		EventBus.Bus.AddListener<bool>(EventId.Sound, MuteSound);
	}

	private void MuteSound(bool mute) {
		audioSource.mute = mute;
	}

	private void PlaySound() {
		audioSource.Play();
		
	}

	private void OnDestroy() {
		EventBus.Bus.RemoveListener(EventId.OnClick, PlaySound);
		EventBus.Bus.RemoveListener<bool>(EventId.Sound, MuteSound);
	}
}
