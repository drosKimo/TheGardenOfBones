using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Menu
{
    public GameObject MenuPrefab;
    public GameObject SpeechPrefab;
}

public class CS_HotKeys : MonoBehaviour
{
    // ������������ ������:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    [SerializeField] public GameObject plant;
    [SerializeField] TMP_Text daytimeText;
    [HideInInspector] public Object obj;

    CS_PlayerController controller;
    CS_SettingGround settingGround;
    CS_SpiritController spirit;
    CS_DaytimeTimer daytimeTimer;
    CS_SpeechBoxScript boxScript;
    CS_SpawnCreature spawnCreature;
    public Menu menu = new Menu();

    int useCode = 0, // ��� ��� ��������������
        speechCode = 0, // ��� �������
        counterGround = 0, // ������� ������ �����
        counterSpirits = 0, // ������� �������� ��������� (����� ������)
        counterHappySpirits = 0, // ������� ������ ������ � ��������
        counterTotalSpirits = 0, // ������� ������� ����� ���� ������� � �������� ���������
        counterDays = 0; // ������� ���� 
    bool stopped = false, moving = false;
    string collName;
    float finalPercent;

    Component[] components;
    Animator spiritAnim, plantAnim, playerAnim;
    TMP_Text textSpeech;
    public List<Sprite> dayKeys = new List<Sprite>(); // ������ �������� ����

    private void Awake()
    {
        controller = GetComponent<CS_PlayerController>(); // ��� ��������������

        GameObject ground = GameObject.Find("GroundTest");

        settingGround = ground.GetComponent<CS_SettingGround>(); // ��� ������� �����
        playerAnim = controller.gameObject.GetComponent<Animator>();
        daytimeTimer = GameObject.Find("Slider").GetComponent<CS_DaytimeTimer>();
        boxScript = GameObject.Find("SpeechBoxScripter").GetComponent<CS_SpeechBoxScript>(); // ��� ������ � ��������
        textSpeech = GameObject.Find("SP_text").GetComponent<TMP_Text>(); // ��� ������ � ������� � ���������� ����

        GameObject speech = GameObject.Find("Speech");
        
        if (SceneManager.GetActiveScene().name != "Tutorial")
            speech.SetActive(false);
    }

