using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_HideEditorSprite : MonoBehaviour
{
    SpriteRenderer editorSprite;
    private void Awake()
    {
        editorSprite = GetComponent<SpriteRenderer>();
        editorSprite.enabled = false;
    }
}
