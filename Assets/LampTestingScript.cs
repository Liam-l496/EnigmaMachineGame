using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampTestingScript : MonoBehaviour
{

    public Interactable interactable;
    private BulbScript bulbEquipped;
    private bool bulbTurnedOn = false;
    // Start is called before the first frame update
    void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LateUpdate()
    {
        if (bulbEquipped && !PlayerPickUpScript.Instance.inAnimation && !bulbTurnedOn)
        {
            print("TESTING LIGHT ON");
            bulbEquipped.lightOn = true;
            bulbEquipped.UpdateMaterial();
            bulbTurnedOn = true;
        }
    }

    public void ClickedWithBulb(BulbScript bulb)
    {
        // BulbScript bulb = PlayerPickUpScript.Instance.heldObject.GetComponent<BulbScript>();
        PlayerPickUpScript.Instance.UpdateEndPosition(transform);
        bulbEquipped = bulb;
    }

    public void DeactivateLampTestingScript()
    {
        bulbEquipped.lightOn = false;
        bulbEquipped.UpdateMaterial();
        bulbTurnedOn = false;
        bulbEquipped = null;
    }
}
