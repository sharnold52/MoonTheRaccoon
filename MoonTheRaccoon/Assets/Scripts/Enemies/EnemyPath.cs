using Pathfinding;
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
    int currentDirection;
    int backDirection;
    int searchDirection1;
    int searchDirection2;


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
        if (collision.tag.Equals("Player"))
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
            if (initialDirection.x > 0)
            {
                backDirection = 3;       // left
                searchDirection1 = 2;
                searchDirection2 = 0;
                currentDirection = 2;
            }
            else if(initialDirection.x < 0)
            {
                backDirection = 1;       // right
                searchDirection1 = 0;
                searchDirection2 = 2;
                currentDirection = 0;
            }
            else if (initialDirection.y > 0)
            {
                backDirection = 0;       // up
                searchDirection1 = 3;
                searchDirection2 = 1;
                currentDirection = 3;
            }
            else if (initialDirection.y < 0)
            {
                backDirection = 2;       // down
                searchDirection1 = 1;
                searchDirection2 = 3;
                currentDirection = 1;
            }

            searchTarget.transform.localPosition = lookDirection;
            isAware = true;
            aiPath.maxSpeed = 0.01f;
        }
    }

    void OnAlert()
    {
        // update time
        timeOnAlert += Time.deltaTime;

        // lerp to current search target and update destination transform
        searchTarget.transform.localPosition = Vector3.Lerp(initialDirection * 0.5f, searchDirections[currentDirection] * 0.5f, lerpFraction);
        destination.target = searchTarget.transform;

        // increment lerp
        lerpFraction += 1f * Time.deltaTime;

        // reset search target if close
        if (!checkedBack && lerpFraction >= 1f)
        {
            // reset lerp
            lerpFraction = 0;

            // set initial direction to current direction and set current direction to back
            initialDirection = searchDirections[currentDirection];
            currentDirection = backDirection;

            // checked the back of the enemy
            checkedBack = true;
        }
        else if(lerpFraction >= 1f)
        {
            // reset lerp
            lerpFraction = 0;

            // set initial direction to current direction
            initialDirection = searchDirections[currentDirection];

            // set new current direction
            if (searchLeft)
            {
                if(currentDirection == backDirection)
                {
                    currentDirection = searchDirection1;
                }
                else if(currentDirection == searchDirection1)
                {
                    currentDirection = backDirection;
                    searchLeft = false;
                }
                else
                {
                    currentDirection = backDirection;
                }
            }
            else
            {
                if (currentDirection == backDirection)
                {
                    currentDirection = searchDirection2;
                }
                else if (currentDirection == searchDirection2)
                {
                    currentDirection = backDirection;
                    searchLeft = true;
                }
                else
                {
                    currentDirection = backDirection;
                }
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
