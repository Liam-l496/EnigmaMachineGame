using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class BulbManager : MonoBehaviour
{
    public static BulbManager Instance;
    public List<BulbScript> BulbScriptList = new List<BulbScript>();
    public List<BulbPositionIndicator> BulbsPositionIndicatorList = new List<BulbPositionIndicator>();
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddBulb(BulbScript newBulb, BulbPositionIndicator newBulbIndi)
    {
        if (newBulb == null || newBulbIndi == null) return;
        BulbScriptList.Add(newBulb);
        BulbsPositionIndicatorList.Add(newBulbIndi);
    }

    public void RemoveBulb(BulbScript newBulb, BulbPositionIndicator newBulbIndi)
    {
        if (newBulb == null || newBulbIndi == null) return;
        BulbScriptList.Remove(newBulb);
        BulbsPositionIndicatorList.Remove(newBulbIndi);
    }

    public void LightUpIndicator()
    {
        foreach (BulbPositionIndicator bulbP in BulbsPositionIndicatorList)
        {
            bulbP.gameObject.SetActive(true);
            bulbP.TurnOnOutline();
            bulbP.interactable.enabled = true;
        }
    }

    public void LightDownIndicator()
    {
        foreach (BulbPositionIndicator bulbP in BulbsPositionIndicatorList)
        {
            print("QWERTY Light down indiciator");
            bulbP.TurnOffOutline();
            bulbP.interactable.enabled = false;
            bulbP.gameObject.SetActive(false);
        }
    }
}
