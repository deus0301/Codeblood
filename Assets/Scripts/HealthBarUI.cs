using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider playerHealth;
    public Slider enemyHealth;

    public PlayerData player;
    public Enemy enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerData>();
        playerHealth = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        enemyHealth = GameObject.Find("Boss Health").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy == null)
        {
            enemyHealth.gameObject.SetActive(false);
            try
            {
                enemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
            }
            catch { }
        }
        else if (enemy != null)
        {
            enemyHealth.gameObject.SetActive(true);
            enemyHealth.maxValue = enemy.maxHealth;
            enemyHealth.value = enemy.currentHealth;
        }
        if (player != null)
        {
            playerHealth.maxValue = player.maxHealth;
            playerHealth.value = player.currentHealth;
        }

        
    }
}
