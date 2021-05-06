using System;
using UnityEngine;
using UnityEngine.UI;

public class MoveImage : MonoBehaviour
{
    private RawImage m_img;
    private float mFadeDuration = 3000.0f;

    [SerializeField] private bool m_ignoreTimeScale=true;

    private void Start()
    {
        m_img = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            m_img.CrossFadeAlpha(0f, mFadeDuration, m_ignoreTimeScale);
        if (Input.GetMouseButtonDown(1))
            m_img.CrossFadeAlpha(1f, mFadeDuration, m_ignoreTimeScale);
        transform.position += new Vector3(-0.2f, -0.01f);
    }
}