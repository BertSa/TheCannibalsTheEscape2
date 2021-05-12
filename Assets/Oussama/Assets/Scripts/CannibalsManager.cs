using UnityEngine;
using static CannibalsManager.CannibalsState;
using static SoundManager;

public class CannibalsManager : Singleton<CannibalsManager>
{
    public enum CannibalsState
    {
        /// <summary>
        ///     when cannibals are following the player
        /// </summary>
        Following,

        /// <summary>
        ///     when cannibals are searching the player
        /// </summary>
        Searching
    }

    [SerializeField] private CannibalsState state = Following;
    [HideInInspector] public EventAmbiance onAmbianceChanged;

    private EnemyFollow[] cannibals;

    private Transform player;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        cannibals = FindObjectsOfType<EnemyFollow>();
        player = PlayerController.Instance.GetComponent<Transform>();
        SetState(Following);
    }

    private void Update()
    {
        foreach (var c in cannibals)
        {
            var cannibalPosition = c.transform;
            var dirFromAtoB = (player.transform.position - cannibalPosition.position).normalized;
            var dotProd = Vector3.Dot(dirFromAtoB, cannibalPosition.forward);

            var lookingAtPlayer = dotProd >= 0 && dotProd <= 1;

            if (lookingAtPlayer)
            {
                SetState(Following);
                return;
            }

            SetState(Searching);
            return;
        }
    }


    public void SetState(CannibalsState actual)
    {
        var previous = state;
        state = actual;

        onAmbianceChanged.Invoke(previous, actual);
    }
}