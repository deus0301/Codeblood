using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    PlayerInput playerInput;

    [Header("Animations")]

    public const string ATTACK1 = "Attack1";
    public const string ATTACK2 = "Attack2";

    public Animator animator;
    string currentAnimationState;

    [Header("Attacking")]
    float attackDistance;
    public float attackDelay;
    public float attackSpeed = 0.1667f;
    int attackDamage;
    public LayerMask attackLayer;

    //public GameObject hitEffect;
    public ParticleSystem bulletTracer;
    public GameObject cam;
    //public AudioClip swordSwing;
    //public AudioClip hitSound;
    WeaponBehaviour weaponBehaviour;
    WeaponData weapon;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;
    private GameObject weaponInstance;

    PlayerAbilites abilities;


    private void Start()
    {
        weaponBehaviour = GetComponent<WeaponBehaviour>();
    }
    void Update()
    {
        //if (weaponBehaviour.data != null)
        //{
            //weapon = weaponBehaviour.data;
            //attackDamage = weapon.damage;
            //attackDistance = weapon.range;
            //attackSpeed = weapon.cooldown;
            //abilities = gameObject.GetComponent<PlayerAbilites>();
            //if (weapon.weaponPrefab.gameObject.layer == LayerMask.NameToLayer("Ranged")) { bulletTracer = GameObject.Find("Bullet").GetComponent<ParticleSystem>(); }
        //}
    }
    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);
        Debug.DrawRay(cam.transform.position, cam.transform.forward * attackDistance, Color.red, 2f);

        if (weapon != null)
        {
            if (weapon.weaponPrefab.gameObject.layer == LayerMask.NameToLayer("Ranged"))
            {
                bulletTracer.Play();
            }
            else
            {
                if (attackCount == 0)
                {
                    ChangeAnimationState(ATTACK1);
                    attackCount++;
                }
                else
                {
                    ChangeAnimationState(ATTACK2);
                    attackCount = 0;
                }
            }
        }
        else
        {
            return;
        }

        abilities.TrackMove("normal");

        //audioSource.pitch = Random.Range(0.9f, 1.1f);
        //audioSource.PlayOneShot(swordSwing);
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    void AttackRaycast()
    {
        Debug.Log("Attack raycast sent");
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            Debug.Log("You hitting shit nigga");
            if (hit.transform.TryGetComponent<Enemy>(out Enemy T))
            { Debug.Log("Took Damage"); T.TakeDamage(attackDamage); }
        }
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        currentAnimationState = newState;
        animator.Play(currentAnimationState);
    }

    public void SetWeaponInstance(GameObject instance)
    {
        weaponInstance = instance;
        animator = weaponInstance.GetComponent<Animator>();
        weapon = weaponBehaviour.data;
        attackDamage = weapon.damage;
        attackDistance = weapon.range;
        attackSpeed = weapon.cooldown;
        abilities = gameObject.GetComponent<PlayerAbilites>();
        if (weapon.weaponPrefab.gameObject.layer == LayerMask.NameToLayer("Ranged")) { bulletTracer = GameObject.Find("Bullet").GetComponent<ParticleSystem>(); }
    }
}
