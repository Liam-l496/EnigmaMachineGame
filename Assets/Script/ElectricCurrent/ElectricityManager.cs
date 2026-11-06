
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class ElectricityManager : MonoBehaviour
{
    public static ElectricityManager Instance { get; private set; }

    // Only wires currently starting or stopping.
    private readonly List<SplineElectricityWire> transitioningWires =
        new List<SplineElectricityWire>();

    // Prevents duplicate registrations.
    private readonly HashSet<SplineElectricityWire> registeredWires =
        new HashSet<SplineElectricityWire>();

    public int TransitioningWireCount => transitioningWires.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "Multiple ElectricityManagers exist in the scene.");

            enabled = false;
            return;
        }

        Instance = this;
    }

    public void Register(SplineElectricityWire wire)
    {
        if (wire == null)
            return;

        if (registeredWires.Add(wire))
            transitioningWires.Add(wire);
    }

    public void Unregister(SplineElectricityWire wire)
    {
        if (wire == null)
            return;

        if (registeredWires.Remove(wire))
            transitioningWires.Remove(wire);
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        // Iterate backwards so completed wires can be removed safely.
        for (int i = transitioningWires.Count - 1; i >= 0; i--)
        {
            SplineElectricityWire wire = transitioningWires[i];

            if (wire == null)
            {
                transitioningWires.RemoveAt(i);
                continue;
            }

            // Returns false when the transition has finished.
            if (!wire.ManagerUpdate(delta))
            {
                registeredWires.Remove(wire);
                transitioningWires.RemoveAt(i);
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
