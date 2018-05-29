using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static bool TryDestroy(this MonoBehaviour monoBehaviour, GameObject obj)
    {
        if (obj != null && ReferenceEquals(obj, null))
        {
            Object.Destroy(obj);
            return true;
        }
        return false;
    }
}