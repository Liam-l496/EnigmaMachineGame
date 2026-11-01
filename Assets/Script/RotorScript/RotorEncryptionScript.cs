using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.AI;
using System.IO.Pipes;
using System;

public class RotorEncryptionScript : MonoBehaviour
{
    [Header("Rotor Mechanisms")]
    [SerializeField] string wiring;
    public char notchChar;
    public int notch;

    public Dictionary<int, int> dictionaryEntry = new Dictionary<int, int>();
    public Dictionary<int, int> dictionaryExit = new Dictionary<int, int>();

    private int[] forwardMap = new int[26];
    private int[] reverseMap = new int[26];

    void Start()
    {
        PopulateWiring();
        notch = notchChar - 'A' + 1;
    }

    // public void PopulateDictionary()
    // {
    //     int normalLetter = 0;
    //     foreach (char c in wiring)
    //     {
    //         int exit = c - 'A';
    //         dictionaryEntry.Add(normalLetter, exit);
    //         dictionaryExit.Add(exit, normalLetter);
    //         normalLetter += 1;
    //     }
    // }

    // public int PlainToCipher(int plainValue, int rotorValue)
    // {
    //     int shift = rotorValue - 1;
        
    //     int pinIn = (plainValue + shift) % 26;
        
    //     int pinOut = dictionaryEntry[pinIn];
        
    //     int cipherValue = (pinOut - shift + 26) % 26;
        
    //     return cipherValue;
    // } 

    // public int CipherToPlain(int cipherValue, int rotorValue)
    // {
    //     int shift = rotorValue - 1;
        
    //     int pinIn = (cipherValue + shift) % 26;
        
    //     int pinOut = dictionaryExit[pinIn];
        
    //     int plainValue = (pinOut - shift + 26) % 26;
        
    //     return plainValue;
    // }

public void PopulateWiring()
    {
        wiring = wiring.ToUpper();
        for (int i = 0; i < 26; i++)
        {
            int mappedChar = wiring[i] - 'A';
            forwardMap[i] = mappedChar;
            reverseMap[mappedChar] = i;
        }
    }

    private int Mod26(int value)
    {
        return (value % 26 + 26) % 26;
    }

    /// <summary>
    /// Signal passing forward (Right to Left towards Reflector)
    /// </summary>
    public int PlainToCipher(int plainValue, int rotorValue)
    {
        int shift = rotorValue - 1;
        int pinIn = Mod26(plainValue + shift);
        int pinOut = forwardMap[pinIn];
        return Mod26(pinOut - shift);
    } 

    /// <summary>
    /// Signal passing backward (Left to Right from Reflector)
    /// </summary>
    public int CipherToPlain(int cipherValue, int rotorValue)
    {
        int shift = rotorValue - 1;
        int pinIn = Mod26(cipherValue + shift);
        int pinOut = reverseMap[pinIn];
        return Mod26(pinOut - shift);
    }



}