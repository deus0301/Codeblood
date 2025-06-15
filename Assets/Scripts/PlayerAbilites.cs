using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilites : MonoBehaviour
{
    public const string ABILITY = "Ability";
    public const string ULTIMATE = "Ultimate";

    public WeaponData currentWeapon;
    public GameObject cam;
    public BossController boss;

    public float normalCooldown = 5f;
    public float ultimateCooldown = 60f;

    private bool canUseNormal = true;
    private bool canUseUltimate = true;

    private Queue<string> recentMove = new Queue<string>();
    [SerializeField] private WebSocketManager wsManager;
    private int maxStoredMoves = 1;
    public LayerMask enemyLayer;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        wsManager = GameObject.Find("WebSocketManager").GetComponent<WebSocketManager>();
    }

    void Update()
    {
        if (TryGetComponent<WeaponBehaviour>(out WeaponBehaviour T))
        {
            if (T.data != null)
            {
                currentWeapon = T.data;
            }
        }
        try
        {
            if(GameObject.FindWithTag("Enemy").TryGetComponent<BossController>(out BossController E))
                boss = E;
        }
        catch
        {

        }
    }

    public void Ability()
    {
        if (canUseNormal)
        {
            StartCoroutine("UseAbility");
        }
        else
        {
            Debug.Log("Cant Use shit");
        }
    }
    public void Ultimate()
    {
        if (canUseUltimate)
        {
            StartCoroutine("UseUltimate");
        }
        else
        {
            Debug.Log("Cant Use shit");
        }
    }

    void CastAbility(ElementType type)
    {
        switch (type)
        {
            case ElementType.Ice:
                FrostSlam();
                Debug.Log("Casting Frost Slam");
                break;

            case ElementType.Fire:
                Debug.Log("Casting Flame Whip");
                StartCoroutine("FlameWhip");
                break;

            case ElementType.Earth:
                Debug.Log("Casting Stonewall Slash");
                break;

            case ElementType.Water:
                Debug.Log("Casting Piercing Stream");
                break;

            case ElementType.Air:
                Debug.Log("Casting Gale Dash");
                break;

            case ElementType.Light:
                Debug.Log("Casting Photon Burst");
                break;
        }
    }
    void CastUltimate(ElementType type)
    {
        switch (type)
        {
            case ElementType.Ice:
                Debug.Log("Casting Glacial Tomb");
                break;

            case ElementType.Fire:
                Debug.Log("Casting Infernal Maelstrom");
                break;

            case ElementType.Earth:
                Debug.Log("Casting Terra Shatter");
                break;

            case ElementType.Water:
                Debug.Log("Casting Tidal Barrage");
                break;

            case ElementType.Air:
                Debug.Log("Casting Storm Pulse");
                break;

            case ElementType.Light:
                Debug.Log("Casting Solar Nova");
                break;
        }
    }
    IEnumerator UseAbility()
    {
        canUseNormal = false;
        CastAbility(currentWeapon.elementType);
        TrackMove("ability");
        yield return new WaitForSeconds(normalCooldown);
        canUseNormal = true;
    }

    IEnumerator UseUltimate()
    {
        canUseUltimate = false;
        CastUltimate(currentWeapon.elementType);
        TrackMove("ultimate");
        yield return new WaitForSeconds(ultimateCooldown);
        canUseUltimate = true;
    }

    public void TrackMove(string moveName)
    {
        if (recentMove.Count >= maxStoredMoves) recentMove.Dequeue();
        recentMove.Enqueue(moveName);
        Debug.Log("Move Tracked: " + moveName);
        int attackNumber = 0;
        switch(moveName)
        {
            case "normal":
                attackNumber = 0; 
                break;
            case "ability":
                attackNumber = 1; 
                break;
            case "ultimate":
                attackNumber = 2; 
                break;
        }

        if (recentMove.Count == maxStoredMoves)
        {
            Debug.Log("Moves sent to Enemy AI");
            var testData = new TestMessage()
            {
                distance = (int)(transform.position.magnitude - boss.gameObject.transform.position.magnitude),
                player_health = gameObject.GetComponent<PlayerData>().currentHealth,
                boss_health = boss.gameObject.GetComponent<Enemy>().currentHealth,
                skill_ready = true,
                action_taken = attackNumber,
            };

            string json = JsonUtility.ToJson(testData);
            wsManager.SendMessageToServer(json);
            Debug.Log("Sent message: " + json);

            recentMove.Clear(); // Optional
        }
    }

    [Serializable]
    public class TestMessage
    {
        public int distance;
        public int player_health;
        public int boss_health;
        public bool skill_ready;
        public int action_taken;
    }

    void FrostSlam()
    {
        int damage = 200;
        float freezeDuration = 5;
        float radius = 7.5f;
        Collider[] hit = Physics.OverlapSphere(transform.position, radius);
        foreach (var enemy in hit)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
                enemy.GetComponent<Enemy>().Freeze(freezeDuration);
            }
        }
    }
    void GlacialTomb()
    {
        int damage = 500;
        float freezeDuration = 20f;
        float radius = 10f;
        Collider[] hit = Physics.OverlapSphere(transform.position, radius);
        foreach (var enemy in hit)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
                enemy.GetComponent<Enemy>().Freeze(freezeDuration);
            }
        }
    }

    float range = 5f;
    float radius = 1.5f;
    int dmg = 100;
    int burndmg = 10;
    float burnDuration = 5f;
    float burnInterval = 1f;

    IEnumerator FlameWhip()
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        Debug.DrawRay(origin, direction * range, Color.red, 2f);

        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, range, enemyLayer))
        {
            if (hit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Debug.Log("Flame Whip hit ");
                enemy.TakeDamage(dmg);

                // Apply burn
                StartCoroutine(ApplyBurn(enemy, burndmg, burnDuration, burnInterval));
            }
        }   

        yield return null;
    }
    IEnumerator ApplyBurn(Enemy target, int dmg, float duration, float tickRate)
{
    float timer = 0f;

    while (timer < duration)
    {
        if (target == null) yield break;

        target.TakeDamage(dmg);
        Debug.Log("Burn tick on " + target.name);

        timer += tickRate;
        yield return new WaitForSeconds(tickRate);
    }
}
}

