using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CS_PlantBehaviour : MonoBehaviour
{
    // ����� ����� ����� ������ � ���������� ������� ����� �� �����

    [SerializeField] public GameObject help;
    [HideInInspector] public float timeLeft, angry_timeLeft; // ����� � ��������
    [HideInInspector] public Animator animHelp;

    TMP_Text text;
    Animator animObj, animPlayer;
    // ���� ����������� ������������� ��� �������� "����" �����
    int angryMult = 1;

    private void Awake()
    {
        animHelp = help.GetComponent<Animator>();
        animObj = GetComponent<Animator>();
        text = GameObject.Find("DaytimeText").GetComponent<TMP_Text>();
        animPlayer = GameObject.Find("Player").GetComponent<Animator>();

        animObj.SetBool("TimeRun", true);
    }

    void LateUpdate()
    {
        switch (text.text)
        {
            case "night time": // ����� ������ ������������������
                animObj.SetInteger("TimeUp", 0);
                break;

            case "work time": // ���� ������ ����
                if (animObj.GetBool("TimeRun"))
                {
                    angryMult = 0;
                    timeLeft = 5; // ���� �� ������ ����� �����, ������ �� ��������� ��������� ������
                    animObj.SetBool("TimeRun", false); // ������ �� ���������� �������
                    StartCoroutine(StartTimer()); // ������
                }

                animObj.SetInteger("TimeUp", 1); // �������������, ��� ���� ������ ���� �� ��������� 1

                break;
        }
    }

    private IEnumerator StartTimer()
    {
        // ������ ���� ������ �����
        while (timeLeft > 0)
        {
            //Debug.Log(timeLeft);

            // TimeUp = ��������� ������� �������
            timeLeft -= Time.deltaTime * animObj.GetInteger("TimeUp");
            UpdateTime();
            yield return null;
        }
    }

    private IEnumerator AngryTimer()
    {
        // ������ ���� ������ �����
        // ���� �� ������ �� �����, �������� ��������
        while (angry_timeLeft > 0)
        {            
            // TimeUp = ��������� ������� �������
            angry_timeLeft -= Time.deltaTime * animObj.GetInteger("TimeUp") * angryMult;
            UpdateAngryTime();

            yield return null;
        }
    }

    private void UpdateTime()
    {
        if (timeLeft < 0) // ����� ������ �����
        {
            animHelp.SetTrigger("Triggered"); // �������� ��������� "������"
            timeLeft = 0;
            angry_timeLeft = 10;
            angryMult = 1;
            StartCoroutine(AngryTimer()); // "����" ������
        }
    }

    private void UpdateAngryTime()
    {
        if (angry_timeLeft < 0) // ����� ������ �����
        {
            animHelp.SetTrigger("Left"); // ��������� "������"
            angry_timeLeft = 0;
            animPlayer.SetInteger("AngrySpirit", 1);
            Destroy(gameObject, 0.5f);
        }
    }
}
