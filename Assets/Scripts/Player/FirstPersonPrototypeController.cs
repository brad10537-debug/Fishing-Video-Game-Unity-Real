using UnityEngine;

// Owns first-person movement and mouse look only.
// Interaction, fishing, inventory, and UI stay in their own systems.
[RequireComponent(typeof(CharacterController))]
public class FirstPersonPrototypeController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Look")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;

    private CharacterController characterController;
    private LivingEntity livingEntity;
    private PrototypeHud prototypeHud;
    private float verticalVelocity;
    private float cameraPitch;
    private PlayerMovementState currentState = PlayerMovementState.Interacting;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        livingEntity = GetComponent<LivingEntity>();
        prototypeHud = FindAnyObjectByType<PrototypeHud>();

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        LookAround();

        if (currentState == PlayerMovementState.Sitting)
        {
            CheckForSitExitInput();
            return;
        }

        if (currentState == PlayerMovementState.Fishing)
        {
            return;
        }

        Move();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);

        if (playerCamera != null)
        {
            playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * horizontal + transform.forward * vertical;
        movement = Vector3.ClampMagnitude(movement, 1f);
        bool isTryingToSprint = Input.GetKey(sprintKey) && movement.sqrMagnitude > 0.01f;
        bool isSprinting = false;
        float currentMoveSpeed = livingEntity != null ? livingEntity.movementSpeed : moveSpeed;

        if (isTryingToSprint && livingEntity != null && livingEntity.TryUseStamina(livingEntity.sprintStaminaCostPerSecond * Time.deltaTime))
        {
            currentMoveSpeed = livingEntity.sprintSpeed;
            isSprinting = true;
        }
        else if (isTryingToSprint && livingEntity == null)
        {
            currentMoveSpeed = moveSpeed * 1.5f;
            isSprinting = true;
        }

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (characterController.isGrounded && Input.GetKeyDown(jumpKey))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 horizontalMovement = movement * currentMoveSpeed;
        horizontalMovement.y = verticalVelocity;

        characterController.Move(horizontalMovement * Time.deltaTime);

        if (movement.sqrMagnitude <= 0.01f)
        {
            SetMovementState(PlayerMovementState.Idle);
        }
        else if (isSprinting)
        {
            SetMovementState(PlayerMovementState.Sprinting);
        }
        else
        {
            SetMovementState(PlayerMovementState.Walking);
        }
    }

    public bool IsSitting()
    {
        return currentState == PlayerMovementState.Sitting;
    }

    public void EnterSitting(Transform sitPoint)
    {
        verticalVelocity = 0f;

        if (sitPoint != null)
        {
            transform.position = sitPoint.position;
            transform.rotation = Quaternion.Euler(0f, sitPoint.eulerAngles.y, 0f);
        }

        SetMovementState(PlayerMovementState.Sitting);
    }

    public void ExitSitting()
    {
        if (!IsSitting())
        {
            return;
        }

        SetMovementState(PlayerMovementState.Idle);
    }

    public void SetMovementState(PlayerMovementState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        Debug.Log("Player movement state: " + currentState);

        if (prototypeHud != null)
        {
            prototypeHud.SetMovementState(currentState.ToString());
        }
    }

    private void CheckForSitExitInput()
    {
        bool movementPressed =
            Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f
            || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f
            || Input.GetKeyDown(jumpKey);

        if (movementPressed)
        {
            ExitSitting();
        }
    }
}
