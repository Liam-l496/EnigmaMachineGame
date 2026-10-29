using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxWoodScript : MonoBehaviour
{
    Animator animator;
    int openBoxHash;
    int plugBoardLidOpenHash;

    [Header("Box Details")]
    [SerializeField] BoxCollider lidCollider;
    [SerializeField] BoxCollider plugBoardFlapCollider;



    // Start is called before the first frame update
    // void Start()
    // {
    //     animator = GetComponent<Animator>();
    //     openBoxHash = Animator.StringToHash("boxOpen");
    //     plugBoardLidOpenHash = Animator.StringToHash("plugBoardLidOpen");
    //     animator.speed = 0f;
    // }

    // public void UpdateLidState()
    // {
    //     // lidOpenState = !lidOpenState;
    //     animator.speed = 1f;
    //     animator.SetBool(openBoxHash, !animator.GetBool(openBoxHash));
    //     UpdateColliderState();
    // }

    // public void UpdatePlugBoardState()
    // {
    //     // plugBoardOpenState = !plugBoardOpenState;
    //     animator.SetBool(plugBoardLidOpenHash, !animator.GetBool(plugBoardLidOpenHash));
    //     UpdateColliderState();
    // }

    // public void UpdateColliderState() {
    //     if (animator.GetBool(openBoxHash))
    //     {
    //         plugBoardFlapCollider.enabled = true;
    //     }
    //     else
    //     {
    //         plugBoardFlapCollider.enabled = false;
    //     }
    //     if (animator.GetBool(plugBoardLidOpenHash))
    //     {
    //         lidCollider.enabled = false;
    //     }
    //     else
    //     {
    //         lidCollider.enabled = true;
    //     }
    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
