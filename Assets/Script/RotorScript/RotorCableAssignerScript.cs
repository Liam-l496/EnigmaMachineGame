using UnityEngine;
using GogoGaga.OptimizedRopesAndCables;
using System.Collections.Generic;

public class RotorCableAssignerScript : MonoBehaviour
{
    [Header("Parent Containers")]
    public Transform startHoldersContainer;
    public Transform endHoldersContainer;
    public Transform ropesContainer;

    [Header("Cipher Configuration")]
    public string cipherKey = "EKMFLGDQVZNTOWYHXUSPAIBRCJ";

    [Header("Rope Animation")]
    public List<RopeElectricityWire> ropeElectricityWires = new List<RopeElectricityWire>();

    public RopeElectricityWire ropeTurningOff;

    private void Start()
    {
        AssignRopes();
        // print("CIPHER TEXT + " + cipherKey);
    }

    public void AssignRopes()
    {
        if (startHoldersContainer == null || endHoldersContainer == null || ropesContainer == null)
        {
            // print($"{cipherKey} {startHoldersContainer} {endHoldersContainer} {ropesContainer}");
            // Debug.LogError("[RopeAssigner] Assign all 3 parent containers in the Inspector!");
            return;
        }

        cipherKey = cipherKey.Trim().ToUpper();
        if (cipherKey.Length != 26)
        {
            // Debug.LogError($"[RopeAssigner] Cipher key must be 26 characters long. Current length: {cipherKey.Length}");
            return;
        }

        for (int i = 0; i < 26; i++)
        {
            int mappedIndex = cipherKey[i] - 'A'; // e.g. 'A' -> 0, 'E' -> 4

            // Fetch children in hierarchy order (0..25)
            Transform startChild = startHoldersContainer.GetChild(i);
            Transform endChild = endHoldersContainer.GetChild(mappedIndex);
            Transform ropeChild = ropesContainer.GetChild(i);

            // Fetch GogoGaga's Rope component
            Rope ropeComponent = ropeChild.GetComponent<Rope>();

            if (ropeComponent != null)
            {
                // Set points and force instant mesh recalculation
                // print($"ropeComponent = {ropeComponent},    startChild = {startChild},      endChild = {endChild},        {cipherKey}");
                ropeComponent.SetStartPoint(startChild, instantAssign: true);
                ropeComponent.SetEndPoint(endChild, instantAssign: true);
                ropeComponent.RecalculateRope();
            }
            else
            {
                // Debug.LogError($"[RopeAssigner] No 'Rope' component found on '{ropeChild.name}' at index {i}");
            }
        }

        // Debug.Log("<color=green>[RopeAssigner] Successfully mapped and assigned all 26 ropes!</color>");
    }

    public RopeElectricityWire LightUpRotor(int index, bool reverse = false)
    {
        GameObject ropeChild = ropesContainer.GetChild(index).gameObject;
        RopeElectricityWire rope = ropeChild.GetComponent<RopeElectricityWire>();
        rope.SetReverseDirection(reverse);
        rope.StartElectricity();
        ropeElectricityWires.Add(rope);
        return rope;
    }

    public bool RotorFullyOn()
    {
        RopeElectricityWire rope = ropeElectricityWires[ropeElectricityWires.Count - 1];
        return rope.IsOn;
    }

    public RopeElectricityWire TurnOffRotor()
    {
        if (ropeElectricityWires.Count <= 0) return null;
        ropeTurningOff = ropeElectricityWires[0];
        ropeTurningOff.StopElectricity();
        ropeElectricityWires.RemoveAt(0);
        return ropeTurningOff;
    }

    public bool RotorFullyOff()
    {
        if (ropeTurningOff == null) return true;
        return ropeTurningOff.IsOff;
    }
}