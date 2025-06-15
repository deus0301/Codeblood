using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerData>(out PlayerData player))
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
