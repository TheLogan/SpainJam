using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Doorway : MonoBehaviour
{
	/*
	 * detects player collision, and moves the player to the next room.
	 */

	enum Directions
	{
		left,
		right,
		up,
		down
	}
	[SerializeField]
	Directions
		doorwayDirection;
	RoomSettings roomScript;
	[SerializeField]
	List<GameObject>
		availableRooms;
	int selectedRoom;
	[SerializeField]
	Vector3
		direction;
	bool isLinking = false;

	void Start ()
	{
		selectedRoom = Random.Range (0, availableRooms.Count - 1); //TODO can this get to all the rooms, or should I remove the -1
		roomScript = transform.parent.GetComponent<RoomSettings> ();
	}

	bool canEnter = true;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (canEnter) {

			bool playerOnRightSide = PlayerOnRightSide (other.gameObject);

			if (playerOnRightSide) {
				if (other.gameObject == Globals.PlayerGo) {
					print ("player entered");
					StartCoroutine (coolDown ());
					canEnter = false;

					var selectedRoomGo = availableRooms [selectedRoom];
					Vector3 direction = Vector3.zero;
					switch (doorwayDirection) {
					case Directions.down:
						direction = -transform.up;
						break;
					case Directions.left:
						direction = -transform.right;
						break;
					case Directions.right:
						direction = transform.right;
						break;
					case Directions.up:
						direction = transform.up;
						break;
					}

					selectedRoomGo.transform.position = transform.parent.position + direction * 10;
					selectedRoomGo.GetComponent<RoomSettings> ().PlayerEnters ();
					Globals.MainCamera.GetComponent<CameraController> ().PlayerMovedRoom (selectedRoomGo);
					roomScript.PlayerLeaves ();
				}
			}
		}
	}

	bool PlayerOnRightSide (GameObject player)
	{
		bool shouldReturnTrue = false;
		switch (doorwayDirection) {
		case Directions.down:
			if (player.transform.position.y > transform.position.y)
				shouldReturnTrue = true;
			break;
		case Directions.up:
			if (player.transform.position.y < transform.position.y)
				shouldReturnTrue = true;
			break;
		case Directions.left:
			if (player.transform.position.x > transform.position.x) {
				shouldReturnTrue = true;
				print ("Left : " + player.transform.position.x + " : " + transform.position.x);
			}
			break;
		case Directions.right:
			if (player.transform.position.x < transform.position.x) {
				shouldReturnTrue = true;
				print ("Right : " + player.transform.position.x + " : " + transform.position.x);
			}
			break;
		}
		return shouldReturnTrue;

	}

	void Update ()
	{
		//TODO if rotation, then the second room should move to fit.
	}

	IEnumerator coolDown ()
	{
		yield return new WaitForSeconds (0.75f);
		canEnter = true;
	}
}