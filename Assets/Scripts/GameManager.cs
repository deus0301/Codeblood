using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public WebSocketManager wsManager;
    private GameObject bossInstance;
    public BossController boss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        wsManager = GameObject.Find("WebSocketManager").GetComponent<WebSocketManager>();
        Scene currentScene = SceneManager.GetActiveScene();
    }
    void OnDestroy()
    {
        if (wsManager != null && boss != null)
        wsManager.OnActionReceived -= boss.SetNextAttack;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetBossInstance(GameObject instance)
    {
        bossInstance = instance;
        boss = bossInstance.GetComponent<BossController>();
        wsManager.OnActionReceived += boss.SetNextAttack;
    }
}
