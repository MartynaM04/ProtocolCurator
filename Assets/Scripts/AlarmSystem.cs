using UnityEngine;
using System.Collections.Generic;

/// Controls alarm lights and audio when security is breached

public class AlarmSystem : MonoBehaviour
{
    [Header("References")]
    public List<Light> lightsToControl = new List<Light>();
    
    [Header("Audio Sources")]
    public AudioSource alarmLoopSource;
    public AudioSource alarmVoiceSource;

    [Header("Alarm Settings")]
    public Color alarmColor = Color.red;
    public float pulseSpeed = 4f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.5f;

    private bool isAlarmActive = false;

    public void StartAlarm()
    {
        isAlarmActive = true;

        if (alarmLoopSource != null)
        {
            alarmLoopSource.loop = true;
            if (!alarmLoopSource.isPlaying) alarmLoopSource.Play();
        }

        if (alarmVoiceSource != null)
        {
            alarmVoiceSource.loop = true;
            if (!alarmVoiceSource.isPlaying) alarmVoiceSource.Play();
        }

        foreach (Light light in lightsToControl)
        {
            if (light == null) continue;
            
            FlickeringLight flicker = light.GetComponent<FlickeringLight>();
            if (flicker != null) flicker.enabled = false;

            light.enabled = true;
            light.color = alarmColor;
        }
    }

    // NEW: Method to pause alarm sounds
    public void PauseAlarm()
    {
        if (alarmLoopSource != null && alarmLoopSource.isPlaying)
        {
            alarmLoopSource.Pause();
        }

        if (alarmVoiceSource != null && alarmVoiceSource.isPlaying)
        {
            alarmVoiceSource.Pause();
        }
    }

    // NEW: Method to resume alarm sounds
    public void ResumeAlarm()
    {
        if (isAlarmActive)
        {
            if (alarmLoopSource != null && !alarmLoopSource.isPlaying)
            {
                alarmLoopSource.UnPause();
            }

            if (alarmVoiceSource != null && !alarmVoiceSource.isPlaying)
            {
                alarmVoiceSource.UnPause();
            }
        }
    }

    // NEW: Check if alarm is active (for save/load)
    public bool IsAlarmActive()
    {
        return isAlarmActive;
    }

    void Update()
    {
        if (isAlarmActive)
        {
            float lerp = (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2;
            float currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, lerp);

            foreach (Light light in lightsToControl)
            {
                if (light != null) light.intensity = currentIntensity;
            }
        }
    }
}