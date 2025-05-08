using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorController : MonoBehaviour
{
    public Tilemap tilemap;           // 대상 타일맵
    public Transform selectorObject;  // 따라다닐 셀렉터 오브젝트

    void Start()
    {
        Cursor.visible = false;
    }
    
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
        var cellPosition = new Vector3Int(cellPos.x, cellPos.y, 0);
        
        // 셀 좌표 → 셀의 중앙 월드 좌표
        Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);

        selectorObject.position = cellCenterWorld;
    }
}
