using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 9f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    public float attackRange = 2.5f;
    public float attackDamage = 25f;

    private CharacterController controller;
    private float verticalVelocity;
    private float nextAttackTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerHealth health = GetComponent<PlayerHealth>();

        if (health != null && health.IsDead())
        return;    
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontal, 0f, vertical);
        move = Vector3.ClampMagnitude(move, 1f);

        float currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? runSpeed : speed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 0.5f;
        }
    }

    void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange
        );

        foreach (Collider hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();

            if (enemy != null)
                enemy.TakeDamage(attackDamage);
        }
    }
}
