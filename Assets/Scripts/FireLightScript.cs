using UnityEngine;

public class FireLightScript : Singleton<FireLightScript>
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

    private void Start()
    {
        _component = fireLight.GetComponent<Light>();
        _torchHealth = MaxTorchHealth;
    }

    private void Update()
    {
        var percentage = (_torchHealth / MaxTorchHealth);
        if (percentage > 0.01)
        {
            _random = Random.Range(0.0f, 150.0f);
            var noise = Mathf.PerlinNoise(_random * percentage, Time.time);
            _component.intensity = Mathf.Lerp(MINIntensity * percentage,
                MAXIntensity * percentage, noise);
            fireParticle.transform.transform.localScale = new Vector3(
                DefaultScale * percentage,
                DefaultScale * percentage,
                DefaultScale * percentage);
        }
        else
        {
            GameManager.Instance.EndGame();
        }
    }


    public void SetRange(bool isRunning)
    {
        if (isRunning)
            _torchHealth -= 1 * Time.deltaTime;
        else
            _torchHealth = Mathf.Clamp(_torchHealth += 1 * Time.deltaTime, 0, MaxTorchHealth);
    }
}