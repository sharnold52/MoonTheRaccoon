using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour

{
    // Public variables
    public float alarmTime = 10.0f;
    [HideInInspector] public bool alarmActive = false;

    // Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The GameManager is null");
            }

            return _instance;
        }
    }


    private void Awake()
    {
        _instance = this;
    }

    // Activates alarm - all enemies chase after alarm source
    public void ActivateAlarm(GameObject newDestination)
    {
        alarmActive = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // loop through enemies and set new destination
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyPath>().ChaseNoisemaker(newDestination);
        }

        // deactivate alarm after alotted time
        StartCoroutine(DeactivateAlarm(enemies, alarmTime));
    }

    IEnumerator DeactivateAlarm(GameObject[] enemies, float timeToWait)
    {
        // Wait for specified time before deactivating the alarm
        yield return new WaitForSeconds(timeToWait);

        // Deactivate the alarm
        alarmActive = false;

        // loop through enemies and reset destination
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyPath>().ResetPath();
        }
    }
}
