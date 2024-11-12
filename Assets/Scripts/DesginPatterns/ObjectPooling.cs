using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectPooling<TComponent, MatchType> : MonoBehaviour where TComponent : Component
{
    [Header("[ Setup for object pooling ]"), Space(6)]
    [SerializeField] protected List<TComponent> componentPrefabs;
    [SerializeField] protected List<TComponent> components;
    [SerializeField] protected Transform pool;

    public virtual TComponent Spawn(MatchType matchType) => SpawnFromPool(matchType);
    private TComponent SpawnFromPool(MatchType matchType = default)
    {
        TComponent componentPrefab = GetAvailableComponentPrefab(matchType);
        if (componentPrefab == null)
        {
            Debug.Log($"[ObjectPooling] SpawnFromPool | cannot find prefab with match : {matchType}");
            return null;
        }

        TComponent componentSpawn = GetPoolComponent(componentPrefab, matchType);
        ActivateComponent(componentSpawn);
        return componentSpawn;
    }

    private void ActivateComponent(TComponent componentSpawn)
    {
        componentSpawn.transform.SetParent(pool);
        componentSpawn.gameObject.SetActive(true);
    }

    public virtual TComponent ActiveComponentPrefab(TComponent component)
    {
        if (!componentPrefabs.Contains(component))
        {
            Debug.Log($"[ObjectPooling] SpawnFromPool | cannot find prefab : {component}");
            return null;
        }
        ActivateComponent(component);
        return component;
    }

    protected virtual TComponent GetAvailableComponentPrefab(MatchType matchType)
    {
        foreach (TComponent component in componentPrefabs)
            if (CheckMatchValue(matchType, component)) return component;
        return null;
    }

    protected virtual bool CheckMatchValue(MatchType matchType, TComponent component)
    {
        //TODO: This method should have a matching criteria. Currently always returns true.
        return true;
    }

    public virtual void Despawn(TComponent component)
    {
        component.gameObject.SetActive(false);
        components.Add(component);
    }

    protected virtual TComponent GetPoolComponent(TComponent componentPrefab, MatchType matchType)
    {
        foreach (TComponent component in components)
            if (CheckMatchValue(matchType, component) && !component.gameObject.activeSelf)
            {
                components.Remove(component);
                return component;
            }

        TComponent newComponent = CreateNewComponent(componentPrefab);
        return newComponent;
    }

    protected virtual TComponent GetPoolComponent(TComponent componentPrefab)
    {
        foreach (TComponent component in components)
            if (component == componentPrefab && !component.gameObject.activeSelf)
            {
                components.Remove(component);
                return component;
            }

        TComponent newComponent = CreateNewComponent(componentPrefab);
        return newComponent;
    }

    private TComponent CreateNewComponent(TComponent prefab)
    {
        TComponent newComponent = Instantiate(prefab);
        newComponent.name = prefab.name;
        return newComponent;
    }
}
