using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GogoGaga.OptimizedRopesAndCables;
using Unity.VisualScripting;
using UnityEngine;


public enum ElectricWireEnum
{
    Battery,
    KeyToPlug,
    PlugIn,
    CablePluggedIn,
    PlugOut,
    PlugToRotor,
    InputWheel,
    Rotor1,
    Rotor2,
    Rotor3,
    Reflector,
    Rotor3Reverse,
    Rotor2Reverse,
    Rotor1Reverse,
    InputWheelReverse,
    RotorToPlug,
    PlugInReverse,
    PlugOutReverse,
    PlugToKey,
    KeyToBulb,
    Bulb
}


public class ElectricityAnimationController : MonoBehaviour
{
    public ElectricityController electricityController;
    public List<ElectricWireEnum> wireAnimationOrderList = new List<ElectricWireEnum>() {}; 
    public ElectricWireEnum[] rotorAnimationArray = {ElectricWireEnum.Rotor1, ElectricWireEnum.Rotor2, ElectricWireEnum.Rotor3, ElectricWireEnum.Rotor3Reverse, ElectricWireEnum.Rotor2Reverse, ElectricWireEnum.Rotor1Reverse};
    public List<int> wireIndexList = new List<int>();
    public int wireIndex;
    public int wireIndexDelete = 0;
    private bool killSwitch = false;

    public SplineElectricityWire currentWireOn = null;
    public SplineElectricityWire currentWireOff = null;
    public RopeElectricityWire currentRopeOn = null;
    public RopeElectricityWire currentRopeOff = null;
    public bool stopCurrentFlow = false;
    public GameManagerScript gameManagerScript;

    void Start()
    {
        Debug.Log($"[{gameObject.name}] Start() initialized.");
        gameManagerScript = GameManagerScript.Instance;
        electricityController = FindFirstObjectByType<ElectricityController>();

        if (gameManagerScript == null) Debug.LogError($"[{gameObject.name}] GameManagerScript.Instance is NULL!");
        if (electricityController == null) Debug.LogError($"[{gameObject.name}] ElectricityController not found in scene!");
    }


    void Update()
    {
        if (killSwitch)
        {
            Debug.Log($"[{gameObject.name}] Update skipped because killSwitch is TRUE.");
            killSwitch = false;
            return;
        }
        
        if (stopCurrentFlow)
        {
            Debug.Log($"[{gameObject.name}] Update: stopCurrentFlow is TRUE. checking currentWireOff status...");
            if (currentWireOff == null || currentWireOff.IsOff) {
                Debug.Log($"[{gameObject.name}] Update: currentWireOff is null or IsOff. Calling RemoveFromList().");
                RemoveFromList();
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Update: Waiting for currentWireOff ({currentWireOff.name}) to finish turning off.");
            }
            return;
        }

        if (wireIndex >= wireAnimationOrderList.Count) 
        {
            // Debug.Log($"[{gameObject.name}] Update: wireIndex ({wireIndex}) >= list count ({wireAnimationOrderList.Count}). Waiting for new commands.");
            return; 
        }

        if ((currentWireOn != null && !currentWireOn.IsOn) || (currentRopeOn != null && !currentRopeOn.IsOn)) {
            return;
        }

        // Debug.Log($"[{gameObject.name}] Update: Step {wireIndex}/{wireAnimationOrderList.Count} -> Running animation: {wireAnimationOrderList[wireIndex]} (Target Index: {wireIndexList[wireIndex]})");
        runAnimation(wireAnimationOrderList[wireIndex], wireIndexList[wireIndex]);
        wireIndex++;
    }


