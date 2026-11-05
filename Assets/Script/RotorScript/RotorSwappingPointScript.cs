using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorSwappingPointScript : MonoBehaviour
{

    private GameManagerScript gameManagerScript;
    public int swappingIndex;

    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RotorSwappingPoint"))
        {
            Transform swappingPoint = other.GetComponent<Transform>();
            RotorSwappingScript swappingRotor = swappingPoint.parent.gameObject.GetComponent<RotorSwappingScript>();
            int swappingRotorIndex = swappingRotor.rotorMasterScript.rotorPosition;
            if (swappingRotorIndex == swappingIndex) return;
            RotorSwappingScript adjacentRotor = gameManagerScript.ReturnRotor(swappingIndex).GetComponent<RotorSwappingScript>();
            SwapPosition(swappingRotor, adjacentRotor);
        }
    }

    public void SwapPosition(RotorSwappingScript movingRotor, RotorSwappingScript stationaryRotor)
    {
        RotorMasterScript rotorMaster1 = movingRotor.rotorMasterScript;
        RotorMasterScript rotorMaster2 = stationaryRotor.rotorMasterScript;

        Transform tempPositionLifted = stationaryRotor.RotorPositionLifted;
        Transform tempSwappingTop = stationaryRotor.RotorPositionSwappingTop;
        Transform tempStationaryPos = stationaryRotor.RotorPositionStationary;
        int tempRotorPosition = rotorMaster2.rotorPosition;

        stationaryRotor.RotorPositionLifted = movingRotor.RotorPositionLifted;
        stationaryRotor.RotorPositionSwappingTop = movingRotor.RotorPositionSwappingTop;
        stationaryRotor.RotorPositionStationary = movingRotor.RotorPositionStationary;
        rotorMaster2.rotorPosition = rotorMaster1.rotorPosition;

        movingRotor.RotorPositionLifted = tempPositionLifted;
        movingRotor.RotorPositionSwappingTop = tempSwappingTop;
        movingRotor.RotorPositionStationary = tempStationaryPos;
        rotorMaster1.rotorPosition = tempRotorPosition;

        gameManagerScript.AssignRotorPlace(rotorMaster2);
        gameManagerScript.AssignRotorPlace(rotorMaster1);

        // stationaryRotor.MoveRotor(stationaryRotor.RotorPositionLifted);
        stationaryRotor.RotorStationaryPosition();
    }
}
