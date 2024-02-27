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
                meshAgent.isStopped = true; // чтобы остановить агента, если он далеко. ВРЕМЕННО
                StartCoroutine (TriggerDestroyer());
                break;
        }

        if (guided == true)
        {
            player_pos = playerTransform.position;

            meshAgent.destination = player_pos;
            meshAgent.stoppingDistance = distance;

            if ((gameObject.transform.position - player_pos).x > 0) // сравнение позиций относительно друг друга
                spiritRender.flipX = true; // моделька разворачивается
            else spiritRender.flipX = false;

            spiritAnim.SetBool(name: "GoRight", value: meshAgent.remainingDistance > distance); // двигается только когда расстояние больше дистанции остановки
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

        yield return new WaitForSeconds(2.5f); // таймер на удаление

        Destroy(gameObject);

        yield return null;
    }
}
