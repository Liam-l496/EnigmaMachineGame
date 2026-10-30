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
    public bool lightOn = false;

    [Header("BulbTesting")]
    public GameObject bulbTestingObject;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        lampBulbRenderer = glassBulb.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactable.isSelected) return;
        // if (interactable.runInitialScript)
        // {
        //     FirstInteractScript();
        //     interactable.runInitialScript = false;
        // }
        // if (interactable.runEndingScript)
        // {
        //     LastInteractionScript();
        //     interactable.runEndingScript = false;
        // }
        if (interactable.clickedAgain)
        {
            interactable.UpdateClickedAgain(false);
            lightOn = !lightOn;
            UpdateMaterial();
        }
        // bulbTestingObject.GetComponent<Outline>().enabled = true;
    }
    void LateUpdate()
    {
        if (!interactable.isSelected) return;
    }

    public void UpdateMaterial()
    {
        if (lightOn)
        {
            lampBulbRenderer.material = lightUpMaterial;
        }
        else
        {
            lampBulbRenderer.material = glassMaterial;
        }
    }

    public void FirstInteractScript()
    {
        print("FIRST INTERACT");
        // bulbTestingObject.SetActive(true);
    }

    public void LastInteractionScript()
    {
        print("Last Interaction");
        // bulbTestingObject.SetActive(false);
    }
}
