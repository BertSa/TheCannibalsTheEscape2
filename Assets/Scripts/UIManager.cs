using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Image _crosshair;
    [SerializeField] private Sprite crosshairImage;
    [SerializeField] private Color crosshairColor = Color.white;

    private void Awake()
    {
        if (!PlayerController.IsInitialized) return;
        _crosshair = PlayerController.Instance.GetComponentInChildren<Image>();
        _crosshair.sprite = crosshairImage;
        _crosshair.color = crosshairColor;
    }
}