/***

## ⚔️ ELEMENTAL CHARACTER ABILITIES

---

### 🧊 **ICE – Axe**

| Ability                     | Description                                                                                             |
| --------------------------- | ------------------------------------------------------------------------------------------------------- |
| **Frost Slam** (Normal)     | Slam the axe into the ground, freezing enemies in a small radius for 1.5s and dealing damage.           |
| **Glacial Tomb** (Ultimate) | Summon massive ice spikes from below, freezing all enemies in a wide area and dealing heavy AoE damage. |

💡 *Theme*: Slow enemies down, control space.

---

### 🔥 **FIRE – Chains**

| Ability                           | Description                                                                                                 |
| --------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| **Flame Whip** (Normal)           | Lash forward with chains, dealing damage and igniting enemies for 3 seconds.                                |
| **Infernal Maelstrom** (Ultimate) | Spin chains in a fiery circle, pulling enemies in and setting the ground ablaze for DOT (damage over time). |

💡 *Theme*: Aggression, burn over time, pull enemies.

---

### 🌱 **EARTH – Sword**

| Ability                      | Description                                                                                               |
| ---------------------------- | --------------------------------------------------------------------------------------------------------- |
| **Stonewall Slash** (Normal) | Slash the ground to raise a short stone wall that blocks enemy movement or projectiles.                   |
| **Terra Shatter** (Ultimate) | Charge up and smash the sword down, causing the ground to quake and rupture in a line, launching enemies. |

💡 *Theme*: Defense + heavy knockback/CC (crowd control).

---

### 🌊 **WATER – Bow**

| Ability                      | Description                                                                            |
| ---------------------------- | -------------------------------------------------------------------------------------- |
| **Piercing Stream** (Normal) | Fire an arrow that bursts into a water jet, passing through enemies and slowing them.  |
| **Tidal Barrage** (Ultimate) | Rain down a storm of water arrows over a large area, dealing continuous splash damage. |

💡 *Theme*: Long range control, AoE harass, slow.

---

### 🌬 **AIR – Pistol**

| Ability                    | Description                                                                                            |
| -------------------------- | ------------------------------------------------------------------------------------------------------ |
| **Gale Dash** (Normal)     | Dash forward with a burst of wind, leaving behind a slicing wind that damages nearby enemies.          |
| **Storm Pulse** (Ultimate) | Fire a charged air bullet that explodes in a wind vortex, pulling enemies inward and stunning briefly. |

💡 *Theme*: Speed, mobility, vortex-style disruption.

---

### 🌟 **LIGHT – Rifle**

| Ability                   | Description                                                                                                              |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| **Photon Burst** (Normal) | Fire a piercing laser beam that damages all enemies in a line.                                                           |
| **Solar Nova** (Ultimate) | Mark enemies for divine judgment — after a short delay, a solar blast hits each marked enemy with high precision damage. |

💡 *Theme*: High damage, precision, light-based smite effect.

---

***/
