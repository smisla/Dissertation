using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    float elapsedTime;

    public Image fadeImage;
    public float fadeDuration = 2f;
    public string gameOverSceneName = "Ending";
    private bool hasFaded = false;

  

    private void Start()
    {
        SetAlpha(1f);
    }


    public void TriggerGameOver()
    {
        if (!hasFaded)
        {
            hasFaded = true;
            StartCoroutine(FadeOutAndLoad());
        }
    }

    IEnumerator FadeOutAndLoad()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f); // ensure full white
        yield return new WaitForSeconds(0.2f); // tiny buffer
        //SceneManager.LoadScene(gameOverSceneName);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }



    void Update()
    {
        StartTime();
    }

    public void StartTime()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
