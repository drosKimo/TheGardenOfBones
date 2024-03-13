using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Menu
{
    public GameObject MenuPrefab;
}

public class CS_HotKeys : MonoBehaviour
{
    // ������������ ������:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    [SerializeField] public GameObject plant;
    [HideInInspector] public Object obj;

    CS_PlayerController controller;
    CS_SettingGround settingGround;
    CS_SpiritController spirit;
    public Menu menu = new Menu();

    int useCode = 0, // ��� ��� ��������������
        counterGround = 0, // ������� ������ �����
        counterSpirits = 0; // ������� �������� ���������
    bool stopped = false, moving = false;
    string collName;

    Component[] components;
    Animator spiritAnim, plantAnim;

    private void Awake()
    {
        controller = GetComponent<CS_PlayerController>(); // ��� ��������������

        GameObject ground = GameObject.Find("GroundTest");
        settingGround = ground.GetComponent<CS_SettingGround>(); // ��� ������� �����
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
                    case KeyCode.E: // �������������                      
                        switch (useCode)
                        {
                            // 1 = ������� ��������
                            // 2 = ������� (� �����); �� ����
                            // 3 = �������� ����� (+ �������)
                            // 4 = ��������� �� �����

                            case 1:
                                if (counterGround > counterSpirits) // ���������, ���� �� �� ����� ��������� ����� ��� �������
                                {
                                    counterSpirits++;
                                    useCode = 2;
                                    obj = GameObject.Find(collName); // ����� ������� �� ����� 
                                    spiritAnim = obj.GetComponent<Animator>();
                                    spirit = obj.GetComponent<CS_SpiritController>();

                                    spiritAnim.SetTrigger("Arised"); // ������� �������� �������
                                    spiritAnim.SetInteger("SetSpirit", 1); // ���������� ������ � CS_SpiritController.cs

                                    StartCoroutine(player_use()); // �������� � ���������� �������� �������������
                                }
                                else
                                {
                                    Debug.Log("������ ��������!");
                                }
                                break;
                            case 4:
                                plantAnim.SetInteger("StateCounter", plantAnim.GetInteger("StateCounter") + 1);
                                if (plantAnim.GetInteger("StateCounter") == 3)
                                {
                                    counterSpirits--;
                                    Destroy(plantAnim.gameObject, 1.5f);
                                }
                                break;
                        }
                        break;

                    case KeyCode.Tab: // �����, ���������� ��������
                        components = FindObjectsOfType<Animator>(); // �������� ��� ���������� ���� Animator �� �����

                        if (stopped)
                        {
                            menu.MenuPrefab.gameObject.SetActive(false);
                            PlayAnimations();
                        }
                        else
                        {
                            menu.MenuPrefab.gameObject.SetActive(true);
                            StopAnimations();
                        }
                        break;

                    case KeyCode.Escape:
                        Application.Quit();
                        break;
                    
                    case KeyCode.Q: // ��� ������, ����� ����� ���������� ������
                        Debug.Log(gameObject.transform.position);
                        break;

                    case KeyCode.F: // ������������ �����
                        SceneManager.LoadScene("Test Scene");
                        break;

                    default:
                        break;
                }
            }
            else if (pushed.isMouse)
            {
                settingGround.GetMousePosition(); // ������ ���, ����� ������ ����, �������� � ���������� �� ��������� �������� (������ - ����� ��� �������)

                //����������� ����� �������� �������� ������� ���� ��� �������� �� ������� ��������, ����� �������� �����������
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                switch (pushed.button)
                {
                    case 0:
                        // ������ settingGround.tilemap.GetTile(settingGround.tilePos) != null ���������, ���� �� ��� ����� ���� �����
                        if (settingGround.tilemap.GetTile(settingGround.tilePos) != null && !stopped)
                        {
                            if (hit.collider.IsUnityNull() && useCode == 2) // ������ �������� ������, ���� �� ��� ��� ���������
                            {
                                StartCoroutine(spirit.GoToGround());
                                moving = spirit.moveToPoint;
                                StartCoroutine(player_use());

                                // �������� ������ ��������, � ������� �����������������
                                obj = null;
                                spirit = null;
                                spiritAnim = null;
                                useCode = 0;
                            }
                            else
                            {
                                Debug.Log("������");
                            }
                        }
                        else if (settingGround.tilemap.GetTile(settingGround.tilePos) == null && useCode != 2 && !stopped) // ������ �������� ����� ��� ������� ���������� �����, ���� �� ����� ��������
                        {
                            settingGround.tilemap.SetTile(settingGround.tilePos, settingGround.tile); // ��������� ����
                            counterGround++;
                        }
                        break;

                    case 1:
                        // ������� �������� ������ ���� ��� ����� ���� ���� + ������ �� ��������, ����:
                        // 1. �� ���� ����� �������
                        // 2. ������ ��� ������ ��������� ��� �� ��� ���� �������
                        
                        if (settingGround.tilemap.GetTile(settingGround.tilePos) != null &&
                            settingGround.tilemap.GetTile(settingGround.tilePos).name == "RT_GardenGround" &&
                            useCode !=2 && hit.collider.IsUnityNull() && !moving)
                        {
                            settingGround.tilemap.SetTile(settingGround.tilePos, null); // ������� ����
                            counterGround--;
                        }
                        break;

                    default:
                        break;
                }
                
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // ���� � ���������� ������ ���� ���������� � ��������, ������� ���, ����� ������ ����
        // ������� ������� ��������, ���� �������� �����������, �������� � ������� �������� �������
        switch (collision.tag)
        {
            case "SpiritTrigger":
                if (useCode == 0)
                {
                    useCode = 1;
                    collName = collision.transform.parent.name; // �������� ��� ������������� ������� �� ��������, � ������� �����
                }
                break;
            case "Garden":
                if (useCode == 0)
                {
                    useCode = 4;
                }
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Garden":
                plantAnim = collision.GetComponentInParent<Animator>();
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "SpiritTrigger":
                if (useCode == 1)
                {
                    useCode = 0;
                    collName = null; // �������� ��� �������
                }
                break;
            case "Garden":
                useCode = 0;
                break;
        }
    }

    IEnumerator player_use()
    {
        // ��������� ������
        controller.speed = 0f;
        controller.used = true;
        controller.player_anim.SetBool(name: "isUse", value: true);

        yield return new WaitForSeconds(1.5f); // ����� ��������������

        // ����������� ����������� ���������
        controller.player_anim.SetBool(name: "isUse", value: false);
        controller.speed = controller.current_speed;
        controller.used = false;

        yield return new WaitForSeconds(1.5f);
        moving = false; // ����� ����� ���� ����� ������� �����

        yield return null;
    }

    void StopAnimations()
    {
        stopped = true;
        Time.timeScale = 0f; // ���������� �����

        foreach (Component component in components) // ���������� ��� �������
        {
            Animator animator = component as Animator;
            animator.speed = 0f;
        }

        // ��������� ������ ��������� � ��������� ��� ��������
        controller.speed = 0f;
        controller.player_anim.enabled = false;
    }

    void PlayAnimations()
    {
        stopped = false;
        Time.timeScale = 1f; // ����������� �����

        foreach (Component component in components) // ���������� ��� �������
        {
            Animator animator = component as Animator;
            animator.speed = 1f;
        }

        // ��������� ������ ��������� � �������� ��� ��������
        controller.speed = controller.current_speed;
        controller.player_anim.enabled = true;
    }
}
