using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class CS_SettingGround : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public TileBase tile;

    [HideInInspector] public Vector3 mousePos;

    void Start()
    {
        tilemap.size = new Vector3Int(23,21,0);
        Debug.Log(tilemap.size);

        //tilemap.SetTile(new Vector3Int(10, 9, 0), tile); // ������ ���� �� �����������
        //tilemap.SetTile(new Vector3Int(10, 8, 0), tile);
        //tilemap.SetTile(new Vector3Int(0, 0, 0), tile);
        //Debug.Log(tilemap.GetTile(new Vector3Int(10, 10, 0)));
    }

    void OnGUI()
    {
        // ������������� switch-case �������� ����������
        if (Input.anyKeyDown)
        {
            var pushed = Event.current;
            if (pushed.isKey)
            {
                switch (pushed.keyCode)
                {
                    case KeyCode.M:
                        // �������� ������� ���� �� ������
                        // ����������� ������ ���, ����� Y ������������
                        mousePos.x = Event.current.mousePosition.x;
                        mousePos.y = Camera.main.pixelHeight - Event.current.mousePosition.y;
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                        Vector3Int tilePos = tilemap.WorldToCell(worldPos);

                        tilemap.SetTile(tilePos, tile);
                        break;

                    default:
                        break;
                }
            }
            else if (pushed.isMouse)
            {

            }
        }
    }
}
