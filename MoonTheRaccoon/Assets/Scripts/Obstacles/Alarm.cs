using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    protected void TriggerAlarm()
    {
        if (!GameManager.Instance.alarmActive)
        {
            GameManager.Instance.ActivateAlarm(gameObject);
        }
    }

}
