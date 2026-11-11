using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbPositionIndicator : MonoBehaviour
{
    [Header("Self Instantiation")]
    public Interactable interactable;
    private Outline outline;

    [Header("Light Bulb")]
    public GameObject bulb;
    public BulbScript bulbScript;
    public Vector3 bulbPosition;
    public Quaternion bulbRotation;

    [Header("Spare Bulb")]
    private GameObject spareBulb;

    [Header("Animation")]
    public bool outlineOn = false;
    public bool inAnimation = false;


    // Start is called before the first frame update
    void Start()
    {
        if (bulb) 
        {
            bulbScript = bulb.GetComponent<BulbScript>();
            bulbPosition = bulb.transform.position;
            bulbRotation = bulb.transform.rotation;
        }
        interactable = GetComponent<Interactable>();
        outline = GetComponent<Outline>();
        interactable.enabled = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (outlineOn) outline.enabled = true;
    }

    void LateUpdate()
    {
        if (inAnimation)
        {
            if(PlayerPickUpScript.Instance.inAnimation) return;
            bulb.SetActive(true);
            bulb.transform.position = bulbPosition;
            bulb.transform.rotation = bulbRotation;
            bulb.GetComponent<BulbScript>().broken = false;

            Destroy(spareBulb);
            inAnimation = false;

            PlayerPickUpScript.Instance.skipUpdate = false;
            PlayerPickUpScript.Instance.PutAnotherInteractableDown();
            PlayerPickUpScript.Instance.PutDownObject();
            PlayerPickUpScript.Instance.inAnimation = false;

            BulbManager.Instance.LightDownIndicator();
            BulbManager.Instance.RemoveBulb(bulb.GetComponent<BulbScript>(), this);

            interactable.enabled = false;
            gameObject.SetActive(false);

            TurnOffOutline();

        }
    }


    public void TurnOnOutline()
    {
        outlineOn = true;
    }

    public void TurnOffOutline()
    {
        outlineOn = false;
    }

    public void MoveLightBulb(GameObject newSpareBulb)
    {
        spareBulb = newSpareBulb;
        // spareBulb.GetComponent<SpareBulbScript>().SpareBulbReleased();
        PlayerPickUpScript.Instance.UpdateEndPosition(bulbPosition);
        inAnimation = true;

    }
}
