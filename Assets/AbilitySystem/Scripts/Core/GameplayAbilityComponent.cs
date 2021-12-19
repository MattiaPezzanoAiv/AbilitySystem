using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo add ability target
// todo add attributtes and cost?
public sealed class GameplayAbilityComponent : MonoBehaviour
{
    private void Error(string message)
    {
        Debug.LogError("GameplayAbilityComponent: " + message, this);
    }

    private readonly List<GameplayAbility> m_availableAbilities = new List<GameplayAbility>();
    private readonly List<GameplayAbility> m_runningAbilities = new List<GameplayAbility>();
    private readonly List<(GameplayAbility ability, bool wasCanceled)> m_abilitiesPendingEnd = new List<(GameplayAbility, bool)>();
    private readonly List<GameplayAbility> m_abilitiesPendingRemove = new List<GameplayAbility>();
    private readonly Dictionary<GameplayAbility, List<GameplayAbilityTask>> m_runningTasks = new Dictionary<GameplayAbility, List<GameplayAbilityTask>>();
    private readonly List<GameplayAbilityTask> m_tasksPendingKill = new List<GameplayAbilityTask>();

    public List<GameplayAbility> GetAbilities() => m_availableAbilities;
    public void GetActivatableAbilities(List<GameplayAbility> outAbilities)
    {
        foreach (var ability in m_availableAbilities)
        {
            if(!m_runningAbilities.Contains(ability))
            {
                outAbilities.Add(ability);
            }
        }
    }

    public T GrantAbility<T>() where T : GameplayAbility, new()
    {
        T ability = new T();
        GrantAbility(ability);
        return ability;
    }

    public GameplayAbility GrantAbility(GameplayAbilityType abilityType)
    {
        if(!abilityType.IsValid())
        {
            return null;
        }

        var ability = abilityType.InstantiateType();
        GrantAbility(ability);
        return ability;
    }

    public void GrantAbility(GameplayAbility ability)
    {
        if(ability != null)
        {
            ability.SetOwner(this);
            m_availableAbilities.Add(ability);
        }
    }

    public bool RemoveAbility<T>() where T : GameplayAbility
    {
        for (int i = 0; i < m_availableAbilities.Count; i++)
        {
            var ability = m_availableAbilities[i];
            if(ability is T)
            {
                return RemoveAbility(ability);
            }
        }

        return false;
    }

    public bool RemoveAbility(GameplayAbility ability)
    {
        if(ability is null)
        {
            return false;
        }

        m_abilitiesPendingRemove.Add(ability);
        
        if(m_runningAbilities.Contains(ability))
        {
            m_abilitiesPendingEnd.Add((ability, true)); // still running so it counts as a cancel
        }
        return true;
    }

    public bool TryActivateAbility(GameplayAbility ability)
    {
        if(ability is null)
        {
            Error("Ability is null!");
            return false;
        }

        if(!m_availableAbilities.Contains(ability))
        {
            // if this ability isn't mine we can't activate it.
            Error("Ability doesn't belong to this component.");
            return false;
        }

        if(m_runningAbilities.Contains(ability) || ability.IsInCooldown || !ability.CanStart())
        {
            // this is running already or cannot start
            return false;
        }

        m_runningAbilities.Add(ability);
        ability.OnAbilityStarted();
        ability.IsRunning = true;

        return true;
    }

    public bool EndAbility(GameplayAbility ability, bool wasCanceled = false)
    {
        if (ability is null)
        {
            Error("Ability is null!");
            return false;
        }

        if (!m_availableAbilities.Contains(ability))
        {
            // if this ability isn't mine we can't activate it.
            Error("Ability doesn't belong to this component.");
            return false;
        }

        if (m_runningAbilities.Contains(ability))
        {
            m_abilitiesPendingEnd.Add((ability, wasCanceled));
            return true;
        }

        return false;
    }

    private void EnsureTaskList(GameplayAbility ability)
    {
        if(!m_runningTasks.TryGetValue(ability, out var list))
        {
            m_runningTasks.Add(ability, new List<GameplayAbilityTask>());
        }
        else if(list == null)
        {
            list = new List<GameplayAbilityTask>();
        }
    }

    private void EnsureTasksRemoved(GameplayAbility ability)
    {
        if(ability == null)
        {
            return;
        }

        var list = m_runningTasks[ability];
        list.Clear();
    }

    public void StartTask(GameplayAbilityTask task)
    {
        EnsureTaskList(task.OwningAbility);
        m_runningTasks[task.OwningAbility].Add(task);
    }

    public void EndTask(GameplayAbilityTask task)
    {
        m_tasksPendingKill.Add(task);
    }

    private void Update()
    {
        // update all tasks
        foreach (var item in m_runningTasks)
        {
            foreach (var task in item.Value)
            {
                task.Tick();
            }
        }

        foreach (var task in m_tasksPendingKill)
        {
            m_runningTasks[task.OwningAbility].Remove(task);
        }
        m_tasksPendingKill.Clear();

        foreach (var item in m_abilitiesPendingEnd)
        {
            item.ability.OnAbilityEnded(item.wasCanceled);
            item.ability.IsRunning = false;
            EnsureTasksRemoved(item.ability);
            m_runningAbilities.Remove(item.ability);
        }
        m_abilitiesPendingEnd.Clear();

        foreach (var ability in m_abilitiesPendingRemove)
        {
            EnsureTasksRemoved(ability);
            m_availableAbilities.Remove(ability);
        }
        m_abilitiesPendingRemove.Clear();
    }
}
