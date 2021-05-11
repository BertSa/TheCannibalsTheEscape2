
public class CannibalsManager : Singleton<CannibalsManager>
{

    private EnemyFollow[] canibals;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        canibals = FindObjectsOfType<EnemyFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private enum CannibalsState
    {
        FOLLOWING, 
        SEARCHING, 
        ATTACKING
    }
}
