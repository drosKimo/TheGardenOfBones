using UnityEngine;

public class CS_SpiritHelp : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player": // включает очередь анимаций
                anim.SetTrigger("Triggered"); // сначала включает триггер
                anim.SetTrigger("Triggered"); // потом выключает. Иначе, багует
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player": 
                // выключает
                anim.SetTrigger("Left");
                break;
        }
    }
}
