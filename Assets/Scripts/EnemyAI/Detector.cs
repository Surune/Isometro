using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField] private AIData data;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            data.targets.Add(other.transform);
            if (data.currentTarget == null)
            {
                data.currentTarget = other.transform;
            }
        }
        else if (other.tag == "Terrain")
        {
            data.obstacles.Add(other);
        }
    }
}