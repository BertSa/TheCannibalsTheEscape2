using UnityEngine;

public class FireLightScript : MonoBehaviour
{
    [Header("Light")] 
    [SerializeField] private float minIntensity = 2f;
    [SerializeField] private float maxIntensity = 3f;

    [SerializeField] private Light fireLight;

    float _random;

    void Update()
    {
        _random = Random.Range(0.0f, 150.0f);
        var noise = Mathf.PerlinNoise(_random, Time.time);
        fireLight.GetComponent<Light>().intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}