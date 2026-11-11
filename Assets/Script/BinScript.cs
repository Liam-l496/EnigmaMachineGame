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

    [Header("Light bulb")]
    public Vector3 bulbStartingPosition;
    public Quaternion bulbStartingRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveLightBulb()
    {
        NewStartingPostition();
        NewRandomRotation();
        PlayerPickUpScript.Instance.UpdateEndPosition(bulbStartingPosition);
        PlayerPickUpScript.Instance.UpdateEndRotation(bulbStartingRotation);
    }

    private Vector3 NewStartingPostition()
    {
        Vector3 randomPositionOffset = new Vector3(
            Random.Range(-noiseAmount, noiseAmount),
            Random.Range(-noiseAmount, noiseAmount),
            Random.Range(-noiseAmount, noiseAmount)
        );

        bulbStartingPosition = lightBulbDropPosition.position + randomPositionOffset;
        return bulbStartingPosition;
    }

    private Quaternion NewRandomRotation()
    {
        bulbStartingRotation = Random.rotation;
        return bulbStartingRotation;
    }

    public void InstantiateLampBulb()
    {
        GameObject lightBulb = Instantiate(lightBulbPrefab, bulbStartingPosition, bulbStartingRotation);
        Rigidbody lightBulbRB = lightBulb.AddComponent<Rigidbody>();

        Interactable lightBulbInteractable = lightBulb.GetComponent<Interactable>();
        if (lightBulbInteractable) Destroy(lightBulbInteractable);
        lightBulb.tag = "Untagged";

        SpareBulbScript lightBulbSpareScript = lightBulb.GetComponent<SpareBulbScript>();
        if (lightBulbSpareScript) Destroy(lightBulbSpareScript);

        BulbScript lightBulbBulbScript = lightBulb.GetComponent<BulbScript>();
        if (lightBulbBulbScript) Destroy(lightBulbBulbScript);

        ConstantForce lightBulbForce = lightBulb.AddComponent<ConstantForce>();
        lightBulbForce.force = new Vector3(0, -25f, 0);


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
