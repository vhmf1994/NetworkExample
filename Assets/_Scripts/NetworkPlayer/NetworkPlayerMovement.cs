using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField, ReadOnly] private Vector3 movementVector;
    [SerializeField, ReadOnly] private Vector3 movementInput;

    [SerializeField] private float movementSpeed = 4f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
            enabled = false;
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();

        transform.position = movementVector;
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movementInput = new Vector3(horizontal, 0, vertical);
    }
    private void HandleMovement()
    {
        movementVector += Time.deltaTime * movementSpeed * movementInput;
    }

    [ServerRpc]
    private void OnMovementVectorValueChangedServerRpc(Vector3 previous, Vector3 current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");

        transform.position = current;
    }
}
