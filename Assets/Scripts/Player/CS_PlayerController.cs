using UnityEngine;

public class CS_PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] public float speed;
    
    // должны быть спрятаны в инспекторе Unity
    [HideInInspector] public Animator player_anim;
    [HideInInspector] public Vector2 moveVector;
    [HideInInspector] public float current_speed;
    [HideInInspector] public bool used;

    SpriteRenderer SortRend;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player_anim = GetComponent<Animator>();
        current_speed = speed; // фиксирование скорости
    }

    void Update()
    {
        if (used) // нужна проверка, иначе багуются анимации
        {
            
        }
        else
        {
            // перемещение
            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.y = Input.GetAxis("Vertical");
            rb.MovePosition(rb.position + moveVector * speed * 0.017f * Time.timeScale);

            // анимации
            player_anim.SetBool(name: "isMovingUp", value: moveVector.y > 0);
            player_anim.SetBool(name: "isMovingLeft", value: moveVector.x < 0);
            player_anim.SetBool(name: "isMovingRight", value: moveVector.x > 0);
            player_anim.SetBool(name: "isMovingDown", value: moveVector.y < 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // при входе в определенную коллизию, игрок оказывается как бы позади объекта
        switch (collision.gameObject.tag)
        {
            case "Dead":
                SortRend = collision.gameObject.GetComponent<SpriteRenderer>();
                SortRend.sortingOrder = 2;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // при выходе, игрок снова перед объектом
        switch (collision.gameObject.tag)
        {
            case "Dead":
                SortRend = collision.gameObject.GetComponent<SpriteRenderer>();
                SortRend.sortingOrder = 0;
                break;
        }
    }
}
