using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }
    void Update()
    {
        if (transform.position.y <= -1)
        {
            GameOver();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        { GameOver(); }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        
    }
}
