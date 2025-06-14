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


    private void Start()
    {
        weaponBehaviour = GetComponent<WeaponBehaviour>();
    }
    void Update()
    {
        if (weaponBehaviour.data != null)
        {
            weapon = weaponBehaviour.data;
            attackDamage = weapon.damage;
            attackDistance = weapon.range;
            attackSpeed = weapon.cooldown;
            if (weapon.weaponPrefab.gameObject.layer == LayerMask.NameToLayer("Ranged")) { bulletTracer = GameObject.Find("Bullet").GetComponent<ParticleSystem>(); }
        }
    }
    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        if (weapon != null)
        {
            Debug.Log("Weapon Data is not null");
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
            Debug.LogWarning("Trying to attack, but no weapon animator assigned yet.");
            return;
        }


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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {

            if (hit.transform.TryGetComponent<Enemy>(out Enemy T))
            { T.TakeDamage(attackDamage); }
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
    }
}
