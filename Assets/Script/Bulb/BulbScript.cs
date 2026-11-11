using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public GameObject metalFilament;
    private Renderer metalFilamentRenderer;
    public Material metalFilamentMaterial;
    
    [Header("Bulb State")]
    public bool lightOn = false;
    public bool broken = false;

    [Header("BulbTesting")]
    public GameObject bulbTestingObject;
    public Interactable bulbTestingInteractable;
    public LampTestingScript lampTestingScript;

    [Header("Bin Testing")]
    public bool deleteBulb = false;
    public BinScript binScript;

    [Header("Bulb Position Indicator")]
    public BulbPositionIndicator bulbPositionIndicator;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        lampBulbRenderer = glassBulb.GetComponent<Renderer>();
        if (bulbTestingObject) bulbTestingInteractable = bulbTestingObject.GetComponent<Interactable>();
        metalFilamentRenderer = metalFilament.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactable.isSelected) return;

        if (deleteBulb)
        {
            if (PlayerPickUpScript.Instance.inAnimation) return;
            binScript.InstantiateLampBulb();
            PlayerPickUpScript.Instance.skipUpdate = false;
            PlayerPickUpScript.Instance.PutAnotherInteractableDown();
            PlayerPickUpScript.Instance.PutDownObject();
            PlayerPickUpScript.Instance.inAnimation = false;
            
            bulbTestingObject.SetActive(false);
            deleteBulb = false;


            bulbPositionIndicator.gameObject.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

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

        if (PlayerPickUpScript.Instance.anotherInteractableClicked)
        {
            LampTestingScript lampTesting = CheckAnotherObjectLamp();
            BinScript binTesting = CheckAnotherObjectBin();
            BulbPositionIndicator bulbPositionTest = CheckAnotherObjectBulbPosition();

            if (!lampTesting && !binTesting && !bulbPositionTest)
            {
                PlayerPickUpScript.Instance.wrongObjectClicked = true;
                return;
            }
            else if (lampTesting) {
                lampTestingScript = lampTesting;
                lampTestingScript.ClickedWithBulb(this);
            }
            else if (binTesting)
            {
                binTesting.MoveLightBulb();
                binScript = binTesting;
                BulbManager.Instance.AddBulb(this, bulbPositionIndicator);
                deleteBulb = true;
            }
            else if (bulbPositionTest) 
            {
                bulbPositionTest.MoveLightBulb(gameObject);
                
            }

            PlayerPickUpScript.Instance.skipUpdate = true;
        }



        if (bulbTestingInteractable) bulbTestingInteractable.EnableOutline();
    }

    void LateUpdate()
    {
        if (interactable.runInitialScript)
        {
            if (bulbTestingObject) bulbTestingObject.SetActive(true);
            interactable.runInitialScript = false;
        }
        else if (interactable.runEndingScript)
        {
            if (bulbTestingObject) bulbTestingObject.SetActive(false);
            interactable.runEndingScript = false;
            lightOn = false;
            UpdateMaterial();
        }
    }

    public void UpdateMaterial()
    {
        if (broken)
        {
            // lampBulbRenderer.material = glassMaterial;
            LightOff();
            return;
        } 
        if (lightOn)
        {
            // lampBulbRenderer.material = lightUpMaterial;
            LightOn();
        }
        else
        {
            // lampBulbRenderer.material = glassMaterial;
            LightOff();
        }
    }

    public LampTestingScript CheckAnotherObjectLamp()
    {
        LampTestingScript lampTesting = PlayerPickUpScript.Instance.anotherInteractableClicked.GetComponent<LampTestingScript>();
        return lampTesting;
    }

    public BinScript CheckAnotherObjectBin()
    {
        BinScript binTesting = PlayerPickUpScript.Instance.anotherInteractableClicked.GetComponent<BinScript>();
        return binTesting;
    }
    public BulbPositionIndicator CheckAnotherObjectBulbPosition()
    {
        BulbPositionIndicator bulbTesting = PlayerPickUpScript.Instance.anotherInteractableClicked.GetComponent<BulbPositionIndicator>();
        return bulbTesting;
    }

    public void LightOn()
    {
        lampBulbRenderer.material = lightUpMaterial;
        metalFilamentRenderer.material = lightUpMaterial;
    }

    public void LightOff()
    {
        lampBulbRenderer.material = glassMaterial;
        metalFilamentRenderer.material = metalFilamentMaterial;
    }
}
