using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;
    public float slideSpeed = 10f;
    public float jumpForce = 6f;
    public float gravity = -20f;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;

    [Header("Slide Settings")]
    public float slideDuration = 0.8f;
    private float slideTimer;

    [Header("Wall Jump Settings")]
    public LayerMask wallMask;
    public float wallJumpForce = 8f;
    public float wallJumpDirectionForce = 4f;
    public float wallJumpDuration = 1.2f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;

    [Header("Camera Settings")]
    public Transform orientation;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashCooldownTimer;
    private bool isDashing;
    private Vector3 dashDirection;
    private float dashTimer;

    [Header("Dash VFX")]
    public GameObject dashParticlesPrefab;
    public Transform dashEffectSpawnPoint;

    private CharacterController controller;
    private Animator animator;

    private Vector3 moveDirection;
    private Vector3 velocity;
    private Vector3 slideDirection;
    private Vector3 wallJumpDirection;
    private float wallJumpTimer;

    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private bool isSliding;
    private bool isDead = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()

    {
        if (isDead)
        {
            HandleGravity(); 
            return;
        }

        GroundCheck();
        HandleInput();
        HandleMovement();
        HandleGravity();
        HandleAnimations();
        UpdateWallJumpTimer();
        UpdateDash();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void HandleInput()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(inputX, 0, inputZ).normalized;
        moveDirection = orientation.forward * input.z + orientation.right * input.x;

        // Sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift) && inputZ > 0 && isGrounded && !isCrouching && !isSliding;

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
                Jump();
            else
                TryWallJump();
        }

        // Crouch / Slide
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isSprinting && isGrounded)
                StartSlide();
            else
                StartCrouch();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (isSliding) StopSlide();
            else StopCrouch();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.E) && dashCooldownTimer <= 0 && !isDashing)
        {
            StartDash();
        }
    }

    private void HandleMovement()
    {
        if (isDashing)
        {
            controller.Move(dashDirection * dashForce * Time.deltaTime);
            return;
        }

        Vector3 finalMove = moveDirection;

        if (wallJumpTimer > 0)
            finalMove += wallJumpDirection;

        if (isSliding)
        {
            finalMove = slideDirection;
            controller.Move(finalMove.normalized * slideSpeed * Time.deltaTime);
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0) StopSlide();
            return;
        }

        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isSprinting) currentSpeed = sprintSpeed;

        controller.Move(finalMove.normalized * currentSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (!isDashing)
        {
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        animator.SetTrigger("Jump");
    }

    private void TryWallJump()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, orientation.right, out hit, 1.5f, wallMask))
            WallJump(-orientation.right, true);
        else if (Physics.Raycast(transform.position, -orientation.right, out hit, 1.5f, wallMask))
            WallJump(orientation.right, false);
    }

    private void WallJump(Vector3 wallNormal, bool mirror)
    {
        velocity.y = Mathf.Sqrt(wallJumpForce * -2f * gravity);
        wallJumpDirection = wallNormal * wallJumpDirectionForce;
        wallJumpTimer = wallJumpDuration;
        animator.SetBool("WallJumpMirror", mirror);
        animator.SetTrigger("WallJump");
    }

    private void UpdateWallJumpTimer()
    {
        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                wallJumpDirection = Vector3.zero;
                animator.SetBool("WallJumpMirror", false);
            }
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
        controller.height = crouchHeight;
        controller.center = new Vector3(0, crouchHeight / 2f, 0);
        animator.SetBool("IsCrouching", true);
    }

    private void StopCrouch()
    {
        isCrouching = false;
        controller.height = standingHeight;
        controller.center = new Vector3(0, standingHeight / 2f, 0);
        animator.SetBool("IsCrouching", false);
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        controller.height = crouchHeight;
        controller.center = new Vector3(0, crouchHeight / 2f, 0);
        animator.SetTrigger("Slide");
        slideDirection = moveDirection.magnitude > 0.1f ? moveDirection : orientation.forward;
    }

    private void StopSlide()
    {
        isSliding = false;
        controller.height = standingHeight;
        controller.center = new Vector3(0, standingHeight / 2f, 0);
        animator.ResetTrigger("Slide");
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = orientation.forward;

        animator.SetTrigger("Dash");

        // Instanciar partículas del dash
        if (dashParticlesPrefab != null)
        {
            Vector3 spawnPos = dashEffectSpawnPoint != null ? dashEffectSpawnPoint.position : transform.position;
            Instantiate(dashParticlesPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void UpdateDash()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
                isDashing = false;
        }
    }
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Death");
       
    }

    private void HandleAnimations()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        animator.SetFloat("MoveX", inputX, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveZ", inputZ, 0.1f, Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
