using System.Collections;
using UnityEngine;

public class CS_SpiritController : MonoBehaviour
{
    [HideInInspector] public Animator spiritAnim;
    CS_SpiritHelp spiritHelp;

    private void Awake()
    {
        spiritAnim = GetComponent<Animator>();
        spiritHelp = GetComponentInChildren<CS_SpiritHelp>();
    }
    private void Update()
    {
        switch (spiritAnim.GetInteger("SetSpirit"))
        {
            case 1:
                spiritAnim.SetInteger("SetSpirit", 0);
                spiritHelp.anim.SetTrigger("Left");
                StartCoroutine(HelpDestroyer());
                break;
        }
    }

    IEnumerator HelpDestroyer()
    {
        yield return new WaitForSeconds(1.5f); // таймер на удаление

        foreach (Transform child in gameObject.transform) // работа с дочерними объкектами
        {
            if (child.tag == "Help")
            {
                Destroy(child.gameObject);
            }
        }

        yield return null;
    }
}
