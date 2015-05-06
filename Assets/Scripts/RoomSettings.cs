using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoomSettings : MonoBehaviour
{

	[SerializeField]
	SpriteRenderer[]
		roomSprites;
	[SerializeField]
	Color roomColour = Color.white;
	List<Doorway> doorways;
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

		doorways = gameObject.GetComponentsInChildren<Doorway> ().ToList ();

		if (startLighted)
			PlayerEnters ();

		foreach (var item in roomSprites) {
			item.color = Color.black;
		}
	}

	void OnEnable(){
		StartedRotated = false;
		rotationCountdown = 0.75f;
		transform.rotation = Quaternion.Euler( Vector3.zero);
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
		//TODO if gravity is changed, so should jumpforce
		playerGO.GetComponent<Rigidbody2D> ().gravityScale = gravity;
		Time.timeScale = timeScale;


		if (myCamGo != null)
			myCamGo.SetActive (true);

		float f = 0;
		while (f < 1) {
			f += Time.deltaTime;
			foreach (var sprite in roomSprites) {
				sprite.color = Color.Lerp (Color.black, roomColour, f);
			}
			yield return new WaitForEndOfFrame ();
		}
	}

	public void PlayerHasEntered(){
		if(StartedRotated){
			transform.rotation = Quaternion.Euler(Vector3.zero);
			Globals.PlayerGo.transform.rotation = Quaternion.Euler(Vector3.zero);
			print(NextRoomSpawnPoint);
			Globals.PlayerGo.transform.position = PlayerEnteredDoorway.transform.position;
		}
	}
	

	void FixedUpdate ()
	{
		//TODO if rotation, then the second room should move to fit.
		if (rotating) {
			if (rotationCountdown <= 0) {
				transform.Rotate (transform.forward * 0.2f);
			} else if (rotationCountdown > 0) {
				rotationCountdown -= Time.fixedDeltaTime;
			}


			if(playerIsLeaving){
				NextRoom.rotation = transform.rotation;
				NextRoom.position = NextRoomSpawnPoint.position;
//				selectedRoomGo.transform.position = transform.parent.position + direction * 10;
			}
		}
	}

	public void PlayerLeaves ()
	{
		playerIsLeaving = true;
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
				sprite.color = Color.Lerp (roomColour, Color.black, f);
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

		GameObject go = doorways.FirstOrDefault (x => x.DoorwayDirection == wantedDirection).gameObject;
		if (go == null)
			Debug.LogError ("The opposite doorway does not exist");
			
		return go;
	}
} 

























