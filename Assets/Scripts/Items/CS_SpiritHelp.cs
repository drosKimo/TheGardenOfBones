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
            case "Player": // �������� ������� ��������
                anim.SetTrigger("Triggered"); // ������� �������� �������
                anim.SetTrigger("Triggered"); // ����� ���������. �����, ������
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player": 
                // ���������
                anim.SetTrigger("Left");
                break;
        }
    }
}