    public void runAnimation(ElectricWireEnum nextAnimation, int wireAnimationIndex) 
    {
        Debug.Log($"[{gameObject.name}] runAnimation() called: Enum={nextAnimation}, Index={wireAnimationIndex}");


        if (nextAnimation == ElectricWireEnum.Battery)
        {
            currentWireOn = electricityController.PowerOnBatteryWire(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.KeyToPlug)
        {
            currentWireOn = electricityController.PowerOnWirePlugBoard(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugIn)
        {
            currentWireOn = electricityController.PowerOnWirePlugBoardIn(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.CablePluggedIn)
        {
            print("RUNNING ROPE ELECTRICITY");
            currentRopeOn = gameManagerScript.PlugBoardObjectDictionary[wireAnimationIndex].StartElectricity(wireAnimationIndex);
            currentWireOn = null;
            return;
        }
        else if (nextAnimation == ElectricWireEnum.PlugOut)
        {
            currentWireOn = electricityController.PowerOnWirePlugBoardOut(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugInReverse)
        {
            currentWireOn = electricityController.PowerOnWirePlugBoardOut(wireAnimationIndex, true);
        }
        else if (nextAnimation == ElectricWireEnum.PlugOutReverse)
        {
            currentWireOn = electricityController.PowerOnWirePlugBoardIn(wireAnimationIndex, true);
        }
        else if (nextAnimation == ElectricWireEnum.PlugToKey)
        {
            currentWireOn = electricityController.PowerOnWirePlugBoard(wireAnimationIndex, true);
        }
        else if (nextAnimation == ElectricWireEnum.KeyToBulb)
        {
            currentWireOn = electricityController.PowerOnWireLightBulb(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.Bulb)
        {
            currentWireOn = electricityController.PowerOnWireToBulb(wireAnimationIndex);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] runAnimation() -> Unhandled enum step: {nextAnimation}");
        }
        currentRopeOn = null;

        Debug.Log($"[{gameObject.name}] runAnimation() complete. Assigned currentWireOn: {(currentWireOn != null ? currentWireOn.name : "NULL")}");
    }

    public void RemoveFromList()
    {
        Debug.Log($"RemoveFromList() invoked. Delete index: {wireIndexDelete}, List Count: {wireAnimationOrderList.Count}");

        if (wireIndexDelete >= wireAnimationOrderList.Count)
        {
            Debug.Log("All wires have been switched off.");

            stopCurrentFlow = false;
            wireIndexDelete = 0;

            currentWireOn = null;
            currentWireOff = null;

            return;
        }

        ElectricWireEnum removed = wireAnimationOrderList[wireIndexDelete];
        int index = wireIndexList[wireIndexDelete];

        RemoveAnimation(removed, index);

        wireIndexDelete++;
    }

    public void RemoveAnimation(ElectricWireEnum nextAnimation, int wireAnimationIndex)
    {
        Debug.Log($"[{gameObject.name}] RemoveAnimation() called: Enum={nextAnimation}, WireIndex={wireAnimationIndex}");

        if (nextAnimation == ElectricWireEnum.Battery)
        {
            currentWireOff = electricityController.PowerOffBatteryWire(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.KeyToPlug)
        {
            currentWireOff = electricityController.PowerOffWirePlugBoard(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugIn)
        {
            currentWireOff = electricityController.PowerOffWirePlugBoardIn(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.CablePluggedIn)
        {
            print("STOPPING ROPE ELECTRICITY");
            currentRopeOn = gameManagerScript.PlugBoardObjectDictionary[wireAnimationIndex].StopElectricity();
            currentWireOn = null;
            return;
        }
        else if (nextAnimation == ElectricWireEnum.PlugOut)
        {
            currentWireOff = electricityController.PowerOffWirePlugBoardOut(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugInReverse)
        {
            currentWireOff = electricityController.PowerOffWirePlugBoardOut(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugOutReverse)
        {
            currentWireOff = electricityController.PowerOffWirePlugBoardIn(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugToKey)
        {
            currentWireOff = electricityController.PowerOffWirePlugBoard(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.KeyToBulb)
        {
            currentWireOff = electricityController.PowerOffWireLightBulb(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.Bulb)
        {
            currentWireOff = electricityController.PowerOffWireToBulb(wireAnimationIndex);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] RemoveAnimation() -> Unhandled enum step: {nextAnimation}");
        }

        Debug.Log($"[{gameObject.name}] RemoveAnimation() complete. Assigned currentWireOff: {(currentWireOff != null ? currentWireOff.name : "NULL")}");
    }

    public void RotorAnimation(ElectricWireEnum nextAnimation, int wireAnimationIndex)
    {
        int rotorIndex = 1;
        bool reverse = false;
        RotorCableAssignerScript rotor;

        if (nextAnimation == ElectricWireEnum.Rotor1) rotorIndex = 1;
        else if (nextAnimation == ElectricWireEnum.Rotor2) rotorIndex = 2;
        else if (nextAnimation == ElectricWireEnum.Rotor3) rotorIndex = 3;

        if (nextAnimation == ElectricWireEnum.Rotor1Reverse) { rotorIndex = 1; reverse = true; }
        else if (nextAnimation == ElectricWireEnum.Rotor2Reverse) { rotorIndex = 2; reverse = true; }
        else if (nextAnimation == ElectricWireEnum.Rotor3Reverse) { rotorIndex = 3; reverse = true; }

        Debug.Log($"[{gameObject.name}] RotorAnimation(): Enum={nextAnimation}, RotorIndex={rotorIndex}, Reverse={reverse}");

        rotor = gameManagerScript.ReturnRotor(rotorIndex).GetComponent<RotorCableAssignerScript>();
        electricityController.currentRope = rotor.LightUpRotor(wireAnimationIndex, reverse);
    }

    public void RotorAnimationOff(ElectricWireEnum nextAnimation)
    {
        int rotorIndex = 1;
        RotorCableAssignerScript rotor;

        if (nextAnimation == ElectricWireEnum.Rotor1) rotorIndex = 1;
        else if (nextAnimation == ElectricWireEnum.Rotor2) rotorIndex = 2;
        else if (nextAnimation == ElectricWireEnum.Rotor3) rotorIndex = 3;

        if (nextAnimation == ElectricWireEnum.Rotor1Reverse) rotorIndex = 1;
        else if (nextAnimation == ElectricWireEnum.Rotor2Reverse) rotorIndex = 2;
        else if (nextAnimation == ElectricWireEnum.Rotor3Reverse) rotorIndex = 3;

        Debug.Log($"[{gameObject.name}] RotorAnimationOff(): Enum={nextAnimation}, RotorIndex={rotorIndex}");

        rotor = gameManagerScript.ReturnRotor(rotorIndex).GetComponent<RotorCableAssignerScript>();
        electricityController.currentRope = rotor.TurnOffRotor();
    }

    public void KeyRelease()
    {
        Debug.Log($"[{gameObject.name}] KeyRelease() triggered! Resetting wireIndexDelete = 0 and setting stopCurrentFlow = true.");
        wireIndexDelete = 0;
        wireIndex = wireAnimationOrderList.Count;
        stopCurrentFlow = true;

        Debug.Log(
            $"KEY RELEASE: {gameObject.name} | " +
            $"stopCurrentFlow: {stopCurrentFlow} | " +
            $"wireIndex: {wireIndex} | " +
            $"wireIndexDelete: {wireIndexDelete} | " +
            $"List Count: {wireAnimationOrderList.Count}"
        );
    }

    public void PrintElectricWires(List<ElectricWireEnum> wires)
    {
        if (wires == null)
        {
            Debug.LogWarning($"[{gameObject.name}] PrintElectricWires() -> Wire list is NULL!");
            return;
        }
        Debug.Log($"[{gameObject.name}] WIRE LIST (Count: {wires.Count}): " + string.Join(", ", wires));
    }

    // public void KillAllWires()
    // {
    //     Debug.Log($"[{gameObject.name}] KillAllWires() triggered! Killing {wireAnimationOrderList.Count} wires and resetting state.");
    //     killSwitch = true;
    //     for (int i = 0; i < wireAnimationOrderList.Count; i++)
    //     {
    //         Debug.Log($"[{gameObject.name}] Killing wire {i}: Enum={wireAnimationOrderList[i]}, Index={wireIndexList[i]}");
    //         electricityController.KillWire(wireAnimationOrderList[i], wireIndexList[i]);
    //     }
    //     wireAnimationOrderList.Clear();
    //     wireIndexList.Clear();
    //     wireIndex = 0;
    //     wireIndexDelete = 0;
    //     currentWireOn = null;
    //     currentWireOff = null;
    //     Debug.Log($"[{gameObject.name}] KillAllWires() completed.");
    // }

    public void KillAllWires()
    {
        Debug.Log($"[{gameObject.name}] KillAllWires() triggered!");

        killSwitch = true;

        for (int i = 0; i < wireAnimationOrderList.Count; i++)
        {
            electricityController.KillWire(
                wireAnimationOrderList[i],
                wireIndexList[i]
            );
        }

        wireAnimationOrderList.Clear();
        wireIndexList.Clear();

        wireIndex = 0;
        wireIndexDelete = 0;

        currentWireOn = null;
        currentWireOff = null;

        currentRopeOn = null;
        currentRopeOff = null;

        stopCurrentFlow = false;

        Debug.Log("KillAllWires() completed.");
    }

}