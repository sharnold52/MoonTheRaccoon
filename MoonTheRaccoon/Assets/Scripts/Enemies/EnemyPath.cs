using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    // public Variables
    public List<GameObject> path;   // List of nodes for path
    public float distanceToNode = 0.5f;


    // member variables
    public bool m_isFollowingPath = true;
    private int m_index = 0;
    private GameObject m_currentTarget;
    private GameObject m_currentNode;
    private AIDestinationSetter destination;

    // Start is called before the first frame update
    void Start()
    {
        if(path.Count >  1)
        {
            m_currentNode = path[m_index];
            destination = gameObject.GetComponent<AIDestinationSetter>();
            destination.target = m_currentNode.transform;
        }
        else
        {
            Debug.Log("Path List needs at least 2 game objects");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_currentNode != null && m_isFollowingPath)
        {
            // Calculate distance from m_currentNode to gameObject
            float distance = Vector3.Distance(gameObject.transform.TransformPoint(Vector3.zero), m_currentNode.transform.TransformPoint(Vector3.zero));

            // Check if distance is within distanceToNode
            if (distance < distanceToNode)
            {
                m_index = (m_index + 1) % path.Count;
                m_currentNode = path[m_index];
                destination.target = m_currentNode.transform;
                m_currentTarget = m_currentNode;
            }

        }
        // if current target doesn't exist, switch back to nodes
        else if(m_currentTarget == null)
        {
            m_currentTarget = m_currentNode;
            destination.target = m_currentTarget.transform;

            m_isFollowingPath = true;
        }
    }

    // tell the enemy to chase noisemaker instead
    public void ChaseNoisemaker(GameObject newTarget)
    {
        m_isFollowingPath = false;
        destination.target = newTarget.transform;
        m_currentTarget = newTarget;
    }
}
