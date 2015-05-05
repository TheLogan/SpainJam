using UnityEngine;
using System.Collections;

public class CharacterControls : MonoBehaviour {

	Transform moveFrom;
	Transform moveTo;

	[SerializeField]
	UnityStandardAssets._2D.Platformer2DUserControl characterInputs;

	[SerializeField]
	UnityStandardAssets._2D.PlatformerCharacter2D charContr;

	public void MoveToRoom(GameObject oldDoor, GameObject nextDoor){
		moveFrom = oldDoor.transform;
		moveTo = nextDoor.transform;
		StopCoroutine(ForceMovePlayer());
		StartCoroutine(ForceMovePlayer());
	}

	IEnumerator ForceMovePlayer(){
//		characterInputs.enabled = false;
//		charContr.enabled = false;
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
			}

			if(OldRoomGone && distanceToNew < 0.5f){
				playerHitTarget = true;
			}
			yield return new WaitForFixedUpdate();
		}
		Globals.PlayerRigid.isKinematic = false;
//		characterInputs.enabled = true;
//		charContr.enabled = true;
	}
}
