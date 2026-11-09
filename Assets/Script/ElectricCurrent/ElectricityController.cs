
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ElectricityController : MonoBehaviour
{
    [Header("Test wires")]
    public SplineElectricityWire test1;
    public SplineElectricityWire test2;


    [Header("Electricity Wire")]
    [SerializeField]
    private SplineElectricityWire electricityWire;
    [SerializeField] private RopeElectricityWire electricityRope;

    public SplineElectricityWire currentWire;
    public RopeElectricityWire currentRope;

    public SplineElectricityWire currentWireOff;
    public RopeElectricityWire currentRopeOff;


    [Header("Key Switch to PlugBoard Wire")]
    public SplineElectricityWire[] wireKeyToPlugBoard;
    private Dictionary<int, bool> dictionaryKeyToPlugBoard = new Dictionary<int, bool>();

    [Header("Key to LightBulb Wire")]
    public SplineElectricityWire[] wireBulb;
    private Dictionary<int, bool> dictionaryBulb = new Dictionary<int, bool>();

    [Header("Plug Board Wire IN")]
    public SplineElectricityWire[] wirePlugBoardIn;
    private Dictionary<int, bool> dictionaryPlugBoardIn = new Dictionary<int, bool>();

    [Header("Plug Board Wire OUT")]
    public SplineElectricityWire[] wirePlugBoardOut;
    private Dictionary<int, bool> dictionaryPlugBoardOut = new Dictionary<int, bool>();

    [Header("Light Bulb Wire")]
    public SplineElectricityWire[] wireToBulb;
    private Dictionary<int, bool> dictionaryToBulb = new Dictionary<int, bool>();

    [Header("Battery Wires")]
    public SplineElectricityWire[] wireBattery;
    // in order: Battery, bottom, ladderBT, middle, ladderMT, top, returnWire

    [Header("Key Input Manager Script")]
    public KeyInputManagerScript keyInputManagerScript;



    // public void EmergencyPowerOff()
    // {
    //     electricityWire.TurnElectricityOffImmediately();
    //     electricityRope.TurnElectricityOffImmediately();
    // }

    void Start()
    {
        int wireIndex = 0;
        foreach (SplineElectricityWire wire in wireKeyToPlugBoard)
        {
            dictionaryKeyToPlugBoard[wireIndex] = false;
            dictionaryBulb[wireIndex] = false;
            dictionaryPlugBoardIn[wireIndex] = false;
            dictionaryPlugBoardOut[wireIndex] = false;
            dictionaryToBulb[wireIndex] = false;
            wireIndex++;
        }
        UpdateEnabledState();
        
        // test1.StartElectricity();
        // test2.StartElectricity();
    }


/// ////////////////////////////////////////////////////

    public void KillWire(ElectricWireEnum enumInput, int wireIndex) {
        SplineElectricityWire[] wire;
        Dictionary<int, bool> dictionary;
        switch (enumInput)
        {
            case ElectricWireEnum.KeyToPlug:
            case ElectricWireEnum.PlugToKey:
                wire = wireKeyToPlugBoard;
                dictionary = dictionaryKeyToPlugBoard;
                break;
            case ElectricWireEnum.PlugIn:
            case ElectricWireEnum.PlugOutReverse:
                wire = wirePlugBoardIn;
                dictionary = dictionaryPlugBoardIn;
                break;
            case ElectricWireEnum.PlugOut:
            case ElectricWireEnum.PlugInReverse:
                wire = wirePlugBoardOut;
                dictionary = dictionaryPlugBoardOut;
                break;
            case ElectricWireEnum.KeyToBulb:
                wire = wireBulb;
                dictionary = dictionaryBulb;
                break;
            case ElectricWireEnum.Bulb:
                wire = wireToBulb;
                dictionary = dictionaryToBulb;
                break;
            default:
                return;
        }
        
        wire[wireIndex].TurnElectricityOffImmediately();
        dictionary[wireIndex] = false;
    }


/// ////////////////////////////////////////////////////

   public SplineElectricityWire PowerOnBatteryWire(int wireIndex)
    {
        SplineElectricityWire wire = wireBattery[wireIndex];
        if (!wire.IsOn) wire.StartElectricity();
        return wire;
    }

    public SplineElectricityWire PowerOffBatteryWire(int wireIndex)
    {
        SplineElectricityWire wire = wireBattery[wireIndex];
        if (!keyInputManagerScript.noKeyPressed) return null;
        wire.StopElectricity();
        return wire;
    }



/// ////////////////////////////////////////////////////
    public SplineElectricityWire PowerOnWirePlugBoard(int wireIndex, bool comingIn=false)
    {
        SplineElectricityWire wire = wireKeyToPlugBoard[wireIndex];
        dictionaryKeyToPlugBoard[wireIndex] = true;
        UpdateEnabledState();
        wire.SetReverseDirection(comingIn, true);
        wire.StartElectricity();
        return wire;
    }

    public SplineElectricityWire PowerOffWirePlugBoard(int wireIndex)
    {
        SplineElectricityWire wire = wireKeyToPlugBoard[wireIndex];
        dictionaryKeyToPlugBoard[wireIndex] = false;
        wire.StopElectricity();
        UpdateEnabledState();
        return wire;
    }

/// ////////////////////////////////////////////////////

    public SplineElectricityWire PowerOnWireLightBulb(int wireIndex)
    {
        SplineElectricityWire wire = wireBulb[wireIndex];
        dictionaryBulb[wireIndex] = true;
        UpdateEnabledState();
        wire.StartElectricity();
        return wire;
    }

    public SplineElectricityWire PowerOffWireLightBulb(int wireIndex)
    {
        SplineElectricityWire wire = wireBulb[wireIndex];
        dictionaryBulb[wireIndex] = false;
        wire.StopElectricity();
        UpdateEnabledState();
        return wire;
    }

/// ////////////////////////////////////////////////////

    public SplineElectricityWire PowerOnWirePlugBoardIn(int wireIndex, bool comingIn=false)
    {
        SplineElectricityWire wire = wirePlugBoardIn[wireIndex];
        dictionaryPlugBoardIn[wireIndex] = true;
        UpdateEnabledState();
        wire.SetReverseDirection(comingIn, true);
        wire.StartElectricity();
        return wire;
    }

    public SplineElectricityWire PowerOffWirePlugBoardIn(int wireIndex)
    {
        SplineElectricityWire wire = wirePlugBoardIn[wireIndex];
        dictionaryPlugBoardIn[wireIndex] = false;
        wire.StopElectricity();
        UpdateEnabledState();
        return wire;
    }

/// ////////////////////////////////////////////////////

    public SplineElectricityWire PowerOnWirePlugBoardOut(int wireIndex, bool comingIn=false)
    {
        SplineElectricityWire wire = wirePlugBoardOut[wireIndex];
        dictionaryPlugBoardOut[wireIndex] = true;
        UpdateEnabledState();
        wire.SetReverseDirection(comingIn, true);
        wire.StartElectricity();
        return wire;
    }

    public SplineElectricityWire PowerOffWirePlugBoardOut(int wireIndex)
    {
        SplineElectricityWire wire = wirePlugBoardOut[wireIndex];
        dictionaryPlugBoardOut[wireIndex] = false;
        wire.StopElectricity();
        UpdateEnabledState();
        return wire;
    }

/// ////////////////////////////////////////////////////

    public SplineElectricityWire PowerOnWireToBulb(int wireIndex)
    {
        SplineElectricityWire wire = wireToBulb[wireIndex];
        dictionaryPlugBoardOut[wireIndex] = true;
        UpdateEnabledState();
        wire.StartElectricity();
        return wire;
    }

    public SplineElectricityWire PowerOffWireToBulb(int wireIndex)
    {
        SplineElectricityWire wire = wireToBulb[wireIndex];
        dictionaryPlugBoardOut[wireIndex] = false;
        wire.StopElectricity();
        UpdateEnabledState();
        return wire;
    }






/// ////////////////////////////////////////////////////


    public void UpdateEnabledState()
    {
        // foreach (var (wireIndex, active) in dictionaryKeyToPlugBoard)
        // {
        //     wireKeyToPlugBoard[wireIndex].enabled = active;
        // }
        // foreach (var (wireIndex, active) in dictionaryBulb)
        // {
        //     wireBulb[wireIndex].enabled = active;
        // }
        // foreach (var (wireIndex, active) in dictionaryPlugBoardIn)
        // {
        //     wirePlugBoardIn[wireIndex].enabled = active;
        // }
        // foreach (var (wireIndex, active) in dictionaryPlugBoardOut)
        // {
        //     wirePlugBoardOut[wireIndex].enabled = active;
        // }
    } 

/// ////////////////////////////////////////////////////
    public bool CurrentWireOn()
    {
        if (currentWire == null && currentRope == null) return true; 

        else if (currentRope == null) {
            if (currentWire.IsOn)
            {
                currentWire = null;
                return true;
            }
            print($"CURRENT WIRE IS THIS ONE: {currentWire}");
            return false;
        }
        else
        {
            if (currentRope.IsOn)
            {
                currentRope = null;
                return true;
            }
            return false;
        }
    }

    public bool CurrentWireOff()
    {
        if (currentWireOff == null && currentRopeOff == null) return true;
        // else if (currentRopeOff == null) return currentWireOff.IsOff;
        // return currentRopeOff.IsOff;

        else if (currentRopeOff == null) {
            if (currentWireOff.IsOff)
            {
                currentWireOff = null;
                return true;
            }
            return false;
        }
        else
        {
            if (currentRopeOff.IsOff)
            {
                currentRopeOff = null;
                return true;
            }
            return false;
        }
    }

}
 