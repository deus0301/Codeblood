using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilites : MonoBehaviour
{
    public const string ABILITY = "Ability";
    public const string ULTIMATE = "Ultimate";

    public WeaponData currentWeapon;
    public GameObject cam;
    public BossController boss;

    public float normalCooldown = 5f;
    public float ultimateCooldown = 200f;

    private bool canUseNormal = true;
    private bool canUseUltimate = true;

    private Queue<string> recentMove = new Queue<string>();
    [SerializeField] private WebSocketManager wsManager;
    private int maxStoredMoves = 1;
    public LayerMask enemyLayer;

    public Button abilityButton;
    public Button ultButton;

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
            if (GameObject.FindWithTag("Enemy").TryGetComponent<BossController>(out BossController E))
                boss = E;
        }
        catch { }

        if (canUseNormal)
        {
            abilityButton.interactable = true;
        }
        else
        {
            abilityButton.interactable = false;
        }
        if (canUseUltimate)
        {
            ultButton.interactable = true;
        }
        else
        {
            ultButton.interactable = false;
        }
    }

    public void Ability()
    {
        if (canUseNormal && boss.gameObject != null)
        {
            StartCoroutine("UseAbility");
        }
    }
    public void Ultimate()
    {
        if (canUseUltimate && boss.gameObject != null)
        {
            StartCoroutine("UseUltimate");
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
                StartCoroutine(FlameWhip());
                break;

            case ElementType.Earth:
                Debug.Log("Casting Stonewall Slash");
                StartCoroutine(StonewallSlash());
                break;
            case ElementType.Water:
                Debug.Log("Casting Piercing Stream");
                PiercingStream();
                break;

            case ElementType.Air:
                Debug.Log("Casting Gale Dash");
                StartCoroutine(GaleDash());
                break;

            case ElementType.Light:
                Debug.Log("Casting Photon Burst");
                StartCoroutine(PhotonBurst());
                break;
        }
    }
    void CastUltimate(ElementType type)
    {
        switch (type)
        {
            case ElementType.Ice:
                GlacialTomb();
                Debug.Log("Casting Glacial Tomb");
                break;

            case ElementType.Fire:
                Debug.Log("Casting Infernal Maelstrom");
                StartCoroutine(InfernalMaelstrom());
                break;

            case ElementType.Earth:
                Debug.Log("Casting Stonewall Slash");
                StartCoroutine(TerraShatter());
                break;
            case ElementType.Water:
                Debug.Log("Casting Tidal Barrage");
                StartCoroutine(TidalBarrage());
                break;

            case ElementType.Air:
                Debug.Log("Casting Storm Pulse");
                StartCoroutine(StormPulse());
                break;

            case ElementType.Light:
                Debug.Log("Casting Solar Nova");
                StartCoroutine(SolarNova());
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
        if (moveName == null) return;
        if (recentMove.Count >= maxStoredMoves) recentMove.Dequeue();
        recentMove.Enqueue(moveName);
        Debug.Log("Move Tracked: " + moveName);
        int attackNumber = 0;
        switch (moveName)
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

    [Header("Fire Abilities")]

    float range = 10f;
    float radius = 1.5f;
    int dmg = 100;
    int burndmg = 10;
    float burnDuration = 5f;
    float burnInterval = 1f;

    IEnumerator FlameWhip()
    {
        Debug.Log("Flame Whip starts");
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
    private float maelstromRadius = 10f;
    private int maelstromDamage = 500;
    private float maelstromDuration = 5f;
    private float maelstromTickRate = 1f;
    private float pullStrength = 3f;

    private IEnumerator InfernalMaelstrom()
    {
        float timer = 0f;

        while (timer < maelstromDuration)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, maelstromRadius, enemyLayer);

            foreach (Collider enemyCol in hitEnemies)
            {
                if (enemyCol.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    // Apply damage over time
                    enemy.TakeDamage(maelstromDamage);

                    // Apply pulling force toward the player
                    Vector3 pullDir = (transform.position - enemy.transform.position).normalized;
                    enemy.transform.position += pullDir * pullStrength * Time.deltaTime;
                }
            }

            // Optional: Add VFX here
            timer += maelstromTickRate;
            yield return new WaitForSeconds(maelstromTickRate);
        }
    }

    [Header("Earth Abilities")]
    public GameObject stonewallPrefab;
    private float stonewallDistance = 4f;
    private float stonewallDuration = 5f;

    private float shatterRange = 10f;
    private float shatterWidth = 2f;
    private int shatterDamage = 750;
    private float shatterKnockupForce = 50f;

    private IEnumerator StonewallSlash()
    {
        Vector3 spawnPos = transform.position + transform.forward * stonewallDistance;
        Quaternion rotation = Quaternion.LookRotation(-transform.forward); // Face player
        GameObject wall = Instantiate(stonewallPrefab, spawnPos, rotation);

        yield return new WaitForSeconds(stonewallDuration);
        Destroy(wall);
    }

    private IEnumerator TerraShatter()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        Debug.DrawRay(origin, forward * shatterRange, Color.green, 1.5f);

        RaycastHit[] hits = Physics.SphereCastAll(origin, shatterWidth, forward, shatterRange, enemyLayer);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(shatterDamage);

                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * shatterKnockupForce, ForceMode.Impulse);
                }

                Debug.Log("Terra Shatter hit " + enemy.name);
            }
        }

        yield return null;
    }

    [Header("Water Abilities")]
    public GameObject waterArrowPrefab;
    private float arrowRange = 25f;
    private int arrowDamage = 100;
    private float slowDuration = 3f;

    private float barrageRadius = 8f;
    private int barrageDamagePerHit = 100;
    private float barrageTickRate = 0.5f;
    private float barrageDuration = 8f;

    void PiercingStream()
    {
        Vector3 origin = cam.transform.position + cam.transform.forward * 1.5f;
        Vector3 dir = cam.transform.forward;

        if (waterArrowPrefab != null)
        {  
            GameObject arrow = Instantiate(waterArrowPrefab, origin, Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f));
            arrow.transform.Translate(Vector3.forward);
            Destroy(arrow, 1.5f); // Clean up after short time
        }
        RaycastHit[] hits = Physics.RaycastAll(origin, dir, arrowRange, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(arrowDamage);
                if (enemy.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    // Optional: slow effect or drag increase
                    StartCoroutine(SlowEnemy(enemy, slowDuration));
                }

                Debug.Log("Piercing Stream hit: " + enemy.name);
            }
        }

        // Optional: Visual arrow effect
        Debug.DrawRay(origin, dir * arrowRange, Color.cyan, 1.5f);
    }

    private IEnumerator TidalBarrage()
    {
        float timer = 0f;

        while (timer < barrageDuration)
        {
            // Random point inside radius
            Vector2 offset = UnityEngine.Random.insideUnitCircle * barrageRadius;
            Vector3 targetPos = transform.position + new Vector3(offset.x, 0f, offset.y);
            Vector3 spawnPos = targetPos + Vector3.up * 15f; // 15 units above

            // Instantiate arrow prefab
            if (waterArrowPrefab != null)
            {
                GameObject arrow = Instantiate(waterArrowPrefab, spawnPos, Quaternion.identity);
                Rigidbody rb = arrow.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 dir = (targetPos - spawnPos).normalized;
                    rb.linearVelocity = dir * 25f;
                }

                // Schedule impact using coroutine
                StartCoroutine(WaterArrowImpact(targetPos, 0.6f)); // delay depends on speed/distance
                Destroy(arrow, 3f); // clean up visual arrow
            }

            timer += barrageTickRate;
            yield return new WaitForSeconds(barrageTickRate);
        }
    }
    IEnumerator SlowEnemy(Enemy enemy, float duration)
    {
        if (enemy == null) yield break;

        // Optional: Implement movement speed reduction inside Enemy script
        enemy.SetSlowed(true); // You need to implement this
        yield return new WaitForSeconds(duration);
        enemy.SetSlowed(false);
    }   
    private IEnumerator WaterArrowImpact(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay); // simulate time to hit ground

        // Optionally: play splash VFX here
        Debug.Log("Water arrow impacted at: " + position);

        Collider[] hitEnemies = Physics.OverlapSphere(position, 2f, enemyLayer); // 2f = splash radius
        foreach (var hit in hitEnemies)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(barrageDamagePerHit);
                Debug.Log("Tidal Barrage hit: " + enemy.name);
            }
        }
    }

    [Header("Air Abilities")]
    [SerializeField] private float dashDistance = 8f;
    [SerializeField] private int dashDamage = 150;
    [SerializeField] private float dashRadius = 2f;

    [SerializeField] private float pulseRange = 20f;
    [SerializeField] private float pulseRadius = 6f;
    [SerializeField] private int pulseDamage = 500;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float pullForce = 10f;

    private IEnumerator GaleDash()
    {
        Vector3 dashDirection = transform.forward;
        Vector3 start = transform.position;
        Vector3 end = start + dashDirection * dashDistance;

        // Optional: dash VFX trail or animation

        float elapsed = 0f;
        float dashTime = 0.2f;

        CharacterController controller = GetComponent<CharacterController>();
        while (elapsed < dashTime)
        {
            controller.Move(dashDirection * (dashDistance / dashTime) * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Damage enemies in dash path
        Collider[] hit = Physics.OverlapCapsule(start, end, dashRadius, enemyLayer);
        foreach (Collider col in hit)
        {
            if (col.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(dashDamage);
            }
        }
    }
    private IEnumerator StormPulse()
    {
        Debug.Log("Storm Pulse started");
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;

        Ray ray = new Ray(origin, dir);
        Debug.DrawRay(origin, dir * pulseRange, Color.cyan, 1.5f);
        if (Physics.Raycast(ray, out RaycastHit hit, pulseRange))
        {
            Debug.Log("Storm Pulse hit");
            Vector3 impactPoint = hit.point;

            Collider[] enemies = Physics.OverlapSphere(impactPoint, pulseRadius, enemyLayer);
            foreach (Collider col in enemies)
            {
                if (col.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    Debug.Log("Enemy Damaged and stunned");
                    enemy.TakeDamage(pulseDamage);

                    // Pull toward impact
                    Vector3 pullDir = (impactPoint - enemy.transform.position).normalized;
                    enemy.transform.position += pullDir * pullForce * Time.deltaTime;

                    // Apply stun if you have it
                    enemy.Stun(stunDuration); // implement this in Enemy.cs
                }
            }
        }
        else{
            Debug.Log("Storm Pulse missed");
        }

        yield return null;
    }

    [Header("Light Abilities")]
    [SerializeField] private float burstRange = 50f;
    [SerializeField] private int burstDamage = 150;
    [SerializeField] private float burstWidth = 1.5f;

    [SerializeField] private float novaRadius = 25f;
    [SerializeField] private int novaDamage = 500;
    [SerializeField] private float novaDelay = 1.5f;

    private IEnumerator PhotonBurst()
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        float duration = 1f;
        float tickRate = 0.2f;
        float timePassed = 0f;

        // Optional: visual effect setup
        GameObject beam = CreateBeam(origin, direction); // If you have a visual prefab

        while (timePassed < duration)
        {
            RaycastHit[] hits = Physics.SphereCastAll(origin, burstWidth, direction, burstRange, enemyLayer);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.TryGetComponent<Enemy>(out Enemy enemy))
                {   
                    enemy.TakeDamage(burstDamage);
                    Debug.Log("🔆 Photon Burst hit: " + enemy.name);
                }
            }

            Debug.DrawRay(origin, direction * burstRange, Color.yellow, tickRate);
            yield return new WaitForSeconds(tickRate);
            timePassed += tickRate;
        }

        // Cleanup VFX
        if (beam != null)
            Destroy(beam);
    }

    private GameObject CreateBeam(Vector3 origin, Vector3 direction)
    {
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beam.transform.position = origin + direction * (burstRange / 2);
        beam.transform.rotation = Quaternion.LookRotation(direction);
        beam.transform.Rotate(90, 0, 0); // Cylinder is vertical by default
        beam.transform.localScale = new Vector3(burstWidth/5, burstRange * 10, burstWidth/5);

        // Appearance
        Renderer r = beam.GetComponent<Renderer>();
        r.material.color = Color.yellow;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Destroy(beam.GetComponent<Collider>()); // remove physics

        return beam;
    }

    private IEnumerator SolarNova()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, novaRadius, enemyLayer);
        Debug.Log("🌞 Solar Nova will hit: " + enemies.Length + " enemies");

        List<Enemy> targets = new List<Enemy>();

        foreach (Collider col in enemies)
        {
            if (col.TryGetComponent<Enemy>(out Enemy enemy))
            {
                targets.Add(enemy);
            }
        }

        yield return new WaitForSeconds(novaDelay);

        foreach (Enemy target in targets)
        {
            if (target != null)
            {
                target.TakeDamage(novaDamage);
                Debug.Log("💥 Solar Nova hit: " + target.name);
            }
        }
    }

}

/***

## ⚔️ ELEMENTAL CHARACTER ABILITIES

### **LIGHT Rifle**

| Ability                   | Description                                                                                                              |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| **Photon Burst** (Normal) | Fire a piercing laser beam that damages all enemies in a line.                                                           |
| **Solar Nova** (Ultimate) | Mark enemies for divine judgment after a short delay, a solar blast hits each marked enemy with high precision damage. |

*Theme*: High damage, precision, light-based smite effect.

---

***/
