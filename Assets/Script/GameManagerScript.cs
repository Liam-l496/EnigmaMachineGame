using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    public static GameManagerScript Instance;

    [Header("Actuator Bar")]
    [SerializeField] ActuatorBarScript actuatorBar;

    private string sentenceTyped = "";
    [Header("Rotors Swapping")]
    private RotorScript rotorSelected1;
    private RotorScript rotorSelected2; 
    // private bool swapping = false;

    [Header("Rotors")]
    [SerializeField] RotorScript rotor1;
    [SerializeField] RotorScript rotor2;
    [SerializeField] RotorScript rotor3;
    [SerializeField] int notchRequired = 10;
    bool spinRotor2 = false;
    bool spinRotor3 = false;



    public bool powerOn = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpinRotor()
    {
        spinRotor2 = false;
        spinRotor3 = false;
        if (rotor1.notch == (rotor1.rotorValue-1))
        {
            spinRotor2 = true;
        }
        if (rotor2.notch == (rotor2.rotorValue-1))
        {
            spinRotor2 = true;
            spinRotor3 = true;
        }
        rotor1.SpinRotorOnce();
        if(spinRotor2) rotor2.SpinRotorOnce();
        if(spinRotor3) rotor3.SpinRotorOnce();

    }

    public ActuatorBarScript.AnimationState ActuatorBarRotateState(bool spinRotor2, bool spinRotor3)
    {
        if (!spinRotor2 && !spinRotor3)
        {
            return ActuatorBarScript.AnimationState.OneRotate;
        }
        else if (spinRotor2 && !spinRotor3)
        {
            return ActuatorBarScript.AnimationState.OneTwoRotate;
        }
        else if (spinRotor2 && spinRotor3)
        {
            return ActuatorBarScript.AnimationState.OneTwoThreeRotate;
        }
        
        return ActuatorBarScript.AnimationState.OneRotate;
    }

    public ActuatorBarScript.AnimationState ActuatorBarReleaseState()
    {
        if (rotor1.notchPosition == notchRequired)
        {
            return ActuatorBarScript.AnimationState.OneTwoRelease;
        }
        else if (rotor2.notchPosition == notchRequired)
        {
            return ActuatorBarScript.AnimationState.OneTwoThreeRelease;
        }
        return ActuatorBarScript.AnimationState.OneRelease;
        
    }

    public void KeyPress(string keyPressed)
    {
        // rotor1.SpinRotorOnce();
        if (powerOn) {
            sentenceTyped += keyPressed;
        }
        SpinRotor();
        ActuatorBarScript.AnimationState state = ActuatorBarRotateState(spinRotor2, spinRotor3);
        print(state);
        actuatorBar.AnimateActuatorBarRotate(state);
        FindEncryptedText(keyPressed);
        // print("Sentence Typed: " + sentenceTyped);
    }

    public void KeyRelease()
    {
        ActuatorBarScript.AnimationState state = ActuatorBarReleaseState();
        print(state);
        actuatorBar.AnimateActuatorBarRelease(state);
    }

    public void ClearRotorSelected()
    {
        if (rotorSelected1 != null) rotorSelected1.selectedForSwapping = false;
        if (rotorSelected2 != null) rotorSelected2.selectedForSwapping = false;

        rotorSelected1 = null;
        rotorSelected2 = null;
    }

    public void SwapPosition(RotorScript movingRotor, RotorScript stationaryRotor)
    {
        Transform tempPositionLifted = stationaryRotor.RotorPositionLifted;
        Transform tempSwappingTop = stationaryRotor.RotorPositionSwappingTop;
        Transform tempStationaryPos = stationaryRotor.RotorPositionStationary;
        int tempRotorPlace = stationaryRotor.rotorPlace;

        stationaryRotor.RotorPositionLifted = movingRotor.RotorPositionLifted;
        stationaryRotor.RotorPositionSwappingTop = movingRotor.RotorPositionSwappingTop;
        stationaryRotor.RotorPositionStationary = movingRotor.RotorPositionStationary;
        stationaryRotor.rotorPlace = movingRotor.rotorPlace;

        movingRotor.RotorPositionLifted = tempPositionLifted;
        movingRotor.RotorPositionSwappingTop = tempSwappingTop;
        movingRotor.RotorPositionStationary = tempStationaryPos;
        movingRotor.rotorPlace = tempRotorPlace;

        AssignRotorPlace(stationaryRotor);
        AssignRotorPlace(movingRotor);

        // stationaryRotor.MoveRotor(stationaryRotor.RotorPositionLifted);
        stationaryRotor.RotorStationaryPosition();
    }
    public void AssignRotorPlace(RotorScript rotor) {
        if (rotor.rotorPlace == 1)
        {
            rotor1 = rotor;
        }
        else if(rotor.rotorPlace == 2)
        {
            rotor2 = rotor;
        }
        else if (rotor.rotorPlace == 3)
        {
            rotor3 = rotor;
        }
    }

    public int FindEncryptedText(string keyPressed)
    {
        // int plainChar = keyPressed[0] - 'A';
        // int pass1 = rotor1.PlainToCipher(plainChar);
        // int pass2 = rotor2.PlainToCipher(pass1);
        // int pass3 = rotor3.PlainToCipher(pass2);
        // int pass4 = Reflector[pass3];
        // int pass5 = rotor3.CipherToPlain(pass4);
        // int pass6 = rotor2.CipherToPlain(pass5);
        // int pass7 = rotor1.CipherToPlain(pass6);
        // print("PLAIN TEXT: " + plainChar + "    CIPHER TEXT:    " + pass7);
        
        int plainChar = keyPressed[0] - 'A';
        print("Key Pressed: " + (char)(plainChar + 'A') + " (" + plainChar + ")");

        int pass1 = rotor1.PlainToCipher(plainChar);
        print("Pass 1 (Rotor 1 Forward): " + (char)(pass1 + 'A') + " (" + pass1 + ")");

        int pass2 = rotor2.PlainToCipher(pass1);
        print("Pass 2 (Rotor 2 Forward): " + (char)(pass2 + 'A') + " (" + pass2 + ")");

        int pass3 = rotor3.PlainToCipher(pass2);
        print("Pass 3 (Rotor 3 Forward): " + (char)(pass3 + 'A') + " (" + pass3 + ")");

        int pass4 = Reflector[pass3];
        print("Pass 4 (Reflector):       " + (char)(pass4 + 'A') + " (" + pass4 + ")");

        int pass5 = rotor3.CipherToPlain(pass4);
        print("Pass 5 (Rotor 3 Reverse): " + (char)(pass5 + 'A') + " (" + pass5 + ")");

        int pass6 = rotor2.CipherToPlain(pass5);
        print("Pass 6 (Rotor 2 Reverse): " + (char)(pass6 + 'A') + " (" + pass6 + ")");

        int pass7 = rotor1.CipherToPlain(pass6);
        print("Pass 7 (Rotor 1 Reverse): " + (char)(pass7 + 'A') + " (" + pass7 + ")");

        print("PLAIN TEXT: " + (char)(plainChar + 'A') + "    CIPHER TEXT:    " + (char)(pass7 + 'A'));
        
        return pass7;
    }

    public Dictionary<int, int> Reflector = new Dictionary<int, int>()
    {
        {0, 24}, {1, 17}, {2, 20}, {3, 7}, {4, 16}, {5, 18},
        {6,  11}, {7, 3}, {8, 15}, {9, 23},{10, 13}, {11, 6},
        {12, 14}, {13, 10}, {14, 12}, {15, 8}, {16, 4}, {17, 1},
        {18, 5}, {19, 21}, {20, 2}, {21, 19}, {22, 25}, {23, 9},
        {24, 0},{25, 22}
    };

}
