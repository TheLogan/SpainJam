using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomSettings : MonoBehaviour {

	[SerializeField]
	SpriteRenderer[] roomSprites;

	[SerializeField]
	Color roomColour = Color.white;

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
	float rotationCountdown;

	void Start(){
		rotationCountdown = 0.5f;
		roomSprites = GetComponentsInChildren<SpriteRenderer>();

		if(startLighted)
			PlayerEnters();
	}

	public void PlayerEnters(){
		StartCoroutine(PlayerEntersInternal(Globals.PlayerGo));
	}

	IEnumerator PlayerEntersInternal(GameObject playerGO){
		print ("Apply options");
		//TODO if gravity is changed, so should jumpforce
		playerGO.GetComponent<Rigidbody2D>().gravityScale = gravity;
		Time.timeScale = timeScale;


		if(myCamGo != null)
			myCamGo.SetActive(true);

		float f = 1;
		while (f > 0){
			f -= Time.deltaTime;
//		}
//		while(true){
//			bool isDone = false;
			foreach (var sprite in roomSprites) {
				sprite.color = Color.Lerp(sprite.color, roomColour, Time.deltaTime);
//				if(sprite.color == Color.white)
//					isDone = true;
			}
//			if(isDone)
//				break;
			yield return new WaitForEndOfFrame();
		}
	}

	void FixedUpdate(){
		if(rotating){
			if(rotationCountdown <= 0){
				transform.Rotate(transform.forward * 0.2f);
			}else if(rotationCountdown > 0 ) {
				rotationCountdown -= Time.fixedDeltaTime;
			}
		}
	}

	public void PlayerLeaves(){
		foreach (var sprite in roomSprites) {
			sprite.color = Color.black;
		}
	}
}



