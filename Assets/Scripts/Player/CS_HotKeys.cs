using System.Collections;
using UnityEngine;

public class CS_HotKeys : MonoBehaviour
{
    // ������������ ������:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    bool stopped = false;
    CS_PlayerController controller;

    [HideInInspector] public bool aaa; // ��� ����� ���� ���� ��� �������� ���������� � ���� ��������������

    Component[] components;

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
                    case KeyCode.F: // �������������
                        StartCoroutine(player_use());
                        break;

                    case KeyCode.Escape: // �����, ���������� ��������
                        components = FindObjectsOfType<Animator>(); // �������� ��� ���������� ���� Animator �� �����

                        if (stopped)
                            PlayAnimations();
                        else
                            StopAnimations();
                        break;

                    default:
                        break;
                }
            }
            if (pushed.isMouse)
            {

            }
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
