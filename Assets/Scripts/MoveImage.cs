using UnityEngine;
using UnityEngine.UI;

public class MoveImage : MonoBehaviour
{
    private const float MFadeDuration = 3000.0f;
    private RawImage _mImg;

    [SerializeField] private bool mIgnoreTimeScale = true;

    private void Start()
    {
        _mImg = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _mImg.CrossFadeAlpha(0f, MFadeDuration, mIgnoreTimeScale);
        if (Input.GetMouseButtonDown(1))
            _mImg.CrossFadeAlpha(1f, MFadeDuration, mIgnoreTimeScale);
        transform.position += new Vector3(-0.2f, -0.01f);
    }
}