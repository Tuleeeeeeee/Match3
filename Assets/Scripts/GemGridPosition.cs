using System.Collections;
using System.Collections.Generic;
using Tuleeeeee.GridSystem;
using UnityEngine;

public class GemGridPosition
{
    private GemGrid gemGrid;
    private Grid<GemGridPosition> grid;
    private int x;
    private int y;

    public GemGridPosition(Grid<GemGridPosition> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void SetGemGrid(GemGrid gemGrid)
    {
        this.gemGrid = gemGrid;
        grid.TriggerGridObjectChanged(x, y);
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public Vector3 GetWorldPosition()
    {
        return grid.GetWorldPosition(x, y);
    }

    public GemGrid GetGemGrid()
    {
        return gemGrid;
    }

    public void ClearGemGrid()
    {
        gemGrid = null;
    }

    public void DestroyGem()
    {
        gemGrid?.Destroy();
        grid.TriggerGridObjectChanged(x, y);
    }

    public bool HasGemGrid()
    {
        return gemGrid != null;
    }

    public bool IsEmpty()
    {
        return gemGrid == null;
    }

    public override string ToString()
    {
        return gemGrid?.ToString();
    }
}


