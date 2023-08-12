using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayerMovement : NetworkBehaviour
{
    [Title("Components")]
    [SerializeField, ReadOnly] private Rigidbody rb;

    [Title("Movement Configuration")]
    [SerializeField, Range(1f, 10f)] float moveSpeed = 5.0f;
    [SerializeField, Range(5f, 90f)] float rotationSpeed = 10.0f;
    [SerializeField, Range(3f, 15f)] float jumpForce = 7.0f;
    [SerializeField, Range(0.1f, 0.5f)] float groundRaycastDistance = 0.2f;

    [SerializeField, ReadOnly] private Vector3 movement;

    [SerializeField, ReadOnly] private bool isGrounded;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;

        HandleInputVectors();
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) return;

        HandleIsGrounded();
        HandlerMovement();
        HandleRotation();
        HandleJump();

        ApplyForces();
    }

    private void ApplyForces()
    {
        // Aplica a gravidade manualmente
        if (!isGrounded)
        {
            Vector3 gravity = Physics.gravity;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    private void HandleInputVectors()
    {
        // Movimento horizontal
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        movement = new Vector3(horizontalInput, 0.0f, verticalInput).normalized * moveSpeed * Time.deltaTime;
    }

    private void HandlerMovement()
    {
        rb.MovePosition(rb.position + movement);
    }

    private void HandleRotation()
    {
        // Rotação
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        // Pulo
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleIsGrounded()
    {
        // Verifica se o jogador está no chão
        isGrounded = Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, groundRaycastDistance);
    }
}
