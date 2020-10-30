using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightRotation : MonoBehaviour
{
    public AIPath pathfinder;
    public SpriteRenderer flashlight;
    public GameObject searchTarget;

    // member variables
    bool searching = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(pathfinder != null && flashlight != null && !searching)
        {
            // calculate rotation
            Vector2 direction = new Vector2(searchTarget.transform.localPosition.x, searchTarget.transform.localPosition.y);
            direction = direction.normalized;
            if(pathfinder.desiredVelocity != Vector3.zero)
            {
                Vector3 newRotation = new Vector3(0, 0, (Mathf.Atan2(direction.x, direction.y) * -(180 / Mathf.PI)));
                transform.eulerAngles = newRotation;

                // change flashlight order in layer
                if (newRotation.z > -90 && newRotation.z < 90)
                {
                    flashlight.sortingOrder = 0;
                }
                else
                {
                    flashlight.sortingOrder = 2;
                }
            }
        }
    }
}
