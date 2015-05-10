using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityStandardAssets._2D;

public class RoomSettings : MonoBehaviour
{

	[SerializeField]
	SpriteRenderer[]
		roomSprites;
	[SerializeField]
	Color roomColour = Color.white;
	List<Doorway> doorways;
	List<Doorway> Doorways{
		get{
			if(doorways == null || doorways.Count == 0){
				doorways = gameObject.GetComponentsInChildren<Doorway> ().ToList ();
			}
			return doorways;
		}
	}

	[SerializeField]
	GameObject player;
	[SerializeField]
	bool startLighted;
	[SerializeField]
	GameObject myCamGo;
	[SerializeField]
	float gravity = 3f;
	[SerializeField]
	float timeScale = 1;
	[SerializeField]
	bool rotating = false;

	[SerializeField]
	Color vanishColour;

	public bool Rotating {
		get{
			return rotating;
		}
	}

	float rotationCountdown;

	bool playerIsLeaving = false;

	public bool StartedRotated {
		get;
		set;
	}

	public Transform NextRoom {
		get;
		set;
	}

	public Transform NextRoomSpawnPoint {
		get;
		set;
	}

	public GameObject PlayerEnteredDoorway {
		get;
		set;
	}


	void Start ()
	{
		rotationCountdown = 0.5f;
		roomSprites = GetComponentsInChildren<SpriteRenderer> ();

		if (startLighted)
			PlayerEnters ();

		foreach (var item in roomSprites) {
			item.color = Color.black;
		}
	}

	void OnEnable(){
		StartedRotated = false;
		rotationCountdown = 0.75f;
//		transform.rotation = Quaternion.Euler( Vector3.zero);
		playerIsLeaving = false;
		NextRoom = null;
		NextRoomSpawnPoint = null;
	}

	public void PlayerEnters ()
	{
		StartCoroutine (PlayerEntersInternal (Globals.PlayerGo));
	}

	IEnumerator PlayerEntersInternal (GameObject playerGO)
	{
		ApplyEffectsToPlayer();
		float f = 0;
		while (f < 1) {
			f += Time.deltaTime;
			foreach (var sprite in roomSprites) {
				sprite.color = Color.Lerp (vanishColour, roomColour, f);
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	public void PlayerHasEntered(){
		if(StartedRotated){
			transform.rotation = Quaternion.Euler(Vector3.zero);
			Globals.PlayerGo.transform.rotation = Quaternion.Euler(Vector3.zero);
			Globals.PlayerGo.transform.position = PlayerEnteredDoorway.transform.position;
		}
	}

	void ApplyEffectsToPlayer(){
		//TODO if gravity is changed, so should jumpforce
		Globals.PlayerGo.GetComponent<Rigidbody2D> ().gravityScale = gravity;
		Time.timeScale = timeScale;
	}
	

	void FixedUpdate ()
	{
		if (rotating) {
			if (rotationCountdown <= 0) {
				transform.Rotate (transform.forward * 0.2f);
			} else if (rotationCountdown > 0) {
				rotationCountdown -= Time.fixedDeltaTime;
			}


			if(playerIsLeaving){
				NextRoom.rotation = transform.rotation;
				NextRoom.position = NextRoomSpawnPoint.position;
			}
		}
	}

	public void PlayerLeaves ()
	{
		playerIsLeaving = true;
		gameObject.SetActive(true);
		StartCoroutine (PlayerLeavesInternal ());
	}

	public void PlayerHasLeft ()
	{
		gameObject.SetActive (false);
		transform.position = new Vector3 (Random.Range (-10000, 10000), 10000, 10000);
	}

	IEnumerator PlayerLeavesInternal ()
	{
		float f = 0;

		while (f < 1) {
			f += Time.deltaTime;
			foreach (var sprite in roomSprites) {
//				sprite.color = Color.Lerp (roomColour, Color.black, f);
				sprite.color = Color.Lerp (roomColour, vanishColour, f);
				
			}
			yield return new WaitForEndOfFrame ();
		}
	}



	public GameObject GetMatchingDoor (Directions doorDirection)
	{
		Directions wantedDirection = Directions.Down;

		switch (doorDirection) {
		case Directions.Down:
			wantedDirection = Directions.Up;
			break;
		case Directions.Up:
			wantedDirection = Directions.Down;
			break;
		case Directions.Left:
			wantedDirection = Directions.Right;
			break;
		case Directions.Right:
			wantedDirection = Directions.Left;
			break;
		}

		GameObject go = Doorways.FirstOrDefault (x => x.DoorwayDirection == wantedDirection).gameObject;
		if (go == null)
			Debug.LogError ("The opposite doorway does not exist");
		return go;
	}

	public void RoomCleared(){
		StartCoroutine(RoomClearedInternal());
	}

	[SerializeField]
	List<Doorway> lockedDoors;

	IEnumerator RoomClearedInternal(){
		Globals.PlayerRigid.isKinematic = true;
		Globals.PlayerGo.GetComponent<UnityStandardAssets._2D.Platformer2DUserControl>().enabled = false;
		float f = 1;

		//Clear all references to this room.

		List<Doorway> doorwaysToHere = GameObject.FindGameObjectsWithTag("Doorway")
										   	.Select(x => x.GetComponent<Doorway>())
											.Where(x => x.availableRooms.Contains(gameObject)).ToList();

		foreach (var door in doorwaysToHere) {
			door.availableRooms.Remove(gameObject);
		}

		foreach (var door in lockedDoors) {
			door.Unlock();
		}

		while(f > 0){
			f -= Time.fixedDeltaTime * 0.5f;
			transform.rotation = Quaternion.Lerp(Quaternion.Euler (Vector3.zero), transform.rotation, f);
			if(transform.eulerAngles.z < 0.1f || transform.eulerAngles.z > 359.9f)
				break;

			yield return new WaitForFixedUpdate();
		}

		Globals.PlayerGo.GetComponent<Platformer2DUserControl>().enabled = true;
		transform.rotation = Quaternion.Euler(Vector3.zero);
		Globals.PlayerRigid.isKinematic = false;
		gravity = 3;
		timeScale = 1;
		ApplyEffectsToPlayer();
	}


	public void PlayerDied(){
		print("Player Died!");
		Globals.PlayerGo.transform.position = PlayerEnteredDoorway.transform.position;
	}
} 

























