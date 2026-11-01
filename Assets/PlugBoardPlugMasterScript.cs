using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugBoardPlugMasterScript : MonoBehaviour
{

    [Header("Plugs")]
    public PlugBoardPlugScript plug1;
    private char plug1Char;
    public PlugBoardPlugScript plug2;
    private char plug2Char;

    public bool setPlugBoard = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (plug1.plugBoardChar != ' ' && plug2.plugBoardChar != ' ' && !setPlugBoard)
        {
            GameManagerScript.Instance.SetPlugBoard(plug1.plugBoardChar, plug2.plugBoardChar);
            setPlugBoard = true;
            plug1Char = plug1.plugBoardChar;
            plug2Char = plug2.plugBoardChar;
        }
        else if (setPlugBoard && (plug1.plugBoardChar == ' ' || plug2.plugBoardChar == ' '))
        {
            GameManagerScript.Instance.ResetPlugBoard(plug1Char, plug2Char);
            setPlugBoard = false;
            plug1Char = plug1.plugBoardChar;
            plug2Char = plug2.plugBoardChar;
        }
    }
}
