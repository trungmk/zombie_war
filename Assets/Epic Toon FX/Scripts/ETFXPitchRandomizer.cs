using UnityEngine;
using System.Collections;

namespace EpicToonFX
{

	public class ETFXPitchRandomizer : MonoBehaviour
	{
	
		public float randomPercent = 10;
	
		public void PlayAudio ()
		{
			transform.GetComponent<AudioSource>().pitch *= 1 + Random.Range(-randomPercent / 100, randomPercent / 100);
		}
	}
}