using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLaser : Alarm
{
    // public variables
    public float laserOnDuration = 5f;
    public float laserOffDuration = 2.5f;

    // component references
    BoxCollider2D bCollider;
    SpriteRenderer sRender;

    // member variables
    bool isLaserOn = true;
    float timer = 0f;

    // grab component references
    private void Awake()
    {
        bCollider = gameObject.GetComponent<BoxCollider2D>();
        sRender = gameObject.GetComponent<SpriteRenderer>();
    }


    // update
    private void FixedUpdate()
    {
        // update timer
        timer += Time.deltaTime;

        // check if laser is on
        if (isLaserOn)
        {
            // wait for laserOnDuration
            if(timer > laserOnDuration)
            {
                // Disable laser
                isLaserOn = false;
                sRender.enabled = false;
                bCollider.enabled = false;

                // reset timer
                timer = 0f;
            }
        }
        else
        {
            // wait for LaserOffDuration
            if(timer > laserOffDuration)
            {
                // Enable laser
                isLaserOn = true;
                sRender.enabled = true;
                bCollider.enabled = true;

                // reset timer
                timer = 0f;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            TriggerAlarm();
        }
    }
}
