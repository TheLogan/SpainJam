using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SphereObject : MonoBehaviour {

	[SerializeField]
	AudioClip pickupSound;


	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player"){
			Destroy(gameObject);
			GetComponent<AudioSource>().PlayOneShot(pickupSound);
			Application.LoadLevel("WinGame");
		}
	}
}
