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



	Transform roomSpawnPoint;
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
		roomScript = transform.parent.GetComponent<RoomSettings> ();

		BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
		foreach (BoxCollider2D col in colliders) {
			if(col.isTrigger)
				triggerCollider = col;
			else
				lockableWall = col;
		}

		PlaceSpawnPoint();
	}

	void OnEnable(){
		selectedRoom = Random.Range (0, availableRooms.Count);
//		canEnter 
	}

	bool canEnter = true;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (canEnter) {
			StartCoroutine(CoolDown());
			bool playerOnRightSide = PlayerOnRightSide (other.gameObject);

			if (playerOnRightSide) {
				if (other.gameObject == Globals.PlayerGo) {

					canEnter = false;

					var selectedRoomGo = availableRooms [selectedRoom];
					Vector3 direction = Vector3.zero;
					switch (doorwayDirection) {
					case Directions.Down:
						direction = -transform.up;
						break;
					case Directions.Left:
						direction = -transform.right;
						break;
					case Directions.Right:
						direction = transform.right;
						break;
					case Directions.Up:
						direction = transform.up;
						break;
					}
					selectedRoomGo.SetActive(true);
					selectedRoomGo.transform.position = roomSpawnPoint.position;

					roomScript.NextRoom = selectedRoomGo.transform;
					roomScript.NextRoomSpawnPoint = roomSpawnPoint;
					RoomSettings selectedRoomSettings = selectedRoomGo.GetComponent<RoomSettings> ();
					selectedRoomSettings.PlayerEnters ();
					if(roomScript.Rotating){
						selectedRoomSettings.StartedRotated = true;
					}
					Globals.MainCamera.GetComponent<CameraController> ().PlayerMovedRoom (selectedRoomGo);
					GameObject nextDoor = selectedRoomGo.GetComponent<RoomSettings>().GetMatchingDoor(doorwayDirection);
					selectedRoomSettings.PlayerEnteredDoorway = nextDoor;
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
			StartCoroutine(CoolDown());
		}
	}

	void Update(){
		if(!canEnter){
			lockableWall.enabled = true;
		}else{
			lockableWall.enabled = false;
		}
	}

	bool PlayerOnRightSide (GameObject player) //FIXME I'm borked!!!
	{
		bool shouldReturnTrue = false;
		switch (doorwayDirection) {
		case Directions.Down: 
			if (player.transform.position.y > transform.position.y + triggerCollider.offset.y)
				shouldReturnTrue = true;
			break;
		case Directions.Up:
			if (player.transform.position.y < transform.position.y + triggerCollider.offset.y)
				shouldReturnTrue = true;
			break;
		case Directions.Left:
			if (player.transform.position.x > transform.position.x + triggerCollider.offset.x) {
				shouldReturnTrue = true;
			}
			break;
		case Directions.Right:
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
//		yield return new WaitForSeconds (0.75f);
		while(Vector3.Distance (transform.position, Globals.PlayerGo.transform.position) < 1.5f){
			yield return new WaitForEndOfFrame();
		}
		canEnter = true;
		lockableWall.enabled = false;
	}

	void PlaceSpawnPoint ()
	{
		GameObject go = new GameObject("Spawn Point");
		go.transform.parent = transform;

		switch (doorwayDirection) {
		case Directions.Down:
			go.transform.position = transform.parent.position - transform.up * Globals.RoomDistance;
			break;
		case Directions.Up:
			go.transform.position = transform.parent.position + transform.up * Globals.RoomDistance;
			break;
		case Directions.Left:
			go.transform.position = transform.parent.position - transform.right * Globals.RoomDistance;
			break;
		case Directions.Right:
			go.transform.position = transform.parent.position + transform.right * Globals.RoomDistance;
			break;
		}

		roomSpawnPoint = go.transform;
	}
}
public enum Directions
{
	Left,
	Right,
	Up,
	Down
}