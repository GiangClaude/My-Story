using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CusorManager : MonoBehaviour
{
    public Sprite cursorSprite;

    private void Start()
    {
        if (cursorSprite == null)
        {
            Debug.Log("Cursor: Gan sprite di!");
        } else
        {
            Texture2D cursorTexture = cursorSprite.texture;

            Vector2 hotspot = new Vector2(cursorSprite.pivot.x, cursorSprite.pivot.y);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
    }
}
