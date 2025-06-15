using UnityEngine;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public TMP_Dropdown weaponSelect;
    public WeaponBehaviour weaponBehavior;
    public WeaponData[] allWeapons; // Assign all 6 in Inspector
    public GameObject weaponHolder;
    public GameObject weaponPrefab;
    public GameObject currentWeaponInstance;
    public GameObject cam;
    
    public TMP_Dropdown bossSelect;
    public GameObject[] Bosses;
    public GameObject bossSpawn;
    public GameObject currentBossInstance;

    private bool weaponChosen = false;
    private bool bossChosen = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponBehavior = GetComponent<WeaponBehaviour>();
        cam = GameObject.Find("Main Camera");
        cam.GetComponent<MouseLook>().enabled = false;
        gameObject.GetComponent<PlayerController>().enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ChooseWeapon()
    {
        int index = weaponSelect.value - 1;
        weaponBehavior.data = allWeapons[index];
        weaponSelect.gameObject.SetActive(false);
        weaponChosen = true;
        weaponPrefab = weaponBehavior.data.weaponPrefab.gameObject;
        currentWeaponInstance = Instantiate(weaponPrefab, weaponHolder.transform);
        
        GetComponent<PlayerAttack>().SetWeaponInstance(currentWeaponInstance);
    }

    public void ChooseBoss()
    {
        int index = bossSelect.value - 1;
        bossSelect.gameObject.SetActive(false);
        bossChosen = true;
        GameObject boss = Bosses[index];
        currentBossInstance = Instantiate(boss, bossSpawn.transform);

        GameObject.FindWithTag("Enemy").GetComponent<BossController>().SetBossInstance(currentBossInstance);
        GameObject.Find("GameManager").GetComponent<GameManager>().SetBossInstance(currentBossInstance);
    }

    // Update is called once per frame
    void Update()
    {
        if(weaponChosen && bossChosen)
        { 
            cam.GetComponent<MouseLook>().enabled = true;
            gameObject.GetComponent<PlayerController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }
}
