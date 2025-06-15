using System;
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
    private GameObject bossInstance;

    private Animator animator;
    BossAttackType currentAttack;

    [Header("AOE Settings")]
    public float aoeRadius = 5f;
    public int aoeDamage = 50;


    [Header("Projectile Settings")]
    public GameObject proj;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 15f;
    public int projectileDamage = 40;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        projectileSpawnPoint = GameObject.Find("ThrowPoint").transform;
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
                switch (currentAttack) 
                {
                    case BossAttackType.Melee: 
                        if (distance <= attackRange)
                            Attack(); 
                        break;
                    case BossAttackType.AoE: 
                        AOE(); 
                        break;
                    case BossAttackType.Projectile: 
                        Projectile(); 
                        break;
                }

                lastAttackTime = Time.time;
            }
        }
        else if (distance <= followRange)
        {
            // Chase player
            animator.SetBool("IsWalking", true);
            agent.SetDestination(player.position);
        }
        else
        {
            // Idle
            animator.SetBool("IsWalking", false);
            agent.ResetPath();
        }
    }
    public void SetNextAttack(int actionId)
    {
        if (Enum.IsDefined(typeof(BossAttackType), actionId))
        {
            currentAttack = (BossAttackType)actionId;
            Debug.Log("Boss next attack set to: " + currentAttack);
        }
        else
        {
            Debug.LogWarning("Invalid attack ID: " + actionId);
        }
    }
    void Attack()
    {
        // Replace with actual attack logic or animation trigger
        Debug.Log("Enemy Attacks Player!");
        animator.Play("Attack");
        player.gameObject.TryGetComponent<PlayerData>(out PlayerData data);
        data.TakeDamage(attackDamage);

    }
    void AOE()
    {
        Debug.Log("Enemy performed AOE Damage!");
        animator.Play("AOE");

        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerData>(out PlayerData player))
            {
                player.TakeDamage(aoeDamage);
            }
        }
    }
    void Projectile()
    {
        Debug.Log("Enemy threw projectile");
        animator.Play("Throw");

        if (proj != null && player != null)
        {
            GameObject projectile = Instantiate(proj, projectileSpawnPoint.position, Quaternion.identity);

            Vector3 dir = (player.position - projectileSpawnPoint.position).normalized;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.linearVelocity = dir * projectileSpeed;

            BossProjectile projScript = projectile.GetComponent<BossProjectile>();
            if (projScript != null)
            {
                projScript.damage = projectileDamage;
            }
        }
    }
    public void SetBossInstance(GameObject instance)
    {
        bossInstance = instance;
        agent = bossInstance.GetComponent<NavMeshAgent>();
    }
    enum BossAttackType { Melee = 0, AoE = 1, Projectile = 2 }
}
