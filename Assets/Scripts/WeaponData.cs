using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float cooldown;
    public ElementType elementType; // enum for Ice, Fire, etc.
    public GameObject weaponPrefab;
}
public enum ElementType
{
    Ice,
    Fire,
    Earth,
    Water,
    Air,
    Light
}