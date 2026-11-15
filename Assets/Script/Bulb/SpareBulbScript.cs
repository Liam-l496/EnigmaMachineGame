using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpareBulbScript : MonoBehaviour
{
    private Interactable interactable;
    public bool bulbHeld = false;
    public bool currentlyHeld = false;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        currentlyHeld = interactable.isSelected;
        if (!interactable.isSelected) {
            if (bulbHeld) SpareBulbReleased();
            return;
        }
        if (!bulbHeld) SpareBulbHeld();
    }

    public void SpareBulbHeld()
    {
        bulbHeld = true;
        BulbManager.Instance.LightUpIndicator();
    }

    public void SpareBulbReleased()
    {
        bulbHeld = false;    
        BulbManager.Instance.LightDownIndicator();
    }
    
}
