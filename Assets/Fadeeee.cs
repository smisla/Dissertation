using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Fadeeee : MonoBehaviour
{
    public Image fadeImage;
    public AudioSource audio;
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
        audio.Stop();
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
        while (audio.volume > 0f)
        {
            print("Volume if statement found");
            audio.volume = Mathf.Lerp(audio.volume, 0f, 1f * Time.deltaTime);
            yield return 0f;
        }
        audio.Stop();
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
