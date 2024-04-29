using UnityEngine;
using UnityEngine.Tilemaps;

public class CS_SettingGround : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public TileBase tile;

    [HideInInspector] public Vector3 mousePos;
    [HideInInspector] public Vector3Int tilePos;

    void Start()
    {
        tilemap.size = new Vector3Int(tilemap.size.x, tilemap.size.y, 0);
        Debug.Log(tilemap.size);
    }

    public void GetMousePosition()
    {
        // получает позицию мыши на экране
        // обязательно только так, иначе Y реверсирован
        mousePos.x = Event.current.mousePosition.x;
        mousePos.y = Camera.main.pixelHeight - Event.current.mousePosition.y;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        tilePos = tilemap.WorldToCell(worldPos);
    }
}
