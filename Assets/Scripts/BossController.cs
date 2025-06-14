using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    public Transform player;
    public float followRange = 75f; // Distance within which enemy will start following
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 100;

    private NavMeshAgent agent;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Optional: Automatically find player if not assigned
        if (player == null && GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

         if (distance <= attackRange)
        {
            // Stop moving to attack
            agent.ResetPath();

            // Attack if cooldown passed
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else if (distance <= followRange)
        {
            // Chase player
            agent.SetDestination(player.position);
        }
        else
        {
            // Idle
            agent.ResetPath();
        }
    }
    
    void Attack()
    {
        // Replace with actual attack logic or animation trigger
        Debug.Log("Enemy Attacks Player!");
        player.gameObject.TryGetComponent<PlayerData>(out PlayerData data);
        data.TakeDamage(attackDamage);

    }
}
