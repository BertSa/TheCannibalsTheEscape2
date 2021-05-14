using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveImage : MonoBehaviour
{
    [HideInInspector] public Cinematic cinematic;
    [SerializeField] private Image mImg;
    [SerializeField] private float moveX = -0.2f;
    [SerializeField] private float moveY = -0.1f;
    [SerializeField] private bool isFixed;

    private const float FadeTime = 5;
    private readonly YieldInstruction _fadeInstruction = new YieldInstruction();
    private bool _isActivated;
    private float _targetAlpha;
    private Fade _fade;

    private void Update()
    {
        if (!_isActivated || isFixed) return;

        transform.position += new Vector3(moveX, moveY);
        if (transform.position.y > 300) return;
        if (_fade == Fade.FadeOut) StartCoroutine(FadeOut());
        isFixed = true;
    }

    private IEnumerator FadeOut()
    {
        var elapsedTime = 0.0f;
        var c = mImg.color;
        while (elapsedTime < FadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            c.a = 1.0f - Mathf.Clamp01(elapsedTime / FadeTime);
            mImg.color = c;
            yield return _fadeInstruction;
        }

        NextSlide();
    }


    private IEnumerator FadeIn()
    {
        var elapsedTime = 0.0f;
        var c = mImg.color;
        while (elapsedTime < FadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(elapsedTime / FadeTime);
            mImg.color = c;
            yield return _fadeInstruction;
        }

        StartCoroutine(nameof(NextSlide), 2f);
    }

    public void Activate(Fade fadeNew)
    {
        _isActivated = true;
        _fade = fadeNew;
        if (_fade == Fade.FadeIn) StartCoroutine(FadeIn());
    }

    private void NextSlide()
    {
        cinematic.NextSlide();
    }
}