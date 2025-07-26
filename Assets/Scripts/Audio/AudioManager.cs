using UnityEngine.Audio;
using UnityEngine;
using System; //Allows you to find the name in the Play method
using System.Collections; //IEnumerators!

public class AudioManager : MonoBehaviour 
{

	public Sound[] sounds;  //Array of sounds, initially this does not appear in the game object in Unity, to get it to appear you need to mark the class (Sounds) as serializable
	private string currentThemeSong;
	public static AudioManager instance;  //This ensures that we don't create more than one instance of this object.

	void Awake () 
	{
		if (instance == null) //No instances of this object already exist
		{
			instance = this;
		}
		else
		{
			Destroy (gameObject);  //Destroy self if one already exists
			return;
		}


		DontDestroyOnLoad (gameObject);  //Make this persistant through scenes so if you have theme music it doesn't start and stop between scenes

		//Loop through our audio and for each sound add an audio source
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource> ();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;


		}

	}

	void Start()
	{
		int rnd = UnityEngine.Random.Range(1, 4);
		if (rnd == 1)
		{
			Play(MyTags.SOUND_THEMESONG1);
			currentThemeSong = MyTags.SOUND_THEMESONG1;
		}
		else if (rnd == 2)
		{
			Play(MyTags.SOUND_THEMESONG2);
			currentThemeSong = MyTags.SOUND_THEMESONG2;
		}
		else
		{
			Play(MyTags.SOUND_THEMESONG3);
			currentThemeSong = MyTags.SOUND_THEMESONG3;
		}
	}

	public void StopAllSounds()
	{
		foreach(Sound s in sounds)
		{
			s.source.Stop();
		}
	}
	
	public void Play(string name)  //Public as this gets called from other scripts
	{
		//Loop through and find the name of the clip to be played
		Sound s = Array.Find(sounds, sound => sound.name == name);

		if (s == null)  //If there is no sound of this name!
		{
			print ("Cannot find sound " + name);
		}
		else
		{
			s.source.Play(); 
		}
	}

	public void Play(string name, float pitch)  //Public as this gets called from other scripts, optional "pitch"
	{
		//Loop through and find the name of the clip to be played
		Sound s = Array.Find(sounds, sound => sound.name == name);

		if (s == null)  //If there is no sound of this name!
		{
			print("Cannot find sound " + name);
		}
		else
		{
			s.source.pitch = pitch; //The adjustment in pitch may cause issues later if you want
									//to play at normal pitch (this may override that)
			s.source.Play();
		}
	}

    public void PlayAtSetVolume(string name, float vol_0_to_1)  //Public as this gets called from other scripts, optional "pitch"
    {
        //Loop through and find the name of the clip to be played
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)  //If there is no sound of this name!
        {
            print("Cannot find sound " + name);
        }
        else
        {
            s.source.volume = vol_0_to_1; //The adjustment in pitch may cause issues later if you want
										  //to play at normal pitch (this may override that)

			//Adjust pitch to give a bit of variation
			float newPitch = UnityEngine.Random.Range(0.6f, 1.3f);
			s.source.pitch = newPitch;
            s.source.Play();
        }
    }

    public void StopPlay(string name)  //Public as this gets called from other scripts
	{
		//Loop through and find the name of the clip to be played
		Sound s = Array.Find(sounds, sound => sound.name == name);

		if (s == null)  //If there is no sound of this name!
		{
			print("Cannot find sound " + name);
		}
		else
		{
			s.source.Stop();
		}
	}

	public void FadePlay(string name, float timeToFade)
	{
		//Loop through and find the name of the clip to be played
		Sound s = Array.Find(sounds, sound => sound.name == name);

		if (s == null)  //If there is no sound of this name!
		{
			print("Cannot find sound " + name);
		}
		else
		{
		
            StartCoroutine(FadeOut(s.source, timeToFade, 0f));
		}

	}

	IEnumerator FadeOut(AudioSource audioSource, float duration, float targetVolume)
    {
		float originalVolume = audioSource.volume;
		float currentTime = 0;
		float start = audioSource.volume;
		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
			yield return null;
		}
		audioSource.Stop();
		audioSource.volume = originalVolume;
		yield break;
	}

	public void StopTheme()
	{
		string name = currentThemeSong;
		Sound s = Array.Find(sounds, sound => sound.name == name);

		s.source.Stop ();
	}

	public void StopBossTheme()
	{
		string name = MyTags.SOUND_BOSSTHEME;
		Sound s = Array.Find(sounds, sound => sound.name == name);

		s.source.Stop ();
	}
}
