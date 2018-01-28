using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
	public static Audio Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<Audio>();
			}

			return _instance;
		}
	}

	private static Audio _instance;

	public AudioSource travel;
	public AudioSource travel2;
	public AudioSource error;
	public AudioSource dialUp;
	public List<AudioSource> swooshes;

	public static void PlayAudioSource(AudioSource audioSource, float fadeInTime = 0f)
	{
		if (audioSource != null && !audioSource.isPlaying)
		{
			if (fadeInTime == 0)
			{
				audioSource.Play();
			}
			else
			{
				_instance.StartCoroutine( _instance.FadeInAudioSource( audioSource , fadeInTime ) );
			}
		}
	}

	public static void PlayAudioSource(List<AudioSource> audioSourceList, float fadeInTime = 0f, float randomPitch = 0.1f)
	{
		var randomIndex = Random.Range(0, audioSourceList.Count);
		var audioSource = audioSourceList[randomIndex];

		audioSource.pitch += Random.Range(-randomPitch, randomPitch);

		PlayAudioSource(audioSource, fadeInTime);
	}

	public static void StopAudioSource(AudioSource audioSource, float fadeOutTime = 0f)
	{
		if (audioSource != null)
		{
			if (fadeOutTime == 0)
			{
				audioSource.Stop();
			}
			else
			{
				_instance.StartCoroutine(_instance.FadeOutAudioSource(audioSource, fadeOutTime));
			}
		}
	}

	private IEnumerator FadeOutAudioSource(AudioSource audioSource, float time)
	{
		var originalVolume = audioSource.volume;

		time = Mathf.Max(0.01f, time);
		originalVolume = Mathf.Max(0.01f, originalVolume );

		var ratio = originalVolume/time;

		while (audioSource.volume >= 0f)
		{
			audioSource.volume -= ratio * Time.deltaTime;
			if (audioSource.volume <= 0f) break;
			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = originalVolume;

		yield return null;
	}

	private IEnumerator FadeInAudioSource(AudioSource audioSource, float time)
	{
		var originalVolume = audioSource.volume;

		audioSource.volume = 0f;
		audioSource.Play();

		time = Mathf.Max(0.01f, time);
		originalVolume = Mathf.Max(0.01f, originalVolume );

		var ratio = originalVolume/time;

		while (audioSource.volume >= 0f)
		{
			audioSource.volume += ratio * Time.deltaTime;
			if (audioSource.volume >= originalVolume) break;
			yield return null;
		}

		audioSource.volume = originalVolume;

		yield return null;
	}


	private void PlayRandomClipFromList(List<AudioSource> list)
	{
		list[ Random.Range( 0 , list.Count ) ].Play();
	}


}
