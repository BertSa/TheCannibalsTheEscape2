using UnityEngine;

public class Cinematic : MonoBehaviour
{
    [SerializeField] private CinematicMoveImage[] images;
    [SerializeField] private Fade fade;
    private int Index { get; set; }

    private void Start()
    {
        foreach (var image in images)
        {
            if (image.GetType() == typeof(CinematicMoveImage))
            {
                image.cinematic = this;
            }
        }

        images[Index].Activate(fade);
    }

    private void Update()
    {
        CheckSkipFirstCinematic();
    }

    private void CheckSkipFirstCinematic()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            UIManager.Instance.CinematicFinished(this);
        }
    }

    public void NextSlide()
    {
        if (++Index >= images.Length)
        {
            UIManager.Instance.CinematicFinished(this);
            return;
        }

        images[Index].Activate(fade);
    }
}

public enum Fade
{
    FadeIn,
    FadeOut,
}