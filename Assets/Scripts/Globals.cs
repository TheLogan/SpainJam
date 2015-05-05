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

	static GameObject mainCamera;
	static public GameObject MainCamera {
		get{
			if(mainCamera == null)
				mainCamera = GameObject.Find("MultipurposeCameraRig");
			return mainCamera;
		}
	}
}
