using System;
using UnityEngine;

public class Clock : MonoBehaviour {
    [SerializeField]
    Transform hours,minutes,seconds;

    const float hoursToDegrees = -30, minutesToDegrees = -6, secondsToDegrees = -6;

    private void Update() {
        var time = DateTime.Now.TimeOfDay;
        hours.localRotation = Quaternion.Euler(0, 0, hoursToDegrees * (float)time.TotalHours);
        minutes.localRotation = Quaternion.Euler(0, 0, minutesToDegrees * (float)time.TotalMinutes);
        seconds.localRotation = Quaternion.Euler(0, 0, secondsToDegrees * (float)time.TotalSeconds);
    }
}