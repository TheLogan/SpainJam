using UnityEngine;
using System.Collections;

public class RiftScript : MonoBehaviour {

	[SerializeField]
	AudioSource mySource;


	void OnTriggerEnter2D(Collider2D other){

		if(other.tag == "Player"){
			Destroy(gameObject);
			transform.parent.GetComponent<RoomSettings>().RoomCleared();
			mySource.Play();
		}
	}
}
