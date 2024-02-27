using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CS_SpiritController : MonoBehaviour
{
    [HideInInspector] public Animator spiritAnim;
    [SerializeField] Transform playerTransform;
    [SerializeField] float speed;

    CS_SpiritHelp spiritHelp;
    NavMeshAgent meshAgent;
    SpriteRenderer spiritRender;

    Vector3 player_pos;
    bool guided = false;
    float distance = 1.6f;

    private void Awake()
    {
        spiritAnim = GetComponent<Animator>();
        spiritHelp = GetComponentInChildren<CS_SpiritHelp>();
        meshAgent = GetComponent<NavMeshAgent>();
        spiritRender = gameObject.GetComponent<SpriteRenderer>();

        meshAgent.updateRotation = false;
        meshAgent.updateUpAxis = false;
    }
    private void Update()
    {
        switch (spiritAnim.GetInteger("SetSpirit"))
        {
            case 1:
                guided = true;
                spiritAnim.SetInteger("SetSpirit", 0);
                spiritHelp.anim.SetTrigger("Left");
                StartCoroutine(HelpDestroyer());
                break;
            case 2:
                guided = false;
                spiritAnim.SetInteger("SetSpirit", 0);
                spiritAnim.SetTrigger("Planted");
                meshAgent.isStopped = true; // ����� ���������� ������, ���� �� ������. ��������
                StartCoroutine (TriggerDestroyer());
                break;
        }

        if (guided == true)
        {
            player_pos = playerTransform.position;

            meshAgent.destination = player_pos;
            meshAgent.stoppingDistance = distance;

            if ((gameObject.transform.position - player_pos).x > 0) // ��������� ������� ������������ ���� �����
                spiritRender.flipX = true; // �������� ���������������
            else spiritRender.flipX = false;

            spiritAnim.SetBool(name: "GoRight", value: meshAgent.remainingDistance > distance); // ��������� ������ ����� ���������� ������ ��������� ���������
        }
    }

    IEnumerator HelpDestroyer()
    {
        yield return new WaitForSeconds(1.5f); // ������ �� ��������

        foreach (Transform child in gameObject.transform) // ������ � ��������� ���������
        {
            if (child.tag == "Help")
            {
                Destroy(child.gameObject);
            }
        }

        yield return null;
    }
    IEnumerator TriggerDestroyer()
    {
        foreach (Transform child in gameObject.transform) // ������ � ��������� ���������
        {
            if (child.tag == "SpiritTrigger")
            {
                Destroy(child.gameObject);
            }
        }

        yield return new WaitForSeconds(2.5f); // ������ �� ��������

        Destroy(gameObject);

        yield return null;
    }
}
