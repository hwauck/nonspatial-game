using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public AudioSource musicSource;
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
            dataCollector.gameQuit.AddListener(gameQuit);
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
                    musicSource.Stop();
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
                        musicSource.Stop();
                        musicSource.clip = sinisterMusic;
                        musicSource.Play();
                    }
                    timeRemaining.text = "Time Left:\n\n" + minutes.ToString("00") + ":" + seconds.ToString("00");
                }
            }
        }

	}

    public void startTimer()
    {
        timerStarted = true;
        musicSource.Stop();
        musicSource.clip = backgroundMusic;
        musicSource.Play();
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

        musicSource.Play();

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

    // called when gameQuit event is invoked from DataCollector
    public void gameQuit()
    {
        StopAllCoroutines();
        timerStarted = false;
        musicSource.Stop();
        countdownPanel.alpha = 0;
    }

    public void SetVictory () {
		victory = true;
        timerStarted = false;
        musicSource.Stop();
	}

	public void ResetTimer () {
		timer = waitTime;
		victory = false;
	}

    // When Restart button is clicked after running out of time
	public void Restart () {
        //TODO: see if this is being counted as a new attempt each time
        dataCollector.AddNewAttempt(SceneManager.GetActiveScene().name);
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
