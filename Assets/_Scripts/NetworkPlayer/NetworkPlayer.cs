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

    private void Update()
    {
        if (!IsOwner) return;

        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (movementDirection.Value == Vector3.zero) return;

        HandleMovementSpeed();

        ApplyMovement();
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movementInput = new Vector3(horizontal, 0, vertical);
        movementInput.Normalize();

        movementDirection.Value = movementInput;
    }

    private void HandleMovementSpeed()
    {
        movementDirection.Value *= movementSpeed * Time.fixedDeltaTime;
    }

    private void ApplyMovement()
    {
        transform.position += Vector3.Lerp(transform.position,
                                           movementDirection.Value,
                                           Time.deltaTime);
    }
}
