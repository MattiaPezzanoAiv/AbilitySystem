using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using System.Linq;
#endif

[System.Serializable]
public struct GameplayAbilityType
{
    private readonly static Dictionary<string, System.Type> s_typesMap = new Dictionary<string, System.Type>();

    [ValueDropdown(nameof(GetTypeValues))]
    public string TypeName;

    public bool IsValid()
    {
        if(string.IsNullOrEmpty(TypeName))
        {
            return false;
        }

        var t = System.Type.GetType(TypeName);
        return t != null;
    }

    private static List<string> GetTypeValues()
    {
#if UNITY_EDITOR
        var result = new List<string>();
        var baseType = typeof(GameplayAbility);
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies)
        {
            result.AddRange(a.GetTypes().Where(o => o.IsSubclassOf(baseType)).Select(o => o.Name));
        }
        return result.Distinct().ToList();
#endif
    }

    public GameplayAbility InstantiateType()
    {
        if(TryGetFromCacheMap(TypeName, out var type))
        {
            return (GameplayAbility)System.Activator.CreateInstance(type);
        }

        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies)
        {
            type = a.GetType(TypeName);
            if(type != null)
            {
                AddToCacheMap(TypeName, type);
                return (GameplayAbility)System.Activator.CreateInstance(type);
            }
        }

        Debug.LogError("Couldn't instantiate a new type: " + TypeName);
        return null;
    }

    private static void AddToCacheMap(string typeName, System.Type type)
    {
        if(!s_typesMap.ContainsKey(typeName))
        {
            s_typesMap.Add(typeName, type);
        }
    }

    private static bool TryGetFromCacheMap(string typeName, out System.Type outType)
    {
        return s_typesMap.TryGetValue(typeName, out outType);
    }
}
