using System.Collections;
using UnityEngine;

public class CS_HotKeys : MonoBehaviour
{
    // ������������ ������:
    // https://null-code.ru/solution/142-menyu-privyazki-klavish-sohranenie.html

    CS_PlayerController controller;

    [HideInInspector] public bool aaa; // ��� ����� ���� ���� ��� �������� ���������� � ���� ��������������

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
                        {
                            StartCoroutine(player_use());
                            break;
                        }
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
        controller.speed = 0;
        controller.used = true;
        controller.player_anim.SetBool(name: "isUse", value: true);

        yield return new WaitForSeconds(1.5f); // ����� ��������������

        // ����������� ����������� ���������
        controller.player_anim.SetBool(name: "isUse", value: false);
        controller.speed = controller.current_speed;
        controller.used = false;

        yield return null;
    }
}
