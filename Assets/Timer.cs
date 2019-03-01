using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	// Use this for initialization

	public float waitTime;

	public Text timeRemaining;
    public Text countdownText;
    public CanvasGroup timeRemainingPanel;
    public CanvasGroup countdownPanel;

	public GameObject TimeOut;

	public playerArrowIce playerArrow;

    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public AudioClip sinisterMusic;
    private AudioClip countdownSound;
    private AudioClip finalCountSound;
    private AudioClip powerFailureSound;

    private DataCollector dataCollector;

    private float timer;

	private bool victory = false;
    private bool timerStarted = false;
    private bool sinisterMusicNotStarted = true;

    void Start () {
        playerArrow.disablePlayerControls();
        powerFailureSound = Resources.Load<AudioClip>("Audio/msfx_chrono_latency_hammer");
        countdownSound = Resources.Load<AudioClip>("Audio/Select02");
        finalCountSound = Resources.Load<AudioClip>("Audio/Select04");
        timer = waitTime;

        GameObject collectorObj = GameObject.Find("DataCollector");
        if (collectorObj != null)
        {
            dataCollector = collectorObj.GetComponent<DataCollector>();
        }

        startIntroAndCountdown();

    }
	
	// Update is called once per frame
	void Update () {
        if(timerStarted)
        {
            timer -= Time.deltaTime;

            float minutes = Mathf.Floor(timer / 60);
            float seconds = timer % 60;

            if (!victory)
            {
                if (minutes <= 0 && seconds <= 0)
                {
                    timerStarted = false;
                    timeRemaining.text = "Time Left:\n\n00:00";
                    audioSource.Stop();
                    audioSource.PlayOneShot(powerFailureSound);
                    TimeOut.SetActive(true);
                    if(dataCollector != null)
                    {
                        dataCollector.setOutcome("time");

                    }
                    playerArrow.disablePlayerControls();
                }
                else
                {
                    if(minutes <= 0 && sinisterMusicNotStarted)
                    {
                        sinisterMusicNotStarted = false;
                        audioSource.Stop();
                        audioSource.clip = sinisterMusic;
                        audioSource.Play();
                    }
                    timeRemaining.text = "Time Left:\n\n" + minutes.ToString("00") + ":" + seconds.ToString("00");
                }
            }
        }

	}

    public void startTimer()
    {
        timerStarted = true;
        audioSource.Stop();
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    private void startIntroAndCountdown()
    {
        StartCoroutine(doCountdownAndEnableControls());
    }

    private IEnumerator doCountdownAndEnableControls()
    {
        if (timeRemainingPanel != null)
        {
            StartCoroutine(introTimer());

            yield return new WaitForSeconds(3f);

            countdownPanel.alpha = 1;
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = "" + i;
                audioSource.PlayOneShot(countdownSound);
                yield return new WaitForSeconds(1f);

            }
            countdownText.text = "GO!";
            audioSource.PlayOneShot(finalCountSound);
            yield return new WaitForSeconds(1f);
            countdownPanel.alpha = 0;

            startTimer();
        }
        playerArrow.enablePlayerControls();

        audioSource.Play();

    }

    IEnumerator introTimer()
    {
        // move to center of screen
        RectTransform rrRectTimer = timeRemainingPanel.gameObject.GetComponent<RectTransform>();

        rrRectTimer.anchoredPosition = new Vector3(-15f, 50f, 0f); //move timer to center of screen

        timeRemainingPanel.alpha = 1; //show TimeRemainingPanel in center of screen

        Highlighter.Highlight(timeRemainingPanel.gameObject);
        yield return new WaitForSeconds(1f);
        Highlighter.Unhighlight(timeRemainingPanel.gameObject);

        // zoom panel to middle left
        Vector3 startPositionTimer = rrRectTimer.anchoredPosition;

        Vector3 endPositionTimer = new Vector3(-335f, 50f, 0f);

        float lerpTime = 0.5f;
        float currentLerpTime = 0f;

        while (Vector3.Distance(rrRectTimer.anchoredPosition, endPositionTimer) > 2)
        {
            rrRectTimer.anchoredPosition = Vector3.Lerp(startPositionTimer, endPositionTimer, currentLerpTime / lerpTime);
            currentLerpTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        rrRectTimer.anchoredPosition = endPositionTimer;
    }

    public void SetVictory () {
		victory = true;
	}

	public void ResetTimer () {
		timer = waitTime;
		victory = false;
	}

	public void Restart () {
        //TODO: see if this is being counted as a new attempt each time
		playerArrow.newGame();
		TimeOut.SetActive(false);
        timeRemainingPanel.alpha = 0; //hide TimeRemainingPanel

        timer = waitTime;
        float minutes = Mathf.Floor(timer / 60);
        float seconds = timer % 60;
        timeRemaining.text = "Time Left:\n\n" + minutes.ToString("00") + ":" + seconds.ToString("00");

        startIntroAndCountdown();
	}
}
