// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ActuatorBarScript : MonoBehaviour
// {
//     public enum AnimationState
//     {
//         OneRotate, OneRelease,
//         OneTwoRotate, OneTwoRelease,
//         OneTwoThreeRotate, OneTwoThreeRelease
//     }

//     Animator animator;
//     int oneRotate = Animator.StringToHash("OneRotate");
//     int oneTwoRotate = Animator.StringToHash("OneTwoRotate");
//     int oneTwoThreeRotate = Animator.StringToHash("OneTwoThreeRotate");
//     int oneRelease = Animator.StringToHash("OneRelease");
//     int oneTwoRelease = Animator.StringToHash("OneTwoRelease");
//     int oneTwoThreeRelease = Animator.StringToHash("OneTwoThreeRelease");


//     // Start is called before the first frame update
//     void Start()
//     {
//         animator = GetComponent<Animator>();
//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }
//     public void SetAllFalse()
//     {
//         animator.SetBool(oneRelease, false);
//         animator.SetBool(oneTwoRelease, false);
//         animator.SetBool(oneTwoThreeRelease, false);
//         animator.SetBool(oneRotate, false);
//         animator.SetBool(oneTwoRotate, false);
//         animator.SetBool(oneTwoThreeRotate, false);

//     }

//     public void AnimateActuatorBarPress(AnimationState endingState)
//     {
//         SetAllFalse();
//         if (endingState == AnimationState.OneRelease)
//         {
//             animator.SetBool(oneRelease, true);
//         }
//         if (endingState == AnimationState.OneTwoRelease)
//         {
//             animator.SetBool(oneTwoRelease, true);
//         }
//         if (endingState == AnimationState.OneTwoThreeRotate)
//         {
//             animator.SetBool(oneTwoThreeRelease, true);
//         }
//         animator.speed = 0f;

//     }
// }
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActuatorBarScript : MonoBehaviour
{
    public enum AnimationState
    {
        OneRotate, OneRelease,
        OneTwoRotate, OneTwoRelease,
        OneTwoThreeRotate, OneTwoThreeRelease
    }

    private Animator animator;

    // String hashes for Animator parameter lookup
    private readonly int oneRotate = Animator.StringToHash("OneRotate");
    private readonly int oneTwoRotate = Animator.StringToHash("OneTwoRotate");
    private readonly int oneTwoThreeRotate = Animator.StringToHash("OneTwoThreeRotate");
    private readonly int oneRelease = Animator.StringToHash("OneRelease");
    private readonly int oneTwoRelease = Animator.StringToHash("OneTwoRelease");
    private readonly int oneTwoThreeRelease = Animator.StringToHash("OneTwoThreeRelease");

    private Coroutine pauseCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAllFalse()
    {
        animator.SetBool(oneRelease, false);
        animator.SetBool(oneTwoRelease, false);
        animator.SetBool(oneTwoThreeRelease, false);
        animator.SetBool(oneRotate, false);
        animator.SetBool(oneTwoRotate, false);
        animator.SetBool(oneTwoThreeRotate, false);
    }

    // Going from Release to Rotate
public void AnimateActuatorBarRotate(AnimationState startingState)
    {
        SetAllFalse();

        animator.speed = 1f;

        string targetStateName = "";

        if (startingState == AnimationState.OneRotate)
        {
            animator.SetBool(oneRotate, true);
            targetStateName = "OneRotate";
        }
        else if (startingState == AnimationState.OneTwoRotate)
        {
            animator.SetBool(oneTwoRotate, true);
            targetStateName = "OneTwoRotate";
        }
        else if (startingState == AnimationState.OneTwoThreeRotate)
        {
            animator.SetBool(oneTwoThreeRotate, true);
            targetStateName = "OneTwoThreeRotate";
        }

        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
        }

        if (!string.IsNullOrEmpty(targetStateName))
        {
            pauseCoroutine = StartCoroutine(PauseAtEnd(targetStateName));
        }
    }


    // Going from Rotate to Release
    public void AnimateActuatorBarRelease(AnimationState endingState)
    {
        SetAllFalse();
        
        animator.speed = 1f;

        string targetStateName = "";

        if (endingState == AnimationState.OneRelease)
        {
            animator.SetBool(oneRelease, true);
            targetStateName = "OneRelease";
        }
        else if (endingState == AnimationState.OneTwoRelease)
        {
            animator.SetBool(oneTwoRelease, true);
            targetStateName = "OneTwoRelease";
        }
        else if (endingState == AnimationState.OneTwoThreeRelease)
        {
            animator.SetBool(oneTwoThreeRelease, true);
            targetStateName = "OneTwoThreeRelease";
        }

        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
        }

        if (!string.IsNullOrEmpty(targetStateName))
        {
            pauseCoroutine = StartCoroutine(PauseAtEnd(targetStateName));
        }
    }

    private IEnumerator PauseAtEnd(string stateName)
    {
        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        animator.speed = 0f;
        SetAllFalse();
    }

    public void ResetActuatorSpeed()
    {
        animator.speed = 1f;
    }
}