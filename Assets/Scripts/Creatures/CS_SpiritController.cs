using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CS_SpiritController : MonoBehaviour
{
    [HideInInspector] public Animator spiritAnim;
    [HideInInspector] public bool moveToPoint = false;
    Transform playerTransform;
    [SerializeField] float speed;

    CS_SpiritHelp spiritHelp;
    CS_SettingGround settingGround;
    CS_HotKeys hotkeys;

    NavMeshAgent meshAgent;
    SpriteRenderer spiritRender;
    Animator playerAnim;

    public Vector3 nextPosition;
    public bool guided = false;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerAnim = playerTransform.gameObject.GetComponent<Animator>();
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
                playerAnim.SetBool("Guid", true);
                guided = true;
                spiritAnim.SetInteger("SetSpirit", 0);
                spiritHelp.anim.SetTrigger("Left");
                StartCoroutine(HelpDestroyer());
                break;
            case 2:
                playerAnim.SetBool("Guid", false);
                spiritAnim.SetInteger("SetSpirit", 0);
                spiritAnim.SetTrigger("Planted");
                StartCoroutine (TriggerDestroyer());
                break;
        }

        if (guided == true) // пока не посадили, будет ходить за игроком
        {
            nextPosition = playerTransform.position;

            meshAgent.destination = nextPosition;
            meshAgent.stoppingDistance = 1.6f;
            AnimationChecker();
            nextPosition = Vector3.zero;
        }
        else if (moveToPoint == true && meshAgent.remainingDistance <= 0.1f)
        {
            hotkeys = playerTransform.GetComponent<CS_HotKeys>();
            spiritAnim.SetInteger("SetSpirit", 2);

            StartCoroutine(PlantSeed());
        }
    }

    IEnumerator HelpDestroyer()
    {
        yield return new WaitForSeconds(1.5f); // таймер на удаление

        foreach (Transform child in gameObject.transform) // работа с дочерними объектами
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
        foreach (Transform child in gameObject.transform) // работа с дочерними объектами
        {
            if (child.tag == "SpiritTrigger")
            {
                Destroy(child.gameObject);
            }
        }

        Destroy(gameObject, 2.5f); // таймер на удаление всего объекта

        yield return null;
    }

    public IEnumerator GoToGround()
    {
        guided = false;

        settingGround = GameObject.Find("GroundTest").GetComponent<CS_SettingGround>();
        nextPosition = settingGround.tilemap.GetCellCenterWorld(settingGround.tilePos) + new Vector3(0, 0.2f, 0);
        // нужно добавить немного к вектору, чтобы призрак садился примерно по центру грядки

        meshAgent.stoppingDistance = 0;
        meshAgent.SetDestination(nextPosition);
        AnimationChecker();
        moveToPoint = true;

        yield return null;
    }

    IEnumerator PlantSeed()
    {
        moveToPoint = false;

        yield return new WaitForSeconds(1.5f); // таймер до появления семечки

        // создает растение на месте, куда сел призрак
        Instantiate(hotkeys.plant, (nextPosition - new Vector3(0, 0.2f, 0)), Quaternion.Euler(0, 0, 0));

        yield return null;
    }

    void AnimationChecker()
    {
        if ((gameObject.transform.position - nextPosition).x > 0) // сравнение позиций относительно друг друга
            spiritRender.flipX = true; // моделька разворачивается
        else spiritRender.flipX = false;

        spiritAnim.SetBool(name: "GoRight", value: meshAgent.remainingDistance > meshAgent.stoppingDistance); // двигается только когда расстояние больше дистанции остановки
    }
}
