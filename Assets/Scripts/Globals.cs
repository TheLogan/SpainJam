using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

	static GameObject playerGo;
	static public GameObject PlayerGo {
		get{
			if(playerGo == null)
				playerGo = GameObject.FindGameObjectWithTag("Player");
			return playerGo;
		}
	}

	static CharacterControls playerScript;
	static public CharacterControls PlayerScript{
		get{
			if(playerScript == null)
				playerScript = playerGo.GetComponent<CharacterControls>();
			return playerScript;
		}
	}

	static Rigidbody2D playerRigid;
	static public Rigidbody2D PlayerRigid{
		get{
			if(playerRigid == null)
				playerRigid = playerGo.GetComponent<Rigidbody2D>();
			return playerRigid;
		}
	}

	static float roomDistance = 10;
	static public float RoomDistance{
		get{
			return roomDistance;
		}
	}



	static GameObject mainCamera;
	static public GameObject MainCamera {
		get{
			if(mainCamera == null)
				mainCamera = GameObject.Find("MultipurposeCameraRig");
			return mainCamera;
		}
	}
}
