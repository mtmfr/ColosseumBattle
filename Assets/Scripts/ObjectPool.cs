using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool
{
    private static readonly Dictionary<Type, List<MonoBehaviour>> inactiveObjects = new();
    private static readonly Dictionary<Type, List<MonoBehaviour>> activeObjects = new();

    #region activate object
    public static T GetObject<T>(T toActivate, Vector3 position, Quaternion rotation) where T : MonoBehaviour
    {
        T objectToActivate = GetObjectToActivate(toActivate);

        objectToActivate.transform.SetPositionAndRotation(position, rotation);

        AddToActiveObjectValue(objectToActivate);

        return objectToActivate;
    }

    private static T GetObjectToActivate<T>(T objectToActivate) where T : MonoBehaviour
    {
        MonoBehaviour toActivate;
        Type type = objectToActivate.GetType();

        if (!inactiveObjects.TryGetValue(type, out List <MonoBehaviour> inactiveObjectList))
        {
            toActivate = GameObject.Instantiate(objectToActivate);
        }
        else
        {
            if (inactiveObjectList.Count == 0)
                toActivate = GameObject.Instantiate(objectToActivate);
            else
            {
                toActivate = inactiveObjectList[0];
            }
        }
            return (T)toActivate;
    }

    private static void AddToActiveObjectValue(MonoBehaviour activated)
    {
        Type type = activated.GetType();

        if (activeObjects.TryGetValue(type, out List<MonoBehaviour> activeObjectList))
        {
            activeObjectList.Add(activated);
        }
        else
        {
            List<MonoBehaviour> newActiveList = new() { activated };
            activeObjects.Add(type, newActiveList);
        }
    }
    #endregion


    public static void SetObjectInactive<T>(T toDeactivate) where T : MonoBehaviour
    {
        Type type = toDeactivate.GetType();

        toDeactivate.gameObject.SetActive(false);

        if (inactiveObjects.TryGetValue(type, out List<MonoBehaviour> activeObjectList))
        {
            activeObjectList.Add(toDeactivate);
        }
        else
        {
            List<MonoBehaviour> newInactiveList = new() { toDeactivate };
            inactiveObjects.Add(type, newInactiveList);
        }
    }
}