using UnityEngine;
using System.Collections;

public class TraversablePlatform : MonoBehaviour {

	[SerializeField]
	GameObject playerGo;

	[SerializeField]
	BoxCollider2D collider;

	void Update () {
		if(playerGo.transform.position.y - 0.9f < transform.position.y)
			collider.enabled = false;
		else
			collider.enabled = true;
	}
}
