using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public Image three;
    public Image two;
    public Image one;
    public Image start;

    public AudioSource lowMeow;
    public AudioSource lowMeow2;
    public AudioSource highMeow;

    public TextMeshProUGUI timerText;
    public float countdownTime = 3f;
    public System.Action onCountdownComplete;

    public CatSpawner catSpawner;
    public Timer timer;

    public void OnEnable()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(5f);
        three.gameObject.SetActive(false);
        two.gameObject.SetActive(false);
        one.gameObject.SetActive(false);
        start.gameObject.SetActive(false);

        lowMeow.Play();
        three.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        three.gameObject.SetActive(false);

        lowMeow2.Play();
        two.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        two.gameObject.SetActive(false);

        lowMeow.Play();
        one.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        one.gameObject.SetActive(false);

        highMeow.Play();
        start.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        start.gameObject.SetActive(false);
        // START
        //for (int i = (int)countdownTime; i > 0; i--)
        //{
        //    countdownText.text = i.ToString();
        //    yield return new WaitForSeconds(1f);
        //}

        //countdownText.text = "Start!";
        //yield return new WaitForSeconds(1f);

        //countdownText.gameObject.SetActive(false);

        onCountdownComplete?.Invoke();

        if (catSpawner != null)
        {
            timer.StartTime();
            catSpawner.StartSpawning();
        }
    }
}
