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
	public List<GameObject> availableRooms;

	[SerializeField]
	bool isLocked = false;


	Transform roomSpawnPoint;
	BoxCollider2D lockableWall;
	BoxCollider2D LockableWall{
		get{
			if(lockableWall == null){
				BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

				foreach (BoxCollider2D col in colliders) {
					if(!col.isTrigger)
						lockableWall = col;
				}
			}
			return lockableWall;
		}
	}

	BoxCollider2D triggerCollider;
	BoxCollider2D TriggerCollider{
		get{
			if(triggerCollider == null){
				BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
				
				foreach (BoxCollider2D col in colliders) {
					if(col.isTrigger)
						triggerCollider = col;
				}
				return triggerCollider;
			}
			return triggerCollider;
		}
	}

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
		print (colliders.Length);
		foreach (BoxCollider2D col in colliders) {
			print ("is trigger : " + col.isTrigger);
			if(col.isTrigger)
				triggerCollider = col;
			else
				lockableWall = col;
		}

		PlaceSpawnPoint();
	}

	void OnEnable(){
		selectedRoom = Random.Range (0, availableRooms.Count);
	}

	bool canEnter = true;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (!isLocked && canEnter) {
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

					print (selectedRoomGo);
					print (selectedRoomGo.GetComponent<RoomSettings>());
					print (doorwayDirection);
					print (selectedRoomGo.GetComponent<RoomSettings>().GetMatchingDoor(doorwayDirection));


					GameObject nextDoor = selectedRoomGo.GetComponent<RoomSettings>().GetMatchingDoor(doorwayDirection);
					selectedRoomSettings.PlayerEnteredDoorway = nextDoor;
					Globals.PlayerGo.GetComponent<CharacterControls>().MoveToRoom(gameObject, nextDoor);

					roomScript.PlayerLeaves ();
				}
			}else{
				LockableWall.enabled = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if(LockableWall.enabled && other.tag == "Player"){
			StartCoroutine(CoolDown());
		}
	}

	void Update(){
		if(!canEnter || isLocked){
			LockableWall.enabled = true;
		}else{
			LockableWall.enabled = false;
		}
	}

	bool PlayerOnRightSide (GameObject player) //FIXME I'm borked!!! Rotation should be taken into account
	{
		bool shouldReturnTrue = false;

		switch (doorwayDirection) {
		case Directions.Left:
			if (player.transform.position.x > transform.position.x + TriggerCollider.offset.x) {
				shouldReturnTrue = true;
			}
			break;
		case Directions.Right:
			if (player.transform.position.x < transform.position.x + TriggerCollider.offset.x) {
				shouldReturnTrue = true;
			}
			break;
		}
		return shouldReturnTrue;
	}


	IEnumerator CoolDown ()
	{
		canEnter = false;
		while(Vector3.Distance (transform.position, Globals.PlayerGo.transform.position) < 3.5f){
			yield return new WaitForEndOfFrame();
		}
		canEnter = true;
		LockableWall.enabled = false;
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

	public void Unlock(){
		isLocked = false;
	}
}
public enum Directions
{
	Left,
	Right,
	Up,
	Down
}