    void OnGUI()
    {
        if (playerAnim.GetInteger("AngrySpirit") == 1)
        {
            counterSpirits--;
            playerAnim.SetInteger("AngrySpirit", 0);
        }

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
                            // 3 = ����������
                            // 4 = ��������� �� �����

                            case 1:
                                // ���������, ���� �� �� ����� ��������� ����� ��� �������
                                // ��������� ��������� ���������, ���� ������� ����� ������ 
                                if (counterGround > counterSpirits && daytimeText.text == "work time") 
                                {
                                    counterSpirits++;
                                    counterTotalSpirits++;
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

                            case 3:
                                if (daytimeTimer.timeLeft > 0)
                                {
                                    menu.SpeechPrefab.SetActive(true);
                                    Time.timeScale = 0f; // ���������� �����
                                    speechCode = 0;

                                    textSpeech.text = "��� �����? ������ ��� ������! ����������� � ������";
                                }
                                else
                                {
                                    menu.SpeechPrefab.SetActive(true);
                                    speechCode = 1;
                                    Time.timeScale = 0f; // ���������� �����

                                    textSpeech.text = "��� ��� ������? � � � �� �������!";
                                }
                                break;

                            case 4:
                                // �������� ������ � �������-��������
                                // plantAnim = ���������-��������, ��� ��� ������� ����� ���������� � ��� �������� �������
                                CS_PlantBehaviour plantBH = plantAnim.gameObject.GetComponent<CS_PlantBehaviour>();
                                if (plantBH.timeLeft <= 0 && daytimeTimer.timeLeft > 0) // ����� ������ ����� � ���� ��� ��� ����
                                {
                                    plantBH.animHelp.SetTrigger("Left"); // ������� ������� "������" ��� ���������

                                    plantAnim.SetBool("TimeRun", true); // ��������� ����� ��������� ������
                                    plantAnim.SetInteger("StateCounter", plantAnim.GetInteger("StateCounter") + 1); // ��������� ������ �����
                                    if (plantAnim.GetInteger("StateCounter") == 3)
                                    {
                                        counterSpirits--;
                                        counterHappySpirits++;
                                        Destroy(plantAnim.gameObject, 1.5f);
                                    }
                                }
                                break;
                        }
                        break;

                    case KeyCode.Space: // �������/����������� ���������� ����
                        if (SceneManager.GetActiveScene().name == "Tutorial")
                        {
                            switch (speechCode)
                            {
                                // 0 = ������
                                // 1 = ������� ������
                                // 2 = �������� �������
                                // 3 = ������� � �������
                                // 4 = � �������
                                // 5 = � ����������
                                // 6 = ����� ��������

                                case 0:
                                    textSpeech.text = "����� ����� � �����. ������� ����� ������";
                                    speechCode = 1;
                                    break;

                                case 1:
                                    menu.SpeechPrefab.SetActive(false);
                                    speechCode = 2;
                                    break;

                                case 2:
                                    switch (boxScript.counter)
                                    {
                                        case 3:
                                            textSpeech.text = "�������, ���� �� ����� ����� ������";
                                            speechCode = 3;
                                            break;
                                        case 5:
                                            textSpeech.text = "�� �� �� ������� �������� ���� �� ��� ������...";
                                            speechCode = 4;
                                            break;
                                        case 7:
                                            textSpeech.text = "�������� ��������, ���� �� �� ������ ��������� �� ���";
                                            speechCode = 5;
                                            break;
                                        default:
                                            menu.SpeechPrefab.SetActive(false);
                                            break;
                                    }
                                    break;

                                case 3:
                                    textSpeech.text = "����! ������ ����� ������� �����? ������� � ����";
                                    speechCode = 1;
                                    break;

                                case 4:
                                    textSpeech.text = "�������� �������� ���";
                                    speechCode = 1;
                                    break;

                                case 5:
                                    textSpeech.text = "�� ��� � ��, ������ �� ����� � ������!";
                                    speechCode = 6;
                                    break;

                                case 6:
                                    SceneManager.LoadScene("MainScene");
                                    break;
                            }
                        }
                        else
                        {
                            switch (speechCode)
                            {
                                // 0 = ������� ������
                                // 1 = ����������� �������
                                // 2 = ����� ���
                                // 3 = ����� �� ����
                                // 4 = ��������� ��������� (�������)
                                // 5 = ��������� ����������� (�������)

                                case 0: // ���� ��� �� ������
                                    menu.SpeechPrefab.SetActive(false);
                                    Time.timeScale = 1f;
                                    break; 

                                case 1:
                                    if (counterDays < 6) // ���� ������� ����� ����� � ������ ������ 7 ����
                                    {
                                        textSpeech.text = "�����, �� ������!";
                                        Time.timeScale = 1f;
                                        speechCode = 2;

                                        // ��� �������� ��������-��������� ��� ��������
                                        GameObject[] wolf = new GameObject[GameObject.FindGameObjectsWithTag("SpiritTrigger").Length];
                                        wolf = GameObject.FindGameObjectsWithTag("SpiritTrigger");

                                        foreach (GameObject sp in wolf)
                                        {
                                            // ������� ������� ����� � �������
                                            Destroy(sp.transform.parent.gameObject);
                                        }

                                        // � ����� ������� �����
                                        GameObject[] spawn = new GameObject[GameObject.FindGameObjectsWithTag("Respawn").Length];
                                        spawn = GameObject.FindGameObjectsWithTag("Respawn");

                                        foreach(GameObject sp in spawn)
                                        {
                                            spawnCreature = sp.GetComponent<CS_SpawnCreature>();
                                            spawnCreature.Spawn();
                                        }
                                    }
                                    else if (counterDays == 6) // ����� ����
                                    {
                                        // �� �������� � DOUBLE
                                        // �������� ��������� ��������� ������
                                        finalPercent = ((float)counterTotalSpirits / (float)playerAnim.GetInteger("SpawnedCreatures")) * 100;
                                        finalPercent = (float)System.Math.Round(finalPercent, 2); // ���������� �� 2 ������ ����� �������

                                        textSpeech.text = $"�� ������ ��������� � {finalPercent}%";
                                        speechCode = 4;
                                    }
                                    break;

                                case 2:
                                    menu.SpeechPrefab.SetActive(false);

                                    Image dayImg = GameObject.Find("DaytimeImage").GetComponent<Image>();

                                    counterDays++;
                                    dayImg.sprite = dayKeys[counterDays];
                                    daytimeText.text = "work time";

                                    StartCoroutine(daytimeTimer.StartTimer()); // ���������� ������� ���
                                    break;

                                case 3:
                                    Application.Quit();
                                    break;

                                case 4:
                                    if (finalPercent < 25) // 0 - 24.99 %
                                        textSpeech.text = "� ������ ������ �������������� ���?..";
                                    else if (finalPercent >= 25 && finalPercent < 50) // 25 - 49.99 %
                                        textSpeech.text = "����� ���� � �����...";
                                    else if (finalPercent >= 50 && finalPercent < 75) // 50 - 74.99 %
                                        textSpeech.text = "�������.";
                                    else if (finalPercent >= 75 && finalPercent < 100) // 75 - 99.99 %
                                        textSpeech.text = "�������� ������!";
                                    else if (finalPercent == 100) // 100 %
                                        textSpeech.text = "������ ����! �� �����(�) ���� ������, ��� ���� � ���!";

                                    speechCode = 5;
                                    break;

                                case 5:
                                    float happyPercent = ((float)counterHappySpirits / (float)counterTotalSpirits) * 100;

                                    if (counterTotalSpirits - counterHappySpirits > 0)
                                    {
                                        if (happyPercent == 0) // 0 %
                                            textSpeech.text = "��� ���� ������ ����! ����������!";
                                        else if (happyPercent < 45) // 0.01 - 44.99 %
                                            textSpeech.text = $"������ ����... �� {counterTotalSpirits} ���������� ����� {counterHappySpirits}. �����, ����.";
                                        else if (happyPercent >= 45 && happyPercent < 75) // 45 - 74.99 %
                                            textSpeech.text = $"�� ������� ����, �� ��������(���). �� {counterTotalSpirits} ���������� ������ {counterHappySpirits}. ������ ����.";
                                        else if (happyPercent >= 75 && happyPercent < 100) // 75 - 99.99 %
                                            textSpeech.text = $"���������! �� ���� ������ ����� {counterHappySpirits} ��� �� {counterTotalSpirits}. �������, ������!";
                                    }
                                    else if (counterTotalSpirits == 0)
                                        textSpeech.text = "�� ���� �� ���������(���)! ����������!";
                                    else
                                        textSpeech.text = "��� ����, ������� �� �������, ��������. �������, ������!";

                                    speechCode = 3;
                                    break;
                            }
                        }
                        break;

                    case KeyCode.Escape: // �����, ���������� ��������

                        // ���������� ��� ���������, �������� ������� ������
                        if (SceneManager.GetActiveScene().name == "Tutorial")
                        {
                            SceneManager.LoadScene("MainScene");
                        }
                        else
                        {
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
                        }
                        break;

                    case KeyCode.L: // ������� ��� ��� ������
                        daytimeTimer.timeLeft = 0;
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
                        else if (hit.collider.IsUnityNull() && settingGround.tilemap.GetTile(settingGround.tilePos) == null && useCode != 2 && !stopped) // ������ �������� ����� ��� ������� ���������� �����, ���� �� ����� ��������
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

            case "Reaper":
                if (useCode == 0)
                {
                    useCode = 3;
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

            case "Reaper":
                if (playerAnim.GetBool("Guid") == false)
                    useCode = 0;
                else
                    useCode = 2;
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
