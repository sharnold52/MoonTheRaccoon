using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyPath : MonoBehaviour
{
    // public Variables
    public List<GameObject> path;   // List of nodes for path
    public float distanceToNode = 0.5f;
    public bool pathFollower = true;

    // component References
    Animator animator;
    AIPath aiPath;


    // member variables
    bool isFollowingPath = true;
    int index = 0;
    GameObject currentTarget;
    GameObject currentNode;
    AIDestinationSetter destination;
    Vector3 startPos;
    Vector2 lookDirection = new Vector2(0, -1);
    

    // Start is called before the first frame update
    void Start()
    {
        currentNode = path[index];
        destination = gameObject.GetComponent<AIDestinationSetter>();
        destination.target = currentNode.transform;
        startPos = gameObject.transform.position;

        // get the animator
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pathFollower && currentNode != null && isFollowingPath)
        {
            CheckDistance();
        }
        // if current target doesn't exist, switch back to nodes
        else if(currentTarget == null)
        {
            ResetPath();
        }

        // Update Animation
        UpdateAnimator();
    }

    // check distance to node
    void CheckDistance()
    {
        // Calculate distance from currentNode to gameObject
        float distance = Vector3.Distance(gameObject.transform.TransformPoint(Vector3.zero), currentNode.transform.TransformPoint(Vector3.zero));

        // Check if distance is within distanceToNode
        if (distance < distanceToNode)
        {
            index = (index + 1) % path.Count;
            currentNode = path[index];
            destination.target = currentNode.transform;
            currentTarget = currentNode;
        }
    }

    void UpdateAnimator()
    {

        if (!Mathf.Approximately(aiPath.velocity.x, 0.0f) || !Mathf.Approximately(aiPath.velocity.y, 0.0f))
        {
            lookDirection.Set(aiPath.velocity.x, aiPath.velocity.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Speed", aiPath.velocity.magnitude);
        animator.SetFloat("LookX", lookDirection.x);
        animator.SetFloat("LookY", lookDirection.y);
    }


    // tell the enemy to chase noisemaker instead
    public void ChaseNoisemaker(GameObject newTarget)
    {
        isFollowingPath = false;
        destination.target = newTarget.transform;
        currentTarget = newTarget;
    }


    // Reset Path
    public void ResetPath()
    {
        currentTarget = currentNode;
        destination.target = currentTarget.transform;

        isFollowingPath = true;
    }
}
