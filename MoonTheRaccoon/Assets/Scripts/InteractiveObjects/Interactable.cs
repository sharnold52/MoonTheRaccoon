using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // public variables
    public float interactionDelay = 1f;

    // inherited variables
    protected float interactInput = 0f;
    protected bool interacting = false;
    protected bool interactionReady = true;
    protected PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        // get reference to player
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    protected void GetInput()
    {
        // check for player interaction
        interactInput = Input.GetAxis("Interact");
    }

   protected void ReadyInteraction()
    {
        interactionReady = true;
    }
}
