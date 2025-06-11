using UnityEngine;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public TMP_Dropdown weaponSelect;
    public WeaponBehaviour weaponBehavior;
    public WeaponData[] allWeapons; // Assign all 6 in Inspector
    public GameObject weaponHolder;
    public GameObject weaponPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponBehavior = GetComponent<WeaponBehaviour>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ChooseWeapon()
    {
        int index = weaponSelect.value;
        weaponBehavior.data = allWeapons[index];
        weaponSelect.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        weaponPrefab = weaponBehavior.data.weaponPrefab.gameObject;
        //Vector3 instPos = weaponHolder.transform + Vector3();
        Instantiate(weaponPrefab, weaponHolder.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
