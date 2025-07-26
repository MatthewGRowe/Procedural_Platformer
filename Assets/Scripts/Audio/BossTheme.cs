using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTheme : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D trigger)
	{

		if (trigger.tag == MyTags.PLAYER_TAG)
		{
			//End game and restart
			FindObjectOfType<AudioManager> ().StopTheme ();
			StartCoroutine (StartBossTheme ());
		}
	}

	IEnumerator StartBossTheme()
	{
		yield return new WaitForSeconds (0.5f);
		FindObjectOfType<AudioManager> ().Play (MyTags.SOUND_BOSSTHEME);
		//RenderSettings.ambientLight = Color.Lerp (RenderSettings.ambientLight, null, Time.deltaTime);

		//Fade out to black
		//float fadeTime = GameObject.Find("Canvas").GetComponent<Fading>().BeginFade(1);
		//yield return new WaitForSeconds (fadeTime);


	}
}
