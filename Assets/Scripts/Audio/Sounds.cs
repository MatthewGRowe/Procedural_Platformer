using UnityEngine.Audio;  //Added this
using UnityEngine;

[System.Serializable]   //Without this nothing will appear in the audiomanager game object in Unity
public class Sound  //This is a class containing a list of sounds
{
	//This class allows us to store many instances of a sound object along with the additonal selected attribs
	//It is used by the AudioManager script

	public string name; //Name of the audio
	public AudioClip clip;

	[Range(0f, 1f)]  //Creates a slider for us to adjust the volume
	public float volume;

	[Range(.5f,3f)]  //Creates a slider to adjust the pitch
	public float pitch;

	public bool loop; //Should we loop?

	[HideInInspector]  //Hides this as it is automatically populated but needs to be public so it can be accessed in the code
	public AudioSource source;
}
