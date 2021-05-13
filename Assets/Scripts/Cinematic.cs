using UnityEngine;
using static Fade;

public class Cinematic : MonoBehaviour
{
    [SerializeField] private MoveImage[] images;
    [SerializeField] private Fade fade;
    private int index;

    private void Start()
    {
        foreach (var image in images)
        {
            if (image.GetType() == typeof(MoveImage))
            {
                image.cinematic = this;
            }
        }

        images[index].Activate(fade);
    }


    public void NextSlide()
    {
        index++;
        if (index < images.Length)
        {
            images[index].Activate(fade);
        }
        else
            UIManager.Instance.CinematicFinished(this);
    }
}

public enum Fade
{
    FadeIn,
    FadeOut
}