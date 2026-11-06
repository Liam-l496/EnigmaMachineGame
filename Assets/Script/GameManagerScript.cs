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
    private RotorMasterScript rotorSelected1;
    private RotorMasterScript rotorSelected2; 
    // private bool swapping = false;

    [Header("Rotors")]
    [SerializeField] RotorMasterScript rotor1;
    [SerializeField] RotorMasterScript rotor2;
    [SerializeField] RotorMasterScript rotor3;
    [SerializeField] int notchRequired = 10;
    bool spinRotor2 = false;
    bool spinRotor3 = false;

    [Header("Key Objects")]
    public KeyScript[] allKeys; 
    private int currentEncryptedText;


    public bool powerOn = false;

    [Header("Electric Cables")]

    [SerializeField] ElectricityAnimationController electricityAnimationController;

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
        RotorScript rotor1Script = rotor1.rotorScript;
        RotorScript rotor2Script = rotor2.rotorScript;
        RotorScript rotor3Script = rotor3.rotorScript;


        spinRotor2 = false;
        spinRotor3 = false;
        if (rotor1.rotorEncryptionScript.notch == (rotor1Script.rotorValue))
        {
            spinRotor2 = true;
        }
        if (rotor2.rotorEncryptionScript.notch == (rotor2Script.rotorValue))
        {
            spinRotor2 = true;
            spinRotor3 = true;
        }
        rotor1Script.SpinRotorOnce();
        if(spinRotor2) rotor2Script.SpinRotorOnce();
        if(spinRotor3) rotor3Script.SpinRotorOnce();

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
        if (rotor2.rotorScript.rotorValue == rotor2.rotorEncryptionScript.notch)
        {
            return ActuatorBarScript.AnimationState.OneTwoThreeRelease;
        }
        else if (rotor1.rotorScript.rotorValue == rotor1.rotorEncryptionScript.notch)
        {
            return ActuatorBarScript.AnimationState.OneTwoRelease;
        }
        return ActuatorBarScript.AnimationState.OneRelease;
        
    }

    public void EXTERNALAnimateActuatorBarPressing()
    {
        AnimateActuatorBarPressing(spinRotor2, spinRotor3);
    }



    public void AnimateActuatorBarPressing(bool spinRotor2, bool spinRotor3)
    {
        ActuatorBarScript.AnimationState state = ActuatorBarRotateState(spinRotor2, spinRotor3);
        actuatorBar.AnimateActuatorBarRotate(state);
    }

    public void AnimateActuatorBarReleasing()
    {
        ActuatorBarScript.AnimationState state = ActuatorBarReleaseState();
        actuatorBar.AnimateActuatorBarRelease(state);
        
    }

    public void KeyPress(string keyPressed)
    {
        // rotor1.SpinRotorOnce();
        // if (powerOn) {
        //     sentenceTyped += keyPressed;
        // }
        // SpinRotor();
        // AnimateActuatorBarPressing(spinRotor2, spinRotor3);
        // ActuatorBarScript.AnimationState state = ActuatorBarRotateState(spinRotor2, spinRotor3);
        // print(state);
        // actuatorBar.AnimateActuatorBarRotate(state);
        // electricityAnimationController.AddToList(ElectricWireEnum.KeyToPlug, keyPressed[0] - 'A');
        currentEncryptedText = FindEncryptedText(keyPressed);
        sentenceTyped += (char)(currentEncryptedText + 'A');

        // print($"TURNED LIGHT {currentEncryptedText} ON {allKeys[currentEncryptedText]}");
        // allKeys[currentEncryptedText].LampOn();
        // print("Sentence Typed: " + sentenceTyped);


    
        // electricityAnimationController.AddToList(ElectricWireEnum.PlugToKey, currentEncryptedText);
        // electricityAnimationController.AddToList(ElectricWireEnum.KeyToBulb, currentEncryptedText);    
        // electricityAnimationController.AddToList(ElectricWireEnum.Bulb, currentEncryptedText);
    }

    public void KeyRelease()
    {
        // AnimateActuatorBarReleasing();
        // ActuatorBarScript.AnimationState state = ActuatorBarReleaseState();
        // print(state);
        // actuatorBar.AnimateActuatorBarRelease(state);
        // print($"TURNED LIGHT {currentEncryptedText} OFF {allKeys[currentEncryptedText]}");
        // allKeys[currentEncryptedText].LampOff();
        // electricityAnimationController.KeyRelease();        
    }

    public void LampOn(int lampIndex)
    {
        allKeys[lampIndex].LampOn();
    }

    public void LampOff(int lampIndex)
    {
        allKeys[lampIndex].LampOff();
    }

    public void ClearRotorSelected()
    {
        // if (rotorSelected1 != null) rotorSelected1.selectedForSwapping = false;
        // if (rotorSelected2 != null) rotorSelected2.selectedForSwapping = false;

        // rotorSelected1 = null;
        // rotorSelected2 = null;
    }

    public void AssignRotorPlace(RotorMasterScript rotor) {
        if (rotor.rotorPosition == 1)
        {
            rotor1 = rotor;
        }
        else if(rotor.rotorPosition == 2)
        {
            rotor2 = rotor;
        }
        else if (rotor.rotorPosition == 3)
        {
            rotor3 = rotor;
        }
    }

    public RotorMasterScript ReturnRotor(int rotorPosition)
    {
        if (rotorPosition == 1)
        {
            return rotor1;
        }
        else if (rotorPosition == 2)
        {
            return rotor2;
        }
        else if (rotorPosition == 3)
        {
            return rotor3;
        }
        return null;
    }

    public void SetPlugBoard(char plug1, char plug2)
    {
        int plug1Int = plug1 - 'A';
        int plug2Int = plug2 - 'A';
        PlugBoard[plug1Int] = plug2Int;
        PlugBoard[plug2Int] = plug1Int;
    }

    public int GetPlugBoardCipher(char character)
    {
        int cipher = character - 'A';
        return PlugBoard[cipher];
    }

    public int GetPlugBoardCipher(int character)
    {
        return PlugBoard[character];
    }

    public void ResetPlugBoard(char plug1, char plug2)
    {
        int plug1Int = plug1 - 'A';
        int plug2Int = plug2 - 'A';
        PlugBoard[plug1Int] = plug1Int;
        PlugBoard[plug2Int] = plug2Int;
    }

    public void AddNewEncryptedLetter(string keyPressed)
    {
        int currentEncryptedText = FindEncryptedText(keyPressed);
        sentenceTyped += (char)(currentEncryptedText + 'A');
    }

    public int FindEncryptedText(string keyPressed)
    {
        int plainChar = keyPressed[0] - 'A';

        int plugBoard1 = GetPlugBoardCipher(plainChar);

        int pass1 = rotor1.GetPlainToCipher(plugBoard1);
        int pass2 = rotor2.GetPlainToCipher(pass1);
        int pass3 = rotor3.GetPlainToCipher(pass2);
        int pass4 = Reflector[pass3];
        int pass5 = rotor3.GetCipherToPlain(pass4);
        int pass6 = rotor2.GetCipherToPlain(pass5);
        int pass7 = rotor1.GetCipherToPlain(pass6);

        int plugBoard2 = GetPlugBoardCipher(pass7);

        
        // print("Key Pressed: " + (char)(plainChar + 'A') + " (" + plainChar + ")");

        // print("Pass 1 (Rotor 1 Forward): " + (char)(pass1 + 'A') + " (" + pass1 + ") Current Sentence = " + sentenceTyped);

        // print("Pass 2 (Rotor 2 Forward): " + (char)(pass2 + 'A') + " (" + pass2 + ") Current Sentence = " + sentenceTyped);

        // print("Pass 3 (Rotor 3 Forward): " + (char)(pass3 + 'A') + " (" + pass3 + ") Current Sentence = " + sentenceTyped);

        // print("Pass 4 (Reflector):       " + (char)(pass4 + 'A') + " (" + pass4 + ") Current Sentence = " + sentenceTyped);

        // print("Pass 5 (Rotor 3 Reverse): " + (char)(pass5 + 'A') + " (" + pass5 + ") Current Sentence = " + sentenceTyped);

        // print("Pass 6 (Rotor 2 Reverse): " + (char)(pass6 + 'A') + " (" + pass6 + ") Current Sentence = " + sentenceTyped);

        // print("Pass 7 (Rotor 1 Reverse): " + (char)(pass7 + 'A') + " (" + pass7 + ") Current Sentence = " + sentenceTyped);

        // print("PLAIN TEXT: " + (char)(plainChar + 'A') + "    CIPHER TEXT:    " + (char)(pass7 + 'A') + " Current Sentence = " + sentenceTyped);
        
        return plugBoard2;
    }


     public (List<ElectricWireEnum> _enumList, List<int> _intList) ReturnEncryptedPath(int plainChar)
    {

        List<ElectricWireEnum> enumList = new List<ElectricWireEnum>();
        List<int> intList = new List<int>();

        // electricityController.PowerOnWirePlugBoardIn(plainChar);
        // plugBoardWire1 = plainChar;

        enumList.Add(ElectricWireEnum.KeyToPlug);
        intList.Add(plainChar);

        enumList.Add(ElectricWireEnum.PlugIn);
        intList.Add(plainChar);

        int plugBoard1 = GetPlugBoardCipher(plainChar);
        if (plugBoard1 != plainChar) {} // DO the rope animation
        
        enumList.Add(ElectricWireEnum.PlugOut);
        intList.Add(plugBoard1);

        enumList.Add(ElectricWireEnum.InputWheel);
        intList.Add(plugBoard1);

        int pass1 = rotor1.GetPlainToCipher(plugBoard1);
        enumList.Add(ElectricWireEnum.Rotor1);
        intList.Add(pass1);

        int pass2 = rotor2.GetPlainToCipher(pass1);
        enumList.Add(ElectricWireEnum.Rotor2);
        intList.Add(pass2);

        int pass3 = rotor3.GetPlainToCipher(pass2);
        enumList.Add(ElectricWireEnum.Rotor3);
        intList.Add(pass3);

        int pass4 = Reflector[pass3];
        enumList.Add(ElectricWireEnum.Reflector);
        intList.Add(pass4);

        int pass5 = rotor3.GetCipherToPlain(pass4);
        enumList.Add(ElectricWireEnum.Rotor3Reverse);
        intList.Add(pass5);

        int pass6 = rotor2.GetCipherToPlain(pass5);
        enumList.Add(ElectricWireEnum.Rotor2Reverse);
        intList.Add(pass6);

        int pass7 = rotor1.GetCipherToPlain(pass6);
        enumList.Add(ElectricWireEnum.Rotor1Reverse);
        intList.Add(pass7);

        enumList.Add(ElectricWireEnum.InputWheelReverse);
        intList.Add(pass7);

        enumList.Add(ElectricWireEnum.PlugInReverse);
        intList.Add(pass7);

        int plugBoard2 = GetPlugBoardCipher(pass7);
        if (plugBoard2 != pass7) {} // Add the rope animation
        enumList.Add(ElectricWireEnum.PlugOutReverse);
        intList.Add(plugBoard2);

        enumList.Add(ElectricWireEnum.PlugToKey);
        intList.Add(plugBoard2);

        enumList.Add(ElectricWireEnum.KeyToBulb);
        intList.Add(plugBoard2);

        enumList.Add(ElectricWireEnum.Bulb);
        intList.Add(plugBoard2);
    
        return (enumList, intList);
    }


    public Dictionary<int, int> Reflector = new Dictionary<int, int>()
    {
    { 0, 24 }, { 24, 0 },  // A <-> Y
        { 1, 17 }, { 17, 1 },  // B <-> R
        { 2, 20 }, { 20, 2 },  // C <-> U
        { 3, 7  }, { 7,  3 },  // D <-> H
        { 4, 16 }, { 16, 4 },  // E <-> Q
        { 5, 18 }, { 18, 5 },  // F <-> S
        { 6, 11 }, { 11, 6 },  // G <-> L
        { 8, 15 }, { 15, 8 },  // I <-> P
        { 9, 23 }, { 23, 9 },  // J <-> X
        { 10, 13}, { 13, 10},  // K <-> N
        { 12, 14}, { 14, 12},  // M <-> O
        { 19, 25}, { 25, 19},  // T <-> Z
        { 21, 22}, { 22, 21}   // V <-> W
    
    
    };

    public Dictionary<int, int> PlugBoard = new Dictionary<int, int>()
{
    { 0, 0 },   // A <-> A
    { 1, 1 },   // B <-> B
    { 2, 2 },   // C <-> C
    { 3, 3 },   // D <-> D
    { 4, 4 },   // E <-> E
    { 5, 5 },   // F <-> F
    { 6, 6 },   // G <-> G
    { 7, 7 },   // H <-> H
    { 8, 8 },   // I <-> I
    { 9, 9 },   // J <-> J
    { 10, 10 }, // K <-> K
    { 11, 11 }, // L <-> L
    { 12, 12 }, // M <-> M
    { 13, 13 }, // N <-> N
    { 14, 14 }, // O <-> O
    { 15, 15 }, // P <-> P
    { 16, 16 }, // Q <-> Q
    { 17, 17 }, // R <-> R
    { 18, 18 }, // S <-> S
    { 19, 19 }, // T <-> T
    { 20, 20 }, // U <-> U
    { 21, 21 }, // V <-> V
    { 22, 22 }, // W <-> W
    { 23, 23 }, // X <-> X
    { 24, 24 }, // Y <-> Y
    { 25, 25 }  // Z <-> Z
};

}