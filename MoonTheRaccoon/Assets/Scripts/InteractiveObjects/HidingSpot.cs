using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : Interactable
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        // check for player interaction
        GetInput();

        if(interactInput > 0 && !interacting && interactionReady)
        {
            // Hide Player
            player.HidePlayer();

            // update bools
            interacting = true;
            interactionReady = false;
        }
        else if(interactInput > 0 && interacting && interactionReady)
        {
            // Reveal player
            player.RevealPlayer();

            // update bools
            interacting = false;
            interactionReady = false;
        }

        // reset interaction ready
        if(interactInput == 0)
        {
            Invoke("ReadyInteraction", interactionDelay);
        }
    }
}
