using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Doorway : MonoBehaviour
{
	/*
	 * detects player collision, and moves the player to the next room.
	 */

	[SerializeField]
	Directions doorwayDirection;
	[SerializeField]
	List<GameObject> availableRooms;


	BoxCollider2D lockableWall;
	BoxCollider2D triggerCollider;
	RoomSettings roomScript;
	int selectedRoom;
	bool isLinking = false;

	[SerializeField]
	Vector3 direction;

	public Directions DoorwayDirection {
		get{
			return doorwayDirection;
		}
	}

	void Start ()
	{
		selectedRoom = Random.Range (0, availableRooms.Count - 1); //TODO can this get to all the rooms, or should I remove the -1
		roomScript = transform.parent.GetComponent<RoomSettings> ();

		BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
		foreach (BoxCollider2D col in colliders) {
			if(col.isTrigger)
				triggerCollider = col;
			else
				lockableWall = col;
		}
	}

	bool canEnter = true;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (canEnter) {

			bool playerOnRightSide = PlayerOnRightSide (other.gameObject);

			if (playerOnRightSide) {
				if (other.gameObject == Globals.PlayerGo) {
					print ("player entered");
					StartCoroutine (CoolDown ());

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
					selectedRoomGo.SetActive(true);
					selectedRoomGo.transform.position = transform.parent.position + direction * 10;
					selectedRoomGo.GetComponent<RoomSettings> ().PlayerEnters ();
					Globals.MainCamera.GetComponent<CameraController> ().PlayerMovedRoom (selectedRoomGo);
					GameObject nextDoor = selectedRoomGo.GetComponent<RoomSettings>().GetMatchingDoor(doorwayDirection);
					print ("next door: " + nextDoor);
					print ("player: " + Globals.PlayerGo);
					Globals.PlayerGo.GetComponent<CharacterControls>().MoveToRoom(gameObject, nextDoor);

					roomScript.PlayerLeaves ();
				}
			}else{
				lockableWall.enabled = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if(lockableWall.enabled && other.tag == "Player"){
			lockableWall.enabled = false;
			StartCoroutine(CoolDown());
		}
	}

	bool PlayerOnRightSide (GameObject player)
	{
		bool shouldReturnTrue = false;
		switch (doorwayDirection) {
		case Directions.down:
			if (player.transform.position.y > transform.position.y + triggerCollider.offset.y)
				shouldReturnTrue = true;
			break;
		case Directions.up:
			if (player.transform.position.y < transform.position.y + triggerCollider.offset.y)
				shouldReturnTrue = true;
			break;
		case Directions.left:
			if (player.transform.position.x > transform.position.x + triggerCollider.offset.x) {
				shouldReturnTrue = true;
			}
			break;
		case Directions.right:
			if (player.transform.position.x < transform.position.x + triggerCollider.offset.x) {
				shouldReturnTrue = true;
			}
			break;
		}
		return shouldReturnTrue;
	}



	IEnumerator CoolDown ()
	{
		canEnter = false;
		yield return new WaitForSeconds (0.75f);
		canEnter = true;
	}
}
public enum Directions
{
	left,
	right,
	up,
	down
}