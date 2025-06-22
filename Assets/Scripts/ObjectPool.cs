using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ObjectPool
{
    private static readonly Dictionary<Type, List<MonoBehaviour>> inactiveObjects = new();
    private static readonly Dictionary<Type, List<MonoBehaviour>> activeObjects = new();

    #region activate object

    public static T GetObject<T>(T toActivate) where T : MonoBehaviour
    {
        T objectToActivate = GetObjectToActivate(toActivate);

        AddToActiveObjectValue(objectToActivate);
        objectToActivate.gameObject.SetActive(true);

        return objectToActivate;
    }

    public static T GetObject<T>(T toActivate, Vector3 position, Quaternion rotation) where T : MonoBehaviour
    {
        T objectToActivate = GetObjectToActivate(toActivate);

        objectToActivate.transform.SetPositionAndRotation(position, rotation);

        AddToActiveObjectValue(objectToActivate);
        objectToActivate.gameObject.SetActive(true);

        return objectToActivate;
    }

    private static T GetObjectToActivate<T>(T objectToActivate) where T : MonoBehaviour
    {
        MonoBehaviour toActivate;
        Type type = objectToActivate.GetType();

        if (!inactiveObjects.TryGetValue(type, out List <MonoBehaviour> inactiveObjectList))
        {
            toActivate = Object.Instantiate(objectToActivate);
        }
        else
        {
            if (inactiveObjectList.Count == 0)
                toActivate = Object.Instantiate(objectToActivate);
            else toActivate = inactiveObjectList[0];
        }
            return (T)toActivate;
    }

    public static void AddToActiveObjectValue(MonoBehaviour activated)
    {
        Type type = activated.GetType();

        if (activeObjects.ContainsKey(type))
            activeObjects[type].Add(activated);
        else activeObjects.Add(type, new List<MonoBehaviour>() { activated });

        if (inactiveObjects.ContainsKey(type))
            inactiveObjects[type].Remove(activated);
    }
    #endregion

    public static void SetObjectInactive<T>(T toDeactivate) where T : MonoBehaviour
    {
        Type type = toDeactivate.GetType();

        toDeactivate.gameObject.SetActive(false);

        if (inactiveObjects.ContainsKey(type))
            inactiveObjects[type].Add(toDeactivate);
        else inactiveObjects.Add(type, new List<MonoBehaviour>() { toDeactivate });

        activeObjects[type].Remove(toDeactivate);
    }

    public static void DiscardAllObject()
    {
        inactiveObjects.Clear();
        activeObjects.Clear();
    }
}