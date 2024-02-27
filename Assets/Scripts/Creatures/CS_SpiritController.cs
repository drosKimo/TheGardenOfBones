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
    CS_SettingGround settingGround;
    NavMeshAgent meshAgent;
    SpriteRenderer spiritRender;

    Vector3 nextPosition;
    bool guided = false;

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
                spiritAnim.SetInteger("SetSpirit", 0);
                spiritAnim.SetTrigger("Planted");
                StartCoroutine (TriggerDestroyer());
                break;
        }

        if (guided == true) // ���� �� ��������, ����� ������ �� �������
        {
            nextPosition = playerTransform.position;

            meshAgent.destination = nextPosition;
            meshAgent.stoppingDistance = 1.6f;
            AnimationChecker();
            nextPosition = Vector3.zero;
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

        yield return new WaitForSeconds(2.5f); // ������ �� �������� ����� �������

        Destroy(gameObject);

        yield return null;
    }

    public IEnumerator GoToGround()
    {
        guided = false;

        settingGround = GameObject.Find("GroundTest").GetComponent<CS_SettingGround>();
        nextPosition = settingGround.tilemap.GetCellCenterWorld(settingGround.tilePos) + new Vector3(0, 0.2f, 0);
        // ����� �������� ������� � �������, ����� ������� ������� �������� �� ������ ������

        meshAgent.stoppingDistance = 0;
        meshAgent.SetDestination(nextPosition);
        AnimationChecker();
        nextPosition = Vector3.zero;

        yield return new WaitForSeconds(1.5f); // ����� �� ����� �� �����
        
        spiritAnim.SetInteger("SetSpirit", 2);

        yield break;
    }

    void AnimationChecker()
    {
        if ((gameObject.transform.position - nextPosition).x > 0) // ��������� ������� ������������ ���� �����
            spiritRender.flipX = true; // �������� ���������������
        else spiritRender.flipX = false;

        spiritAnim.SetBool(name: "GoRight", value: meshAgent.remainingDistance > meshAgent.stoppingDistance); // ��������� ������ ����� ���������� ������ ��������� ���������
    }
}
