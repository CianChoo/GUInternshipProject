using System;
using UnityEngine;

public class EatingBehaviour : MonoBehaviour
{
    public ParticleSystem crumbs;
    public ParticleSystem heart;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            EatFood(other.gameObject);
            crumbs.Play();
            heart.Play();
        }
    }

    private void EatFood(GameObject food)
    {
        Destroy(food);
    }
}
