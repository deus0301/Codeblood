using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilites : MonoBehaviour
{
    public const string ABILITY = "Ability";
    public const string ULTIMATE = "Ultimate";

    public WeaponData currentWeapon;
    public GameObject cam;

    public float normalCooldown = 5f;
    public float ultimateCooldown = 10f;

    private bool canUseNormal = true;
    private bool canUseUltimate = false;

    [SerializeField]
    private Queue<string> recentMoves = new Queue<string>();
    private int maxStoredMoves = 5;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
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
    }

    public void Ability()
    {
        StartCoroutine("UseAbility");
    }
    public void Ultimate()
    {
        StartCoroutine("UseUltimate");
    }

    void CastAbility(ElementType type)
    {
        switch (type)
        {
            case ElementType.Ice:
                // Frost Slam
                Debug.Log("Casting Frost Slam");
                // Add area freeze + damage
                break;

            case ElementType.Fire:
                Debug.Log("Casting Flame Whip");
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
        TrackMove("ability_" + currentWeapon.elementType.ToString().ToLower());
        yield return new WaitForSeconds(normalCooldown);
        canUseNormal = true;
    }

    IEnumerator UseUltimate()
    {
        canUseUltimate = false;
        CastUltimate(currentWeapon.elementType);
        TrackMove("ultimate_" + currentWeapon.elementType.ToString().ToLower());
        yield return new WaitForSeconds(ultimateCooldown);
        canUseUltimate = true;
    }

    public void TrackMove(string moveName)
    {
        if (recentMoves.Count >= maxStoredMoves) recentMoves.Dequeue();
        recentMoves.Enqueue(moveName);
        Debug.Log("Move Tracked: " + moveName);
        // TODO: Send to WebSocket AI after 5 moves
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
