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
	Color
		roomColour = Color.white;
	List<Doorway> doorways;
	[SerializeField]
	GameObject
		player;
	[SerializeField]
	bool
		startLighted;
	[SerializeField]
	GameObject
		myCamGo;
	[SerializeField]
	float
		gravity = 3f;
	[SerializeField]
	float
		timeScale = 1;
	[SerializeField]
	bool
		rotating = false;
	float rotationCountdown;

	void Start ()
	{
		rotationCountdown = 0.5f;
		roomSprites = GetComponentsInChildren<SpriteRenderer> ();

		doorways = gameObject.GetComponentsInChildren<Doorway> ().ToList ();

		if (startLighted)
			PlayerEnters ();
	}

	public void PlayerEnters ()
	{
		StartCoroutine (PlayerEntersInternal (Globals.PlayerGo));
	}

	IEnumerator PlayerEntersInternal (GameObject playerGO)
	{
		print ("Apply options");
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

	void FixedUpdate ()
	{
		if (rotating) {
			if (rotationCountdown <= 0) {
				transform.Rotate (transform.forward * 0.2f);
			} else if (rotationCountdown > 0) {
				rotationCountdown -= Time.fixedDeltaTime;
			}
		}
	}

	public void PlayerLeaves ()
	{
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
		Directions wantedDirection = Directions.down;

		switch (doorDirection) {
		case Directions.down:
			wantedDirection = Directions.up;
			break;
		case Directions.up:
			wantedDirection = Directions.down;
			break;
		case Directions.left:
			wantedDirection = Directions.right;
			break;
		case Directions.right:
			wantedDirection = Directions.left;
			break;
		}

		GameObject go = doorways.FirstOrDefault (x => x.DoorwayDirection == wantedDirection).gameObject;
		if (go == null)
			Debug.LogError ("The opposite doorway does not exist");
			
		return go;
	}

	void Update ()
	{
		//TODO if rotation, then the second room should move to fit.
	}
} 

























