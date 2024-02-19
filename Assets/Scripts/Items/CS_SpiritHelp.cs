using UnityEngine;

public class CS_SpiritHelp : MonoBehaviour
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // включает очередь анимаций
        if (collision.tag == "Player")
        {
            anim.SetTrigger("Triggered"); // сначала включает триггер
            anim.SetTrigger("Triggered"); // потом выключает. Иначе, багует
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // выключает
        anim.SetTrigger("Left");
    }
}
