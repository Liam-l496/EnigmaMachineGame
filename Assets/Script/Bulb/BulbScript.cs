using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbScript : MonoBehaviour
{

    private Interactable interactable;
    private bool isSelected = false;

    [Header("Material")]
    public GameObject glassBulb;
    private Renderer lampBulbRenderer;
    public Material glassMaterial;
    public Material lightUpMaterial;
    
    [Header("Bulb State")]
    public bool lightOn = false;
    public bool broken = false;

    [Header("BulbTesting")]
    public GameObject bulbTestingObject;
    public Interactable bulbTestingInteractable;
    public LampTestingScript lampTestingScript;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        lampBulbRenderer = glassBulb.GetComponent<Renderer>();
        bulbTestingInteractable = bulbTestingObject.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactable.isSelected) return;

        if (Input.GetMouseButtonDown(0)) {
            print("IN BULB CLICK + " + PlayerPickUpScript.Instance.skipUpdate);
            if (PlayerPickUpScript.Instance.skipUpdate && lampTestingScript)
            {
                lampTestingScript.DeactivateLampTestingScript();
                lampTestingScript = null;
                PlayerPickUpScript.Instance.UpdateEndPosition(PlayerPickUpScript.Instance.holdPosition);
                PlayerPickUpScript.Instance.skipUpdate = false;
                PlayerPickUpScript.Instance.PutAnotherInteractableDown();
                return;
            }
        }

        if (PlayerPickUpScript.Instance.sameObjectClicked)
        {
            lightOn = !lightOn;
            UpdateMaterial();
            PlayerPickUpScript.Instance.sameObjectClicked = false;
        }

        if (PlayerPickUpScript.Instance.anotherInteractableClicked && !lampTestingScript)
        {
            LampTestingScript lampTesting = CheckAnotherObject();
            if (!lampTesting) return;
            print("Lamp Testing Cylinder Clicked");
            lampTestingScript = lampTesting;
            lampTestingScript.ClickedWithBulb(this);
            PlayerPickUpScript.Instance.skipUpdate = true;
        }

        bulbTestingInteractable.EnableOutline();
    }

    void LateUpdate()
    {
        if (interactable.runInitialScript)
        {
            bulbTestingObject.SetActive(true);
            interactable.runInitialScript = false;
        }
        else if (interactable.runEndingScript)
        {
            bulbTestingObject.SetActive(false);
            interactable.runEndingScript = false;
            lightOn = false;
            UpdateMaterial();
        }
    }

    public void UpdateMaterial()
    {
        if (broken)
        {
            lampBulbRenderer.material = glassMaterial;
            return;
        } 
        if (lightOn)
        {
            lampBulbRenderer.material = lightUpMaterial;
        }
        else
        {
            lampBulbRenderer.material = glassMaterial;
        }
    }

    public LampTestingScript CheckAnotherObject()
    {
        LampTestingScript lampTesting = PlayerPickUpScript.Instance.anotherInteractableClicked.GetComponent<LampTestingScript>();
        if (!lampTesting)
        {
            PlayerPickUpScript.Instance.wrongObjectClicked = true;
        }
        return lampTesting;
    }
}
