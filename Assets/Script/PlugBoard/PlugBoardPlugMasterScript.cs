using System.Collections;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class PlugBoardPlugMasterScript : MonoBehaviour
{
    [Header("Plugs")]
    // plug 1 = initial left side
    // If plug1 character is goes in, do not reverse direction 
    public PlugBoardPlugScript plug1;
    private Transform plug1Transform;
    private char plug1Char;
    public PlugBoardPlugScript plug2;
    private Transform plug2Transform;
    private char plug2Char;

    public bool setPlugBoard = false;

    [Header("Rope")]
    public RopeElectricityWire rope; 

    // Start is called before the first frame update
    void Start()
    {
        plug1Transform = plug1.GetComponent<Transform>();
        plug2Transform = plug2.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (plug1.plugBoardChar != ' ' && plug2.plugBoardChar != ' ' && !setPlugBoard)
        {
            GameManagerScript.Instance.SetPlugBoard(plug1.plugBoardChar, plug2.plugBoardChar);
            GameManagerScript.Instance.SetPlugBoardObjectDictionary(plug1.plugBoardChar, plug2.plugBoardChar, this);
            setPlugBoard = true;
            plug1Char = plug1.plugBoardChar;
            plug2Char = plug2.plugBoardChar;
        }
        else if (setPlugBoard && (plug1.plugBoardChar == ' ' || plug2.plugBoardChar == ' '))
        {
            GameManagerScript.Instance.ResetPlugBoard(plug1Char, plug2Char);
            GameManagerScript.Instance.ResetPlugBoardObjectDictionary(plug1Char, plug2Char);
            setPlugBoard = false;
            plug1Char = plug1.plugBoardChar;
            plug2Char = plug2.plugBoardChar;
        }
    }

    public void StartElectricDirection(char character)
    {
        if (character == plug1Char)
        {
            rope.SetReverseDirection(true, true);
        }
        else if (character == plug2Char)
        {
            rope.SetReverseDirection(false, true);
        }
    }

    public RopeElectricityWire StartElectricity(int startingCharInt)
    {
        char startingChar = (char)('A' + startingCharInt);
        StartElectricDirection(startingChar);
        rope.StartElectricity();
        return rope;
    }

    public RopeElectricityWire StopElectricity()
    {
        rope.StopElectricity();
        return rope;
    }

    public bool ElectricityOn()
    {
        return rope.IsOn;
    }

    public bool ElectricityOff()
    {
        return rope.IsOff;
    }
}
