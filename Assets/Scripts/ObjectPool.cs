using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool
{
    private static readonly Dictionary<Type, List<MonoBehaviour>> inactiveObjects = new();
    private static readonly Dictionary<Type, List<MonoBehaviour>> activeObjects = new();

    #region activate object

    public static T GetObject<T>(T toActivate) where T : MonoBehaviour
    {
        T objectToActivate = GetObjectToActivate(toActivate, toActivate.transform.position, toActivate.transform.rotation);

        AddToActiveObjectValue(objectToActivate);
        objectToActivate.gameObject.SetActive(true);

        return objectToActivate;
    }

    public static T GetObject<T>(T toActivate, Vector3 position, Quaternion rotation) where T : MonoBehaviour
    {
        T objectToActivate = GetObjectToActivate(toActivate, position, rotation);

        AddToActiveObjectValue(objectToActivate);
        objectToActivate.gameObject.SetActive(true);

        return objectToActivate;
    }

    private static T GetObjectToActivate<T>(T objectToActivate, Vector3 positionAtActivation, Quaternion rotationAtActivation) where T : MonoBehaviour
    {
        MonoBehaviour toActivate;
        Type type = objectToActivate.GetType();

        if (!inactiveObjects.TryGetValue(type, out List <MonoBehaviour> inactiveObjectList))
        {
            toActivate = UnityEngine.Object.Instantiate(objectToActivate, positionAtActivation, rotationAtActivation);
        }
        else
        {
            if (inactiveObjectList.Count == 0)
                toActivate = UnityEngine.Object.Instantiate(objectToActivate, positionAtActivation, rotationAtActivation);
            else
            {
                toActivate = inactiveObjectList[0];
                toActivate.transform.SetPositionAndRotation(positionAtActivation, rotationAtActivation);
            }
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

    /// <summary>
    /// Deactivate an object and add it of the inactive object list
    /// </summary>
    /// <param name="toDeactivate">the object to deactivate</param>
    public static void SetObjectInactive<T>(T toDeactivate) where T : MonoBehaviour
    {
        Type type = toDeactivate.GetType();

        toDeactivate.gameObject.SetActive(false);

        if (inactiveObjects.ContainsKey(type))
            inactiveObjects[type].Add(toDeactivate);
        else inactiveObjects.Add(type, new List<MonoBehaviour>() { toDeactivate });

        activeObjects[type].Remove(toDeactivate);
    }

    /// <summary>
    /// Deactivate any active object of the specified type and add it of the inactive object list
    /// </summary>
    /// <param name="type">the type of an object to deactivate</param>
    public static void SetAnyObjectOfTypeInactive(Type type)
    {
        MonoBehaviour toDeactivate = activeObjects[type][0];
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