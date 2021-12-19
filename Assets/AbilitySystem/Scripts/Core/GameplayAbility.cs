using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameplayAbility
{
    public GameplayAbilityComponent OwningComponent { get; private set; }

    public bool IsRunning { get; internal set; }
    
    /// <summary>
    /// Ownership cannot be transferrd. this can be called only 1 time from grant ability function
    /// </summary>
    internal void SetOwner(GameplayAbilityComponent OwningComponent)
    {
        UnityEngine.Assertions.Assert.IsNull(this.OwningComponent);
        UnityEngine.Assertions.Assert.IsNotNull(OwningComponent);

        this.OwningComponent = OwningComponent;
    }

    public virtual bool CanStart()
    {
        return true;
    }

    public virtual void OnAbilityStarted()
    {

    }

    public virtual void OnAbilityEnded(bool wasCanceled)
    {

    }

    public void EndAbility(bool wasCanceled = false)
    {
        OwningComponent.EndAbility(this, wasCanceled);
    }
}
