/*using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnapManager : MonoBehaviour
{
    public static SnapManager Instance { get; private set; }

    private List<ISnappable> snappedObjects = new List<ISnappable>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterSnap(ISnappable obj)
    {
        snappedObjects.Add(obj);
        Debug.Log($"Snap Registered: {obj.ObjectName}");
    }

    public int GetTotalSnappedCount()
    {
        return snappedObjects.Count;
    }

    public int GetSnappedCountByName(string name)
    {
        return snappedObjects.Count(obj => obj.ObjectName == name);
    }

    public void PrintSnapSummary()
    {
        Debug.Log($"Total Snapped Objects: {GetTotalSnappedCount()}");

        var groupedCounts = snappedObjects.GroupBy(obj => obj.ObjectName)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var entry in groupedCounts)
        {
            Debug.Log($"Snapped '{entry.Key}': {entry.Value} times");
        }
    }
}*/