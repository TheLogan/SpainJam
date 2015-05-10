using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

	[SerializeField]
	Image bg;
	[SerializeField]
	Image guy;

	IEnumerator Start () {
#if !UNITY_WEBPLAYER
		Screen.SetResolution(1596, 733, true);
#endif
		yield return new WaitForSeconds(2);
		float f = 0;
		while (true){
			f += Time.deltaTime * 2;
			Color c = Color.Lerp(Color.white, Color.black, f);
			bg.color = c;
			guy.color = c;

			yield return new WaitForEndOfFrame();
			if(f > 1)
				break;
		}
		Application.LoadLevel("GameLevel");
	}

}
