using UnityEngine;

public class Cinematic : MonoBehaviour
{
    [SerializeField] private MoveImage[] images;
    private int index;

    private void Start()
    {
        foreach (var image in images) image.cinematic = this;
        images[index].Move();
    }


    public void NextSlide()
    {
        images[index].gameObject.SetActive(false);
        index++;
        if (index < images.Length) images[index].Move();
    }
}