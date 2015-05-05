using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	/*
	 * fixed camera when in room, force player to change room, when room change started
	 * 
	 */ 

	[SerializeField]
	GameObject targetRoom;

	[SerializeField]
	float wantedDistance = 0;

	public void PlayerMovedRoom(GameObject newRoom){
		targetRoom = newRoom;
	}


	void Update(){
		Vector3 wantedPosition = new Vector3(targetRoom.transform.position.x, targetRoom.transform.position.y, wantedDistance);

		transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime);
	}
}
