using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RotorSwappingControllerScript : MonoBehaviour
{
    public static RotorSwappingControllerScript Instance;


    [Header("Swapping Point Script")]
    public RotorSwappingPointScript swappingPoint1;
    public RotorSwappingPointScript swappingPoint2;
    public RotorSwappingPointScript swappingPoint3;
    public RotorSwappingPointScript swappingPoint4;
    public RotorSwappingPointScript swappingPoint5;

    [Header("Swapping Script")]
    public RotorSwappingScript[] rotorSwappingArray = new RotorSwappingScript[5];
    public bool currentlyHolding = false;
    public bool dropEverything = false;


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

    void Update()
    {
        currentlyHolding = FindCurrentlyHolding();
        dropEverything = FindCurrentlyHolding2();
    }

    public bool FindCurrentlyHolding()
    {
        // foreach (RotorSwappingScript rotor in rotorSwappingArray)
        // {
        //     if (rotor.draggingInitialised)
        //     {
        //         return true;
        //     }
        // }
        foreach (RotorSwappingScript rotor in rotorSwappingArray)
        {
            if (rotor.GetComponent<RotorMasterScript>().IsSelected)
            {
                return true;
            }
        }
        return false;
    }

        public bool FindCurrentlyHolding2()
    {
        bool hit = false;
        foreach (RotorSwappingScript rotor in rotorSwappingArray)
        {
            if (rotor.GetComponent<RotorMasterScript>().IsSelected)
            {
                if (hit)
                {
                    return true;
                }
                hit = true;
            }
        }
        return false;
    }


    public void CheckSwapping(RotorSwappingScript movingRotor, RotorSwappingScript stationaryRotor)
    {
        RotorMasterScript rotorMaster1 = movingRotor.rotorMasterScript;
        RotorMasterScript rotorMaster2 = stationaryRotor.rotorMasterScript;

        int rotor1Position = rotorMaster1.rotorPosition;
        int rotor2Position = rotorMaster2.rotorPosition;

        if (Mathf.Abs(rotor1Position - rotor2Position) <= 1)
        {
            DetermineSwappingPoint(rotor1Position).SwapPosition(movingRotor, stationaryRotor);
            return;
        }

        int index = 0;
        RotorSwappingPointScript tempPoint;
        RotorSwappingScript tempRotor1;
        RotorSwappingScript tempRotor2;
        // Right to Left
        print($"ROTOR1 POSITION: {rotor1Position}  ROTOR2 POSITION: {rotor2Position}");
        if (rotor1Position < rotor2Position)
        {
            tempRotor1 = movingRotor;
            for (int zeroIndex = rotor1Position; zeroIndex < rotor2Position; zeroIndex++)
            {
                index = zeroIndex + 1;
                tempPoint = DetermineSwappingPoint(index);
                tempRotor2 = DetermineRotorSwappingScript(index);
                tempPoint.SwapPosition(tempRotor1, tempRotor2);
            }
        }
        else if (rotor1Position > rotor2Position)
        {
            tempRotor1 = movingRotor;
            for (int zeroIndex = rotor1Position; zeroIndex > rotor2Position; zeroIndex--)
            {
                index = zeroIndex - 1;
                tempPoint = DetermineSwappingPoint(index);
                tempRotor2 = DetermineRotorSwappingScript(index);
                print($"zeroIndex: {zeroIndex}    index: {index}");
                tempPoint.SwapPosition(tempRotor1, tempRotor2);
            }
        }



    }

    private RotorSwappingPointScript DetermineSwappingPoint(int rotorPosition)
    {
        if (rotorPosition == 1) return swappingPoint1;
        if (rotorPosition == 2) return swappingPoint2;
        if (rotorPosition == 3) return swappingPoint3;
        if (rotorPosition == 4) return swappingPoint4;
        return swappingPoint5;
    }

    public RotorSwappingScript DetermineRotorSwappingScript(int rotorPosition)
    {
        foreach (RotorSwappingScript rotor in rotorSwappingArray)
        {
            if (rotor.GetComponent<RotorMasterScript>().rotorPosition == rotorPosition)
            {
                return rotor;
            }
        }
        return null;
    }

}
