using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameplayAbilityTask
{
    public virtual void Tick() { }

    public static T NewTask<T>(GameplayAbility Owner) where T : GameplayAbilityTask, new()
    {
        UnityEngine.Assertions.Assert.IsNotNull(Owner);
        T task = new T();
        task.OwningAbility = Owner;
        task.OwningComponent = Owner.OwningComponent;
        return task;
    }

    public GameplayAbility OwningAbility { get; private set; }
    public GameplayAbilityComponent OwningComponent{ get; private set; }

    public void ActivateTask()
    {
        OwningComponent.StartTask(this);
    }

    public void EndTask()
    {
        OwningComponent.EndTask(this);
    }
}
