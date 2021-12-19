using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTaskTimer : GameplayAbilityTask
{
    public float Seconds;

    private float m_counter;

    public System.Action OnTimerEnded;
    public System.Action OnTick;

    public override void Tick()
    {
        base.Tick();

        OnTick?.Invoke();
        
        m_counter += Time.deltaTime;
        if(m_counter >= Seconds)
        {
            OnTimerEnded?.Invoke();
            EndTask();
        }
    }
}
