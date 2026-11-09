using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using System.Linq;

public class KeyInputManagerScript : MonoBehaviour
{

    public static KeyInputManagerScript Instance;

    [Header("Electric Animation Controller")]
    [SerializeField] private ElectricityAnimationController electricAnimationControllerPrefab;

    ElectricityAnimationController[] controllers = new ElectricityAnimationController[26];

    [Header("Dictionary to store electric wiring")]
    List<ElectricWireEnum> enumList = new List<ElectricWireEnum>();
    List<int> intList = new List<int>();

    [Header("Store state of keyboards")]
    public List<int> keyHeldDownList = new List<int>();
    public Dictionary<int, int> keyToBulbDictionary = new Dictionary<int, int>(); 
    public bool noKeyPressed = true;
    
    
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

    public void CreateController(int index)
    {
        if (controllers[index] != null) return;
        controllers[index] = Instantiate(electricAnimationControllerPrefab, Vector3.zero, Quaternion.identity);
    }

    public void CreateElectricPath(int index)
    {
        CleanUpController(index);
        CreateController(index);
        
        var (tempEnums, tempInts) = GameManagerScript.Instance.ReturnEncryptedPath(index);
        // print($"COUNT {tempEnums.Count} + {tempInts.Count}");
        
        // Create new list copies so the controller owns its own data
        controllers[index].wireAnimationOrderList = new List<ElectricWireEnum>(tempEnums);
        controllers[index].wireIndexList = new List<int>(tempInts);
        // PrintList(tempInts);
        keyToBulbDictionary[index] = tempInts[tempInts.Count - 2];
    }

    public void CleanUpController(int index)
    {
        if (controllers[index] == null) return;
        controllers[index].KillAllWires();
        controllers[index].wireIndex = 0;
        // Destroy(controllers[index].gameObject);
        // controllers[index] = null;  
    }

    public void KeyPress(string keyPressed)
    {
        if (PlayerPickUpScript.Instance.heldObject) return;
        if (noKeyPressed) {
            GameManagerScript.Instance.SpinRotor();
        }
        GameManagerScript.Instance.EXTERNALAnimateActuatorBarPressing();
        
        int charInt = keyPressed[0] - 'A';
        keyHeldDownList.Add(charInt);
        noKeyPressed = false;
        if (!GameManagerScript.Instance.powerOn) {
            return;
        }

        if(keyHeldDownList.Count == 1)
        {
            GameManagerScript.Instance.AddNewEncryptedLetter(keyPressed);
        }
        CreateElectricPath(charInt);
        // PrintDictionary(keyToBulbDictionary);
        // print($"FORWARD  {charInt}    {keyToBulbDictionary[charInt]}");
        GameManagerScript.Instance.LampOn(keyToBulbDictionary[charInt]);
        // PrintControllerArray();
    }

    public void KeyRelease(string keyReleased)
    {
        if (PlayerPickUpScript.Instance.heldObject) return;

        GameManagerScript.Instance.AnimateActuatorBarReleasing();

        int charInt = keyReleased[0] - 'A';

        // PrintControllerArray();

        keyHeldDownList.Remove(charInt);

        if (keyHeldDownList.Count == 0)
        {
            noKeyPressed = true;
        }

        if (!GameManagerScript.Instance.powerOn) return;

        GameManagerScript.Instance.LampOff(keyToBulbDictionary[charInt]);
        keyToBulbDictionary.Remove(charInt);
        if (controllers[charInt] != null) controllers[charInt].KeyRelease();
    }

    public void PrintControllerArray()
    {
        if (controllers == null)
        {
            Debug.Log("Array is null.");
            return;
        }

        string formattedOutput = string.Join("\n", controllers.Select((controller, index) => 
            $"[Index: {index}] -> {(controller != null ? controller.name : "null")}"
        ));

        Debug.Log($"--- Controller Array ({controllers.Length} items) ---\n{formattedOutput}");
    }

    public static void PrintList(List<int> list, string label = "List")
    {
        if (list == null)
        {
            Debug.Log($"{label} is null");
            return;
        }

        string output = $"LIST PRINT {label} ({list.Count}):\n" +
            string.Join("\n", list.Select((item, index) => $"[{index}] => {item}"));

        Debug.Log(output);
    }


    public static void PrintDictionary<TKey, TValue>(System.Collections.Generic.Dictionary<TKey, TValue> dictionary, string dictionaryName = "Dictionary")
    {
        string formattedOutput = $"KEYTOBULB {dictionaryName} ({dictionary.Count}):\n" +
            string.Join("\n", dictionary.Select(kvp => $"[{kvp.Key}] => {kvp.Value}"));

        Debug.Log(formattedOutput);
    }
}
