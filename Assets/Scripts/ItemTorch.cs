using UnityEngine;
using static Enums.GameState;

public class ItemTorch : Singleton<ItemTorch>
{
    private const float DefaultScale = 2f;
    private const float MINIntensity = 2f;
    private const float MAXIntensity = 2.5f;
    private const float MaxTorchHealth = 5f;

    [SerializeField] private Light fireLight;
    [SerializeField] private ParticleSystem fireParticle;

    private Light TorchLight { get; set; }
    private float TorchHealth { get; set; }
    private float PercentageHealth => TorchHealth / MaxTorchHealth;

    private void Start()
    {
        TorchLight = fireLight.GetComponent<Light>();
        TorchHealth = MaxTorchHealth;
    }

    private void Update()
    {
        if (GameManager.Instance.State != Playing)
        {
            return;
        }

        UpdateTorchHealth();

        EndGameIfTorchIsDead();

        UpdateParticles();
    }

    private void UpdateParticles()
    {
        var random = Random.Range(0f, 150f);
        var noise = Mathf.PerlinNoise(random * PercentageHealth, Time.time);
        TorchLight.intensity = Mathf.Lerp(MINIntensity * PercentageHealth, MAXIntensity * PercentageHealth, noise);
        fireParticle.transform.localScale = Vector3.one * (DefaultScale * PercentageHealth);
    }

    private void EndGameIfTorchIsDead()
    {
        if (PercentageHealth <= 0.01 && GameManager.IsInitialized)
        {
            GameManager.Instance.UpdateGameState(LostTorch);
        }
    }


    private void UpdateTorchHealth() => TorchHealth = PlayerController.Instance.IsSprinting ? TorchHealth - Time.deltaTime : Mathf.Clamp(TorchHealth += Time.deltaTime, 0, MaxTorchHealth);
}