using System;
using UnityEngine;

public class HeatmapCell
{
    private int _value;
    public GameObject cellObject;
    public Vector3 worldPosition;

    public HeatmapCell()
    {
    }

    public HeatmapCell(Vector3 worldPosition, GameObject cellObject)
    {
        this.worldPosition = worldPosition;
        this.cellObject = cellObject;
    }

    public event Action<int, int> OnChanged;

    public int GetValue()
    {
        return _value;
    }

    public void AddValue(int add)
    {
        OnChanged?.Invoke(_value, add);
        _value += add;
    }

    internal void SetValue(int value)
    {
        OnChanged?.Invoke(_value, value);
        _value = value;
    }
}