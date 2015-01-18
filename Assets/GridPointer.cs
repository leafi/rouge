using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridPointer : MonoBehaviour
{
    private IntVector2 lastCell;
    private MeshRenderer meshRenderer;

    private static GridPointer gp;
    public static GridPointer Get() { return gp; }

	void Start()
    {
        lastCell = Grid.Get().GetCellFromMousePicker();
        meshRenderer = GetComponent<MeshRenderer>();
        gp = this;
	}

    public void Hilight(IntVector2 gv) { Hilight(gv.x, gv.z); }
    public void Hilight(int gx, int gz)
    {
        lastCell = new IntVector2(gx, gz);
        meshRenderer.enabled = !Grid.Get().IsBlocked(gx, gz);
        transform.position = new Vector3(gx, transform.position.y, gz);
    }
	
	void Update()
    {
        var cell = Grid.Get().GetCellFromMousePicker();

        if (cell.x != lastCell.x || cell.z != lastCell.z)
            Hilight(cell);
	}
}
