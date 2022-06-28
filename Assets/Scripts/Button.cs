using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    public UnityEvent buttonClick;
    private Animator animator;



    void Awake(){
        if (buttonClick == null){
            buttonClick = new UnityEvent();
        }
    }

    public bool OnMouseUp(){
        Invoke("attack", 0f);
        return true;
    }

    void attack(){
        animator = GetComponent<Animator>();
        animator.SetTrigger("playerChop");
    }
}
