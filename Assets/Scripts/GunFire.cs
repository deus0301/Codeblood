using UnityEngine;

public class GunFire : MonoBehaviour
{
    public ParticleSystem bulletTracer;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            bulletTracer.Play();
        }
    }
}