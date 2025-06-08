using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    public WeaponData data;

    public void Use()
    {
        switch (data.elementType)
        {
            case ElementType.Ice:
                Freeze();
                break;
            case ElementType.Fire:
                Burn();
                break;
            case ElementType.Earth:
                //earth effect
                break;
            case ElementType.Water:
                Drown();
                break;
            case ElementType.Air:
                //Air effect
                break;
            case ElementType.Light:
                Stun();
                break;
        }
    }

    private void Freeze()
    {
        Debug.Log("Freezing enemy...");
        // Freeze logic here
    }

    private void Burn()
    {
        Debug.Log("Burning enemy...");
        // Burn logic here
    }
    private void Drown()
    {
        Debug.Log("Drowning enemy...");
        // Burn logic here
    }
    private void Stun()
    {
        Debug.Log("Stunning enemy...");
        // Burn logic here
    }
}
