using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_HotKeys : MonoBehaviour
{
    // ������������ ������:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    [SerializeField] GameObject plant;

    CS_PlayerController controller;
    CS_SettingGround settingGround;
    CS_SpiritController spirit;

    int useCode = 0, // ��� ��� ��������������
        counterGround = 0, // ������� ������ �����
        counterSpirits = 0; // ������� �������� ���������
    bool stopped = false;
    string collName;

    Component[] components;
    Animator spiritAnim;

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
                                    Object obj = GameObject.Find(collName); // ����� ������� �� ����� 
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
                        }
                        break;

                    case KeyCode.Tab: // �����, ���������� ��������
                        components = FindObjectsOfType<Animator>(); // �������� ��� ���������� ���� Animator �� �����

                        if (stopped)
                            PlayAnimations();
                        else
                            StopAnimations();
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

                switch (pushed.button)
                {
                    case 0:
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                        Debug.Log(hit.collider.IsUnityNull());
                        //Debug.Log(111);


                        // ������ settingGround.tilemap.GetTile(settingGround.tilePos) != null ���������, ���� �� ��� ����� ����
                        if (settingGround.tilemap.GetTile(settingGround.tilePos) != null && useCode == 2)
                        {
                            StartCoroutine(spirit.GoToGround());
                            StartCoroutine(player_use());

                            if (hit.collider.IsUnityNull())
                            {
                                //Debug.Log(hit.collider.gameObject.name);
                                Instantiate(plant, (spirit.nextPosition - new Vector3(0, 0.2f, 0)), Quaternion.Euler(0, 0, 0));
                            }


                            /*if (hit.collider.gameObject.name.Contains("Plant"))
                            {
                                Debug.Log(123);
                            }
                            else if (hit.collider == null)
                            {
                                //Debug.Log(hit.collider.gameObject.name);
                                Instantiate(plant, (spirit.nextPosition - new Vector3(0, 0.2f, 0)), Quaternion.Euler(0, 0, 0));
                            }*/


                            // �������� ������ ��������, � ������� �����������������
                            spiritAnim = null;
                            spirit = null;
                            useCode = 0;

                            //RaycastHit2D hit2D = Physics2D.Raycast(spirit.nextPosition, -Vector2.up);
                            //List<Collider2D> list = new List<Collider2D>();
                            //list.Add(Physics2D.OverlapCircle((new Vector2(spirit.nextPosition.x, spirit.nextPosition.y)), 2f));
                            //Debug.Log(hit2D.collider.gameObject.name);
                        }
                        else if (settingGround.tilemap.GetTile(settingGround.tilePos) == null && useCode != 2)
                        {
                            settingGround.tilemap.SetTile(settingGround.tilePos, settingGround.tile); // ��������� ����
                            counterGround++;
                        }
                        break;

                    case 1:
                        // ������� �������� ������ ���� ��� ����� ���� ���� + ������ �� �������� ���� �� ���� ����� �������
                        if (settingGround.tilemap.GetTile(settingGround.tilePos) != null &&
                            settingGround.tilemap.GetTile(settingGround.tilePos).name == "RT_GardenGround" &&
                            useCode !=2)
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
