using UnityEngine;

public class Cinematic : MonoBehaviour
{
    [SerializeField] private MoveImage[] images;
    [SerializeField] private Fade fade;
    private int _index;

    private void Start()
    {
        foreach (var image in images)
            if (image.GetType() == typeof(MoveImage))
                image.cinematic = this;

        images[_index].Activate(fade);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            UIManager.Instance.CinematicFinished(this);
    }

    public void NextSlide()
    {
        if (++_index < images.Length)
            images[_index].Activate(fade);
        else
            UIManager.Instance.CinematicFinished(this);
    }
}

public enum Fade
{
    FadeIn,
    FadeOut
}