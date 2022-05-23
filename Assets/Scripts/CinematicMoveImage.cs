using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CinematicMoveImage : MonoBehaviour
{
    private const float FadeTime = 5;

    private readonly YieldInstruction _fadeInstruction = new();

    [SerializeField] private Image mImg;
    [SerializeField] private float moveX = -0.2f;
    [SerializeField] private float moveY = -0.1f;
    [SerializeField] private bool isFixed;

    [HideInInspector] public Cinematic cinematic;

    private bool IsActivated { get; set; }
    private Fade FadeState { get; set; }

    private void Update()
    {
        if (!IsActivated || isFixed)
        {
            return;
        }

        transform.position += new Vector3(moveX, moveY);
        if (transform.position.y > 300)
        {
            return;
        }

        if (FadeState == Fade.FadeOut)
        {
            StartCoroutine(FadeOut());
        }

        isFixed = true;
    }

    private IEnumerator FadeOut()
    {
        var elapsedTime = 0f;
        var color = mImg.color;
        while (elapsedTime < FadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            color.a = 1.0f - Mathf.Clamp01(elapsedTime / FadeTime);
            mImg.color = color;
            yield return _fadeInstruction;
        }

        NextSlide();
    }


    private IEnumerator FadeIn()
    {
        var elapsedTime = 0f;
        var color = mImg.color;
        while (elapsedTime < FadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(elapsedTime / FadeTime);
            mImg.color = color;
            yield return _fadeInstruction;
        }

        StartCoroutine(nameof(NextSlide), 2f);
    }

    public void Activate(Fade fadeNew)
    {
        IsActivated = true;
        FadeState = fadeNew;
        if (FadeState == Fade.FadeIn)
        {
            StartCoroutine(FadeIn());
        }
    }

    private void NextSlide() => cinematic.NextSlide();
}