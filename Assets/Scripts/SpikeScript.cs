using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour {




	void OnCollisionEnter2D(Collision2D other){
		if(other.transform.tag == "Player"){
			transform.parent.parent.GetComponent<RoomSettings>().PlayerDied();

			GameObject go = (GameObject)Resources.Load("BloodSplatter");
			Vector3 v = other.contacts[0].point;
			v.z = -5;
			go = (GameObject)Instantiate(go, v, Quaternion.Euler(new Vector3(0, 180, 0)));
			Destroy(go, 2);
		}
	}
}
