using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGrid
{
    public event EventHandler OnDestroyed;

    private GemSO gem;
    private int x;
    private int y;
    private bool isDestroyed;

    public GemGrid(GemSO gem, int x, int y)
    {
        this.gem = gem;
        this.x = x;
        this.y = y;

        isDestroyed = false;
    }

    public GemSO GetGem()
    {
        return gem;
    }

    public Vector3 GetWorldPosition()
    {
        return new Vector3(x, y);
    }

    public void SetGemXY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Destroy()
    {
        isDestroyed = true;
        OnDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public override string ToString()
    {
        return isDestroyed.ToString();
    }
}
