using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int currentHealth;
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
    public void SetSlowed(bool slow)
    {
        NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();
        if (slow)
        {
            agent.speed = 1f;
        }
        else
        {
            agent.speed = 4f;
        }
    }
    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float time)
    {
        // Example: disable movement
        GetComponent<NavMeshAgent>().isStopped = true;
        yield return new WaitForSeconds(time);
        GetComponent<NavMeshAgent>().isStopped = false;
}

    void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object
        Destroy(gameObject);
    }
}
