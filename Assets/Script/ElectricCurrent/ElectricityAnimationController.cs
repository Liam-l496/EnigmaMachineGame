using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;


public enum ElectricWireEnum
{
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
}


public class ElectricityAnimationController : MonoBehaviour
{
    public ElectricityController electricityController;
    public List<ElectricWireEnum> wireAnimationOrderList = new List<ElectricWireEnum>() {};
    public ElectricWireEnum[] rotorAnimationArray = {ElectricWireEnum.Rotor1, ElectricWireEnum.Rotor2, ElectricWireEnum.Rotor3, ElectricWireEnum.Rotor3Reverse, ElectricWireEnum.Rotor2Reverse, ElectricWireEnum.Rotor1Reverse};
    public List<int> wireIndexList = new List<int>();
    public int wireIndex;
    public int wireIndexDelete = 0;

    public GameManagerScript gameManagerScript;

    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;
    }


    void Update()
    {
        
        if (wireIndexDelete > 0)
        {
            if (electricityController.CurrentWireOff()) {
                RemoveFromList();
            }
        }

        if (wireIndex >= wireAnimationOrderList.Count) 
        {
            // print($"WIRE INDEX BIGGER {wireIndex} {wireAnimationOrderList.Count}");
            return; 
        }


        if (!electricityController.CurrentWireOn())
        {
            print("CURRENT WIRE IS ON");
            PrintElectricWires(wireAnimationOrderList);
            return;
        } 
        runAnimation(wireAnimationOrderList[wireIndex], wireIndexList[wireIndex]);
        // PrintElectricWires(wireAnimationOrderList);
        wireIndex++;
    }


    public void runAnimation(ElectricWireEnum nextAnimation, int wireAnimationIndex) 
    {
        // print($"NOW DOING ANIMATION: {nextAnimation} + {wireAnimationIndex}");
        if (nextAnimation == ElectricWireEnum.KeyToPlug)
        {
            electricityController.PowerOnWirePlugBoard(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugIn)
        {
            electricityController.PowerOnWirePlugBoardIn(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugOut)
        {
            electricityController.PowerOnWirePlugBoardOut(wireAnimationIndex);
        }
        // else if (nextAnimation == ElectricWireEnum.PlugToRotor)
        // {
        //     electricityController.PowerOnWirePlugBoardIn(wireAnimationIndex);
        // }

        // Lots of statements for rotors

        // else if (rotorAnimationArray.Contains(nextAnimation))
        // {
        //     RotorAnimation(nextAnimation, wireAnimationIndex);
        // }

        else if (nextAnimation == ElectricWireEnum.PlugInReverse)
        {
            electricityController.PowerOnWirePlugBoardOut(wireAnimationIndex, true);
        }
        else if (nextAnimation == ElectricWireEnum.PlugOutReverse)
        {
            electricityController.PowerOnWirePlugBoardIn(wireAnimationIndex, true);
        }

        else if (nextAnimation == ElectricWireEnum.PlugToKey)
        {
            electricityController.PowerOnWirePlugBoard(wireAnimationIndex, true);
        }

        else if (nextAnimation == ElectricWireEnum.KeyToBulb)
        {
            electricityController.PowerOnWireLightBulb(wireAnimationIndex);
        }
    }
    
    public void AddToList(ElectricWireEnum newAnimation, int newIndex)
    {
        wireAnimationOrderList.Add(newAnimation);
        wireIndexList.Add(newIndex);
    }

    public void RemoveFromList()
    {
        if (wireAnimationOrderList.Count <= 0) return;
        ElectricWireEnum removed = wireAnimationOrderList[0];
        int index = wireIndexList[0];
        RemoveAnimation(removed, index);
        wireAnimationOrderList.RemoveAt(0);
        wireIndexList.RemoveAt(0);
        wireIndex--;
        wireIndexDelete--;
        PrintElectricWires(wireAnimationOrderList);
        print($"REMOVEFROMLIST: {wireAnimationOrderList} {wireIndex} {wireIndexDelete}");
        if (wireIndex < 0) {
            wireIndex = 0;
        } 
    }

    public void RemoveAnimation(ElectricWireEnum nextAnimation, int wireAnimationIndex)
    {
         if (nextAnimation == ElectricWireEnum.KeyToPlug)
        {
            electricityController.PowerOffWirePlugBoard(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugIn)
        {
            electricityController.PowerOffWirePlugBoardIn(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugOut)
        {
            electricityController.PowerOffWirePlugBoardOut(wireAnimationIndex);
        }
        // else if (nextAnimation == ElectricWireEnum.PlugToRotor)
        // {
        //     electricityController.PowerOnWirePlugBoardIn(wireAnimationIndex);
        // }

        // Lots of statements for rotors

        // else if (rotorAnimationArray.Contains(nextAnimation))
        // {
        //     RotorAnimationOff(nextAnimation);
        // }

        else if (nextAnimation == ElectricWireEnum.PlugInReverse)
        {
            electricityController.PowerOffWirePlugBoardOut(wireAnimationIndex);
        }
        else if (nextAnimation == ElectricWireEnum.PlugOutReverse)
        {
            electricityController.PowerOffWirePlugBoardIn(wireAnimationIndex);
        }

        else if (nextAnimation == ElectricWireEnum.PlugToKey)
        {
            electricityController.PowerOffWirePlugBoard(wireAnimationIndex);
        }

        else if (nextAnimation == ElectricWireEnum.KeyToBulb)
        {
            electricityController.PowerOffWireLightBulb(wireAnimationIndex);
        }
    }

    public void RotorAnimation(ElectricWireEnum nextAnimation, int wireAnimationIndex)
    {
        int rotorIndex = 1;
        bool reverse = false;
        RotorCableAssignerScript rotor;
        if (nextAnimation == ElectricWireEnum.Rotor1)
        {
            rotorIndex = 1;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor2)
        {
            rotorIndex = 2;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor3)
        {
            rotorIndex = 3;
        }
        if (nextAnimation == ElectricWireEnum.Rotor1Reverse)
        {
            rotorIndex = 1;
            reverse = true;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor2Reverse)
        {
            rotorIndex = 2;
            reverse = true;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor3Reverse)
        {
            rotorIndex = 3;
            reverse = true;
        }

        rotor = gameManagerScript.ReturnRotor(rotorIndex).GetComponent<RotorCableAssignerScript>();
        electricityController.currentRope = rotor.LightUpRotor(wireAnimationIndex, reverse);
    }

    public void RotorAnimationOff(ElectricWireEnum nextAnimation)
    {
        int rotorIndex = 1;
        RotorCableAssignerScript rotor;
        if (nextAnimation == ElectricWireEnum.Rotor1)
        {
            rotorIndex = 1;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor2)
        {
            rotorIndex = 2;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor3)
        {
            rotorIndex = 3;
        }
        if (nextAnimation == ElectricWireEnum.Rotor1Reverse)
        {
            rotorIndex = 1;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor2Reverse)
        {
            rotorIndex = 2;
        }
        else if (nextAnimation == ElectricWireEnum.Rotor3Reverse)
        {
            rotorIndex = 3;

        }

        rotor = gameManagerScript.ReturnRotor(rotorIndex).GetComponent<RotorCableAssignerScript>();
        electricityController.currentRope = rotor.TurnOffRotor();
    }

    public void KeyRelease()
    {
        wireIndexDelete += wireAnimationOrderList.Count;
        wireIndex = wireAnimationOrderList.Count;
        electricityController.currentWire = null;
    }

    public void PrintElectricWires(List<ElectricWireEnum> wires)
    {
        Debug.Log("WIRE LIST : " + string.Join(", ", wires) + " " + wires.Count);
    }
}