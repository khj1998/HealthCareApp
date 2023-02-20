using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOMA.Sound
{
	public class StateSound : MonoBehaviour
	{
		[SerializeField]
		private AudioSource _audioSource;
		[SerializeField]
		private AudioClip _breakTime;
		[SerializeField]
		private AudioClip _half;
		[SerializeField]
		private AudioClip _beep;

		public void HalfPlay()
		{
			_audioSource.clip = _half;
			_audioSource.Play();
		}
		public void BreakTimePlay()
		{
			_audioSource.clip = _breakTime;
			_audioSource.Play();
		}
		public void BeepPlay()
		{
			_audioSource.clip = _beep;
			_audioSource.Play();
		}
	}
}
