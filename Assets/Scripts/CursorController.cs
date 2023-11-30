using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Sprite cursorSprite;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorSprite.texture, hotSpot, cursorMode);
    }
}
