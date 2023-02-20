using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOMA.Sound
{
	public class TipSoundManager : MonoBehaviour
	{
		[SerializeField]
		private AudioSource _audioSource;
		[SerializeField]
		private AudioClip[] _tips;

		public void TipPlay(int exerNum)
		{
			_audioSource.clip = _tips[exerNum];
			_audioSource.Play();
		}
	}
}
