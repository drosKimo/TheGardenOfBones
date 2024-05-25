using TMPro;
using UnityEngine;

            // ������ ��� ���������
public class CS_SpeechBoxScript : MonoBehaviour
{
    public int counter = 0; // ������� �������� ����������� ����

    TMP_Text speechBox; // ��������� ����

    private void Awake()
    {
        speechBox = GameObject.Find("SP_text").GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        counter++;
        Time.timeScale = 0f;
        TutorialScene();
    }

    public void TutorialScene()
    {
        switch (counter)
        {
            case 1:
                speechBox.text = "����� ���������� � ��� ������, ������!";
                break;

            case 2:
                speechBox.text = "�������! ������ �������� �������� ���� �� ���";
                break;

            case 3:
                speechBox.text = "������, ������ �� ������ ��� �������� � �����";
                break;

            case 4:
                speechBox.text = "������ �������? ��� ������, ��� ��� ����� ���� ������";
                break;

            case 5:
                speechBox.text = "������ ���� ������ ����� ������ �� �����!";
                break;

            case 6:
                speechBox.text = "��� � ���� ���������, ��� ����� ����";
                break;

            case 7:
                speechBox.text = "������ ������ ����! �� ������� ��������� �� �����";
                break;
        }
    }
}
