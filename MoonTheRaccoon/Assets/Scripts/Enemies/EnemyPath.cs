﻿using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyPath : MonoBehaviour
{
    //** PUBLIC VARIABLES **//
    public List<GameObject> path;   // List of nodes for path
    public GameObject searchTarget; // list of directions to search
    public float distanceToNode = 0.5f;
    public float alertTime = 5f;
    public bool pathFollower = true;

    //** COMPONENT REFERENCES **//
    Animator animator;
    AIPath aiPath;
    AIDestinationSetter destination;

    //** MEMBER VARIABLES **//
    bool isFollowingPath = true;
    int index = 0;
    GameObject currentTarget;
    GameObject currentNode;
    Vector3 startPos;
    Vector2 lookDirection = new Vector2(0, -1);

    // Awareness variables
    List<Vector3> searchDirections;
    Vector3 initialDirection;
    bool isAware = false;
    bool checkedBack = false;
    bool searchLeft = false;
    float timeOnAlert = 0f;
    float initialMaxSpeed = 0f;
    float lerpFraction = 0f;
    Vector3 currentDirVector;
    int currentDirection;
    int backDirection;
    int leftDirection;
    int rightDirection;


    // Start is called before the first frame update
    void Start()
    {
        // pathfinding
        destination = gameObject.GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        currentNode = path[index];
        destination.target = currentNode.transform;
        startPos = gameObject.transform.position;
        initialMaxSpeed = aiPath.maxSpeed;


        // get the animator
        animator = GetComponent<Animator>();

        // initialize searchDirections
        searchDirections = new List<Vector3>();
        searchDirections.Add(Vector3.up);
        searchDirections.Add(Vector3.right);
        searchDirections.Add(Vector3.down);
        searchDirections.Add(Vector3.left);
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

        // if enemy is aware of player behind him
        if (isAware)
        {
            OnAlert();
        }
        else
        {
            // update search target
            searchTarget.transform.localPosition = new Vector3(lookDirection.x, lookDirection.y, 0);
        }

        // Update Animation
        UpdateAnimator();

    }

    // check if player entered the collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") && !collision.GetComponent<PlayerController>().Hidden())
        {
            // Get Initial Direction
            if (Math.Abs(lookDirection.x ) > Math.Abs(lookDirection.y))         // x direction
            {
                initialDirection.x = lookDirection.x;
            }
            else
            {
                initialDirection.y = lookDirection.y;
            }
            initialDirection = initialDirection.normalized;

            // set search direction to behind enemy
            float dirWeight = 0f; // weight of direction

            if (initialDirection.x > 0)
            {
                dirWeight = initialDirection.x;
                backDirection = 3;      // left
                leftDirection = 2;
                rightDirection = 0;
                currentDirection = 3;
            }
            else if(initialDirection.x < 0)
            {
                dirWeight = -initialDirection.x;
                backDirection = 1;       // right
                leftDirection = 0;
                rightDirection = 2;
                currentDirection = 1;
            }
            
            if (initialDirection.y < 0 && -initialDirection.y > dirWeight)
            {
                dirWeight = -initialDirection.y;
                backDirection = 0;       // up
                leftDirection = 1;
                rightDirection = 3;
                currentDirection = 0;
            }
            else if (initialDirection.y > 0 && initialDirection.y > dirWeight)
            {
                dirWeight = initialDirection.y;
                backDirection = 2;       // down
                leftDirection = 3;
                rightDirection = 1;
                currentDirection = 2;
            }

            currentDirVector = searchDirections[leftDirection];

            isAware = true;
            aiPath.maxSpeed = 0.01f;
        }
    }

    void OnAlert()
    {
        // update time
        timeOnAlert += Time.deltaTime;

        // lerp to current search target and update destination transform
        searchTarget.transform.localPosition = Vector3.Lerp(initialDirection.normalized, currentDirVector.normalized, lerpFraction).normalized;

        destination.target = searchTarget.transform;

        // increment lerp
        lerpFraction += 1f * Time.deltaTime;

        // reset search target if close
        if (!checkedBack && lerpFraction >= 1f)
        {
            // reset lerp
            lerpFraction = 0;

            // set initial direction to current direction and set current direction to back
            initialDirection = currentDirVector;
            currentDirection = backDirection;
            currentDirVector = searchDirections[currentDirection];

            // checked the back of the enemy
            checkedBack = true;
        }
        else if(lerpFraction >= 1f)
        {
            // reset lerp
            lerpFraction = 0;
            initialDirection = currentDirVector;

            // look left
            if (searchLeft)
            {
                currentDirVector = (searchDirections[backDirection] + searchDirections[leftDirection]).normalized;
                searchLeft = false;
            }
            // look right
            else
            {
                currentDirVector = (searchDirections[backDirection] + searchDirections[rightDirection]).normalized;
                searchLeft = true;
            }
        }

        // update look direction for animator
        lookDirection = searchTarget.transform.localPosition;

        // exit alert
        if (timeOnAlert > alertTime)
        {
            isAware = false;
            checkedBack = false;
            aiPath.maxSpeed = initialMaxSpeed;
            destination.target = currentTarget.transform;
            timeOnAlert = 0;
            lerpFraction = 0;
        }
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
