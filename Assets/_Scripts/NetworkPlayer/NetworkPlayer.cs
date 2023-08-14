using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<Vector3> movementDirection = new NetworkVariable<Vector3>(
        Vector3.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 180f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
            enabled = false;
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        if (movementDirection.Value == Vector3.zero) return;

        ApplyMovement();
        ApplyRotation();
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movementInput = new Vector3(horizontal, 0, vertical);
        movementInput.Normalize();

        movementDirection.Value = movementInput;
    }

    private void ApplyMovement()
    {
        transform.Translate(movementDirection.Value *
                            movementSpeed *
                            Time.fixedDeltaTime, Space.World);

    }
    private void ApplyRotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection.Value);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
