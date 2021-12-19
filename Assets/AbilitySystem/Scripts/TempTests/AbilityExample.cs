using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class TaskExample : GameplayAbilityTask
//{
//    public float Seconds;

//    private float m_counter;

//    public override void Tick()
//    {
//        m_counter += Time.deltaTime;
//        if(m_counter > Seconds)
//        {
//            OwningAbility.EndAbility();
//        }
//    }
//}

[System.Serializable]
public class AbilityExample : GameplayAbility
{
    public override void OnAbilityStarted()
    {
        base.OnAbilityStarted();

        var task = GameplayAbilityTask.NewTask<AbilityTaskTimer>(this);
        task.Seconds = 5;
        task.OnTimerEnded += () => EndAbility();
        task.ActivateTask();

        Debug.Log("started");
    }

    public override void OnAbilityEnded(bool wasCanceled)
    {
        base.OnAbilityEnded(wasCanceled);

        Debug.Log("ended");
    }
}
