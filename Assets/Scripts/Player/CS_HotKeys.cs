using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_HotKeys : MonoBehaviour
{
    // ������������ ������:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    CS_PlayerController controller;

    int useCode = 0;
    bool stopped = false;
    string collName;

    Component[] components;
    Animator spiritAnim;

    private void Awake()
    {
        // ��� ��������������
        controller = GetComponent<CS_PlayerController>();
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
                            // 2 = ������� (� �����)
                            // 3 = �������� ����� (+ �������)
                            // 4 = ��������� �� �����

                            case 1:
                                useCode = 2;
                                Object obj = GameObject.Find(collName); // ����� ������� �� ����� 
                                spiritAnim = obj.GetComponent<Animator>();

                                spiritAnim.SetTrigger("Arised"); // ������� �������� �������
                                spiritAnim.SetInteger("SetSpirit", 1); // ���������� ������ � CS_SpiritController.cs

                                StartCoroutine(player_use()); // �������� � ���������� �������� �������������
                                break;

                            case 2:
                                spiritAnim.SetInteger("SetSpirit", 2);
                                StartCoroutine(player_use());
                                spiritAnim = null;
                                useCode = 0;
                                break;
                        }
                        break;

                    case KeyCode.Escape: // �����, ���������� ��������
                        components = FindObjectsOfType<Animator>(); // �������� ��� ���������� ���� Animator �� �����

                        if (stopped)
                            PlayAnimations();
                        else
                            StopAnimations();
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

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
