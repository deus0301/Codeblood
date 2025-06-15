using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int currentHealth;
    public int maxHealth;
    float freezeDuration;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        { Death(); }
    }
    public void Freeze(float duration)
    {
        freezeDuration = duration;
        StartCoroutine("Frozen");
    }

    IEnumerator Frozen()
    {
        gameObject.GetComponent<BossController>().enabled = false;
        yield return new WaitForSeconds(freezeDuration);
        gameObject.GetComponent<BossController>().enabled = true;
    }

    void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object
        Destroy(gameObject);
    }
}
