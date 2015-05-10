using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class CharacterControls : MonoBehaviour {

	Transform moveFrom;
	Transform moveTo;
	SpriteRenderer spriteRenderer;
	Animator anim;
	bool playIdleAnim = false;

	[SerializeField]
	Platformer2DUserControl characterInputs;
	[SerializeField]
	PlatformerCharacter2D charContr;

	[SerializeField]
	AudioSource audioSource;

	[SerializeField]
	AudioClip deathSound;


	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
	}

	public void PlayerDied(){
		audioSource.PlayOneShot(deathSound);
	}

	public void MoveToRoom(GameObject oldDoor, GameObject nextDoor){
		moveFrom = oldDoor.transform;
		moveTo = nextDoor.transform;
		StopCoroutine(ForceMovePlayer());
		StartCoroutine(ForceMovePlayer());
	}

	IEnumerator ForceMovePlayer(){
		characterInputs.enabled = false;
		charContr.enabled = false;
		bool playerHitTarget = false;
		float f = 0;
		bool OldRoomGone = false;
		Globals.PlayerRigid.isKinematic = true;
		while(!playerHitTarget){
			f += Time.fixedDeltaTime;
			Globals.PlayerRigid.position = Vector3.Lerp (moveFrom.position, moveTo.position, f);

			float distanceToOld = Vector3.Distance(Globals.PlayerGo.transform.position, moveFrom.position);
			float distanceToNew = Vector3.Distance(Globals.PlayerGo.transform.position, moveTo.position);
			if(!OldRoomGone && distanceToOld > distanceToNew * 10){
				OldRoomGone = true;
				moveFrom.parent.GetComponent<RoomSettings>().PlayerHasLeft();
				moveTo.parent.GetComponent<RoomSettings>().PlayerHasEntered();
			}

			if(OldRoomGone && distanceToNew < 0.5f){
				playerHitTarget = true;
			}
			yield return new WaitForFixedUpdate();
		}
		Globals.PlayerRigid.isKinematic = false;
		characterInputs.enabled = true;
		charContr.enabled = true;
	}

	float idleEyesCount = 0;

	void Update(){
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("IdleClosed")){
			if(idleEyesCount <= 0){
				float val = Random.Range(0f,100f);

				if(val < 10){
					anim.SetBool("Eyes", false);
					idleEyesCount = Random.Range (0.25f, 0.5f);
				}else
					anim.SetBool("Eyes", true);
			}else{
				idleEyesCount -= Time.deltaTime;
			}
		}
	}
}
