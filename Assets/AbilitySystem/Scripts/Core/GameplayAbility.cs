using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class GameplayAbility
{
    public GameplayAbilityComponent OwningComponent { get; private set; }

    public bool IsRunning { get; internal set; }

    public bool IsInCooldown => m_cooldownCommitTime > 0 && Time.time < m_cooldownCommitTime + Cooldown;

    private float m_cooldownCommitTime;

    // parameters
    [Min(0f), SerializeField]
    private float Cooldown;
    // end parameters

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
        CommitCooldown();
    }

    public void EndAbility(bool wasCanceled = false)
    {
        OwningComponent.EndAbility(this, wasCanceled);
    }

    public void CommitCooldown()
    {
        m_cooldownCommitTime = Time.time;
    }
}
