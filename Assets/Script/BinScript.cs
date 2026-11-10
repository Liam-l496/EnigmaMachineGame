using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinScript : MonoBehaviour
{

    [Header("Light Bulb position")]
    public Transform lightBulbDropPosition;
    
    [Header("Light Bulb Prefab")]
    public GameObject lightBulbPrefab;
    
    [Header("Light Bulb Prefab Array")]
    public GameObject[] lightBulbArray = new GameObject[20];
    public int lightBulbArrayIndex = 0;
    public float noiseAmount = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateLampBulb()
    {

        Vector3 randomPositionOffset = new Vector3(
            Random.Range(-noiseAmount, noiseAmount),
            Random.Range(-noiseAmount, noiseAmount),
            Random.Range(-noiseAmount, noiseAmount)
        );

        Vector3 finalPosition = lightBulbDropPosition.position + randomPositionOffset;


        Quaternion randomRotation = Random.rotation;

        GameObject lightBulb = Instantiate(lightBulbPrefab, finalPosition, randomRotation);
        Rigidbody lightBulbRB = lightBulb.AddComponent<Rigidbody>();
        
        Interactable lightBulbInteractable = lightBulb.GetComponent<Interactable>();
        if (lightBulbInteractable) Destroy(lightBulbInteractable);
        lightBulb.tag = "Untagged";

        lightBulbRB.useGravity = true;
        lightBulbRB.isKinematic = false;
        if (lightBulbArray[lightBulbArrayIndex] != null)
        {
            Destroy(lightBulbArray[lightBulbArrayIndex]);
        }
        lightBulbArray[lightBulbArrayIndex] = lightBulb;
        lightBulbArrayIndex++;
        if (lightBulbArrayIndex >= lightBulbArray.Length)
        {
            lightBulbArrayIndex = 0;
        }
    }
}
