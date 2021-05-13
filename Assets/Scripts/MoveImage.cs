using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveImage : MonoBehaviour
{
    [HideInInspector] public Cinematic cinematic;

    [SerializeField] private Image mImg;
    [SerializeField] private float moveX = -0.2f;
    [SerializeField] private float moveY = -0.1f;
    private bool isFading;
    private bool isMoving;

    private void Update()
    {
        if (!isMoving) return;

        transform.position += new Vector3(moveX, moveY);
        print("1:"+ (transform.position.y > 300));
        print("2:"+isFading);
        if (isFading || (transform.position.y > 300)) return;
        StartCoroutine(FadeImage(true));
        isFading = false;
    }

    private IEnumerator FadeImage(bool fadeAway)
    {
        if (fadeAway)
        {
            for (float i = 5; i >= 0; i -= Time.unscaledDeltaTime)
            {
                mImg.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i <= 1; i += Time.unscaledDeltaTime)
            {
                mImg.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }

        cinematic.NextSlide();
    }

    public void Move()
    {
        isMoving = true;
    }
}