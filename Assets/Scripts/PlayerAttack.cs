using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    PlayerInput playerInput;

    [Header("Attacking")]
    float attackDistance;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    int attackDamage;
    public LayerMask attackLayer;

    //public GameObject hitEffect;
    public GameObject cam;
    //public AudioClip swordSwing;
    //public AudioClip hitSound;
    WeaponBehaviour weaponBehaviour;
    WeaponData weapon;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;


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
        }
    }
    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        //audioSource.pitch = Random.Range(0.9f, 1.1f);
        //audioSource.PlayOneShot(swordSwing);

        //if(attackCount == 0)
        //{
        //    ChangeAnimationState(ATTACK1);
        //    attackCount++;
        //}
        //else
        //{
        //    ChangeAnimationState(ATTACK2);
        //    attackCount = 0;
        //}
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
            //HitTarget(hit.point);

            if (hit.transform.TryGetComponent<Enemy>(out Enemy T))
            { T.TakeDamage(attackDamage); }
        }
    }
    
    //void HitTarget(Vector3 pos)
    //{
    //audioSource.pitch = 1;
    //audioSource.PlayOneShot(hitSound);

    //GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
    //Destroy(GO, 20);
    //} 
}
