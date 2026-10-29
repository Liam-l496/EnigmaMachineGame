using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BlackCoverFlapScript : MonoBehaviour
{

    [Header("Details")]
    [SerializeField] BoxCollider flapBC;
    [SerializeField] BoxCollider powerOnBC;
    [SerializeField] BoxCollider windowFlapBC;



    // Start is called before the first frame update
    // void Start()
    // {
    //     animator = GetComponent<Animator>();
    //     openFlapHash = Animator.StringToHash("isCoverLifting");
    //     powerOnHash = Animator.StringToHash("isPowerOn");
    //     windowFlapHash = Animator.StringToHash("isWindowLifting");
    // }

    // public void UpdateFlapInputState()
    // {
    //     animator.SetBool(openFlapHash, !animator.GetBool(openFlapHash));
    //     UpdateBCState();
    // }

    // public void UpdatePowerOnState()
    // {
    //     animator.SetBool(powerOnHash, !animator.GetBool(powerOnHash));
    //     UpdateBCState();
    // }

    // public void UpdateWindowFlapState()
    // {
    //     animator.SetBool(windowFlapHash, ! animator.GetBool(windowFlapHash));
    //     UpdateBCState();
    // }

    // void UpdateBCState()
    // {
    //     if (animator.GetBool(powerOnHash) || animator.GetBool(windowFlapHash))
    //     {
    //         flapBC.enabled = false;
    //     }
    //     else
    //     {
    //         flapBC.enabled = true;
    //     }
    //     if (animator.GetBool(openFlapHash))
    //     {
    //         powerOnBC.enabled = false;
    //         windowFlapBC.enabled = false;
    //     }
    //     else
    //     {
    //         powerOnBC.enabled = true;
    //         windowFlapBC.enabled = true;
    //     }
    // }

    // Update is called once per frame
    // void Update()
    // {
        // bool flapInput = Input.GetKey(openFlapKey);
        // bool powerOnInput = Input.GetKey(powerOnKey);
        // bool windowFlapInput = Input.GetKey(windowFlapKey);
    
        // if (flapInput && !flapPressed)
        // {
        //     animator.SetBool(openFlapHash, !animator.GetBool(openFlapHash));
        //     flapPressed = true;
        // }
        // if (powerOnInput && !powerOnPressed)
        // {
        //     animator.SetBool(powerOnHash, !animator.GetBool(powerOnHash));
        //     powerOnPressed = true;
        // }
        // if (windowFlapInput && !windowFlapPressed)
        // {
        //     animator.SetBool(windowFlapHash, ! animator.GetBool(windowFlapHash));
        //     windowFlapPressed = true;
        // }

        // flapPressed = flapInput;
        // powerOnPressed = powerOnInput;
        // windowFlapPressed = windowFlapInput;
        // animator.SetBool(openFlapHash, flapInput);
        // animator.SetBool(powerOnHash, powerOnInput);
        // animator.SetBool(windowFlapHash, windowFlapInput);
    // }
}
