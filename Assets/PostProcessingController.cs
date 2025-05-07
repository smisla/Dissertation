using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    public Volume postProcessingVolume;

    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    private ChromaticAberration chroma;

    private float targetRed = 0.4f;
    private float targetVignette = 0.85f;
    private float targetChroma = 1f;

    private Color warningColor = new Color(1f, 0.7f, 0.7f);

    private float currentProgress = 0f;
    public float CurrentProgress => currentProgress;

    private void Start()
    {
        if (postProcessingVolume.profile.TryGet(out colorAdjustments) &&
            postProcessingVolume.profile.TryGet(out vignette) && postProcessingVolume.profile.TryGet(out chroma))
        {
            colorAdjustments.colorFilter.value = Color.white;
            vignette.intensity.value = 0f;
            chroma.intensity.value = 0f;
        }
        else
        {
            Debug.LogError("Missing ColorAdjustments or LensDistortion in Volume Profile");
        }
    }

    public void UpdateEffectProgress(float t)
    {
        currentProgress = Mathf.Clamp01(t);
        if (colorAdjustments != null)
        {
            Color color = Color.Lerp(Color.white, Color.red, t);
            colorAdjustments.colorFilter.value = color;
        }

        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(0f, targetVignette, t);
        }

        if (chroma != null)
        {
            chroma.intensity.value = Mathf.Lerp(0f, targetChroma, t);
        }
    }

    public void ResetEffects()
    {
        UpdateEffectProgress(0f);
    }


    //public void ResetEffects(float tr)
    //{
    //    if (colorAdjustments != null)
    //    {
    //        Color color = Color.Lerp(Color.red, Color.white, tr);
    //        colorAdjustments.colorFilter.value = color;
    //    }

    //    if (vignette != null)
    //    {
    //        vignette.intensity.value = Mathf.Lerp(targetVignette, 0f, tr);
    //    }

    //    if (chroma != null)
    //    {
    //        chroma.intensity.value = Mathf.Lerp(targetChroma, 0f, tr);
    //    }
    //}
}

