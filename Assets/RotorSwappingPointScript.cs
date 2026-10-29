using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorSwappingPointScript : MonoBehaviour
{

    private GameObject rotor;

    public GameObject GetRotor()
    {
        return rotor;
    }
    public void SetRotor(GameObject newRotor)
    {
        rotor = newRotor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided a");
    }
}
