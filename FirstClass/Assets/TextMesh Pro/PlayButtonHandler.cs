using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButtonHandler : MonoBehaviour
{
	public LoadingScreenManager gM;
	public string gameScene;
	public Image fade;

	public void PlayButton()
	{
        if (Time.timeSinceLevelLoad < 1.0f)
        {
			return;
        }
		if (gM.menuActive)
			return;

		StartCoroutine(PlayButtonAnim());
	}

	IEnumerator PlayButtonAnim()
    {
		gM.menuActive = true;
		gM.setPlane();

		yield return new WaitForSeconds(2.5f);

		Color fadeOpaque = fade.color;
		fadeOpaque.a = 1.0f;

		Color fadeTransparent = fade.color;
		fadeTransparent.a = 0.0f;

		float t = 0;
		while (t < 1)
        {
			t = Mathf.Clamp01(t + Time.deltaTime * 5.0f);
			fade.color = Color.Lerp(fadeTransparent, fadeOpaque, t);
			yield return null;
		}

		SceneManager.LoadSceneAsync(gameScene);
	}
}
