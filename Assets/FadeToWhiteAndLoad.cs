using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeToWhiteAndLoad : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeDuration = 2f;
    public string gameOverSceneName = "Ending";
    private bool hasFaded = false;

    private void Start()
    {
        SetAlpha(0f);
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
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(1f); // ensure full white
        yield return new WaitForSeconds(0.2f); // tiny buffer
        SceneManager.LoadScene(gameOverSceneName);
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
}

