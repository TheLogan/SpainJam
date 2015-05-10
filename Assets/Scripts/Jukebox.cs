using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Jukebox : MonoBehaviour {

	[SerializeField]
	AudioClip[] allMusic;

	AudioSource myAudio;
	int musicIndex = 0;

	void Start(){
		myAudio = GetComponent<AudioSource>();
	}

	void Update(){
		if(!myAudio.isPlaying){
			bool gotNewIndex = false;

			while (!gotNewIndex){
				int newInt = Random.Range(0, allMusic.Length);

				if(newInt != musicIndex){
					musicIndex = newInt;
					gotNewIndex = true;
				}
			}
			myAudio.clip = allMusic[musicIndex];
			myAudio.Play();
		}
		myAudio.pitch = Mathf.Lerp(myAudio.pitch, Time.timeScale, Time.deltaTime * 1.5f);
	}
}
