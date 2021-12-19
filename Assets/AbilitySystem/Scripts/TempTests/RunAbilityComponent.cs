using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAbilityComponent : MonoBehaviour
{
    public GameplayAbilityType Ability;
    public GameplayAbilityComponent AbilityComp;

    public AbilityExample Ability2;

    private void Start()
    {
        AbilityComp.GrantAbility(Ability2);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!AbilityComp.TryActivateAbility(Ability2))
            {
                Debug.Log("cannot start ability");
            }
        }
    }
}
