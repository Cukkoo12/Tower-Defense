using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpingPower = 16f;

    [Header("Wall")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(10f, 16f);
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingDuration = 0.25f;

    [Header("Check Noktalari")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private float horizontal;
    private bool isFacingRight = true;

    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;

    private string currentAnimState = "";
    private bool animLocked = false;

    public bool FacingRight => isFacingRight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        WallSlide();
        WallJump();

        if (!isWallJumping) Flip();

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    private bool IsWalled() => Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);

    private void WallSlide()
    {
        // DÜZELTME 1: Eðer karakter halihazýrda duvardan dýþarý sekiyorsa, 
        // tekrar duvara sürtünmesini ve zýplamayý iptal etmesini engelle.
        if (isWallJumping)
        {
            isWallSliding = false;
            return;
        }

        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            // DÜZELTME 2: Karakterin boyutu (Scale) deðiþirse zýplama gücünün bozulmamasý için 
            // sadece yönünü (1 veya -1) almak adýna Mathf.Sign kullandýk.
            wallJumpingDirection = -Mathf.Sign(transform.localScale.x);
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (Mathf.Sign(transform.localScale.x) != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() => isWallJumping = false;

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void UpdateAnimation()
    {
        if (animLocked || animator == null) return;

        string state;

        if (isWallSliding)
        {
            state = "WallSlide";
            // Sadece duvarda kayarken karakter görselini ters çeviriyoruz.
            // Bu sayede fiziksel yönü bozulmuyor, ama elleri duvara oturuyor.
            if (sr != null) sr.flipX = true;
        }
        else
        {
            // Duvardan koptuðu an görseli standart haline geri getiriyoruz.
            if (sr != null) sr.flipX = false;

            if (!IsGrounded()) state = rb.linearVelocity.y > 0.1f ? "Jump" : "Fall";
            else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f) state = "Run";
            else state = "Idle";
        }

        PlayState(state);
    }

    public void PlayState(string state)
    {
        if (state == currentAnimState) return;
        if (animator != null) animator.Play(state);
        currentAnimState = state;
    }

    public void LockAnimation(float duration)
    {
        animLocked = true;
        CancelInvoke(nameof(UnlockAnimation));
        Invoke(nameof(UnlockAnimation), duration);
    }

    private void UnlockAnimation() => animLocked = false;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }
}