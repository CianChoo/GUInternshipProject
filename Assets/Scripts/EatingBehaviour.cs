using System;
using UnityEngine;

public class EatingBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            EatFood(other.gameObject);
        }
    }

    private void EatFood(GameObject food)
    {
        Destroy(food);
    }
}
