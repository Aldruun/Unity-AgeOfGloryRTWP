using System;

public interface IGridObject
{
    event Action OnChanged; // x, z
    void SetGridValues(int x, int z);
}