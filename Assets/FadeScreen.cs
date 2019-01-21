using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour {
    private enum Fade { In, Out };
    public Image fadePanel;

    // Use this for initialization
    void Start () {
		
	}

    public void fadeOut(float timer)
    {
        StartCoroutine(fader(timer, Fade.Out));
    }

    public void fadeIn(float timer)
    {
        StartCoroutine(fader(timer, Fade.In));
    }

    IEnumerator fader(float timer, Fade fadeType)
    {
        float start, end;
        if (fadeType == Fade.In)
        {
            start = 1.0F;
            end = 0.0F;
        }
        else
        {
            start = 0.0F;
            end = 1.0F;
        }
        float i = 0.0F;
        float step = 1.0F / timer;

        while (i <= 1.0F)
        {
            i += step * Time.deltaTime;
            Color newColor = fadePanel.color;
            newColor.a = Mathf.Lerp(start, end, i);
            fadePanel.color = newColor;
            yield return new WaitForSeconds(step * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
