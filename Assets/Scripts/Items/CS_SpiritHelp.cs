using UnityEngine;

public class CS_ColorHelp : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator anim;
    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �������� ������� ��������
        if (collision.tag == "Player")
        {
            anim.SetTrigger("Triggered"); // ������� �������� �������
            anim.SetTrigger("Triggered"); // ����� ���������. �����, ������
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // ���������
        anim.SetTrigger("Left");
    }
}
