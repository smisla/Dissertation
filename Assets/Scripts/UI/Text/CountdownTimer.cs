using UnityEngine;
using System.Collections;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownTime = 3f;
    public System.Action onCountdownComplete;

    public CatSpawner catSpawner;

    public void OnEnable()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        for (int i = (int)countdownTime; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Start!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        onCountdownComplete?.Invoke();

        if (catSpawner != null)
        {
            catSpawner.StartSpawning();
        }
    }
}
