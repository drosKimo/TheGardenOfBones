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
