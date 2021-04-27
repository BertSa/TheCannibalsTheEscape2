using UnityEngine;
using static GameManager.EndingStatus;

public class TorchScript : Singleton<TorchScript>
{
    private const float DefaultScale = 2.0f;
    private const float MINIntensity = 2f;
    private const float MAXIntensity = 3f;
    private const float MaxTorchHealth = 5f;

    [SerializeField] private Light fireLight;
    [SerializeField] private ParticleSystem fireParticle;

    private Light _component;
    private float _random;

    private float _torchHealth;
    private float _percentageHealth;


    private void Start()
    {
        _component = fireLight.GetComponent<Light>();
        _torchHealth = MaxTorchHealth;
    }

    private void Update()
    {
        if (Time.time == 0) return;
        _random = Random.Range(0.0f, 150.0f);
        var noise = Mathf.PerlinNoise(_random * _percentageHealth, Time.time);
        _component.intensity = Mathf.Lerp(MINIntensity * _percentageHealth,
            MAXIntensity * _percentageHealth, noise);
        fireParticle.transform.transform.localScale = Vector3.one * (DefaultScale * _percentageHealth);
    }


    public void SetRange(bool isRunning)
    {
        if (isRunning)
            _torchHealth -= 1 * Time.deltaTime;
        else
            _torchHealth = Mathf.Clamp(_torchHealth += 1 * Time.deltaTime, 0, MaxTorchHealth);

        _percentageHealth = (_torchHealth / MaxTorchHealth);
        if (_percentageHealth <= 0.01 && GameManager.IsInitialized) GameManager.Instance.EndGame(LostTorch);
    }
}