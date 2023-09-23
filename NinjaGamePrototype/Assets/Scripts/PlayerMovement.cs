using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float jump_power = 6f;
    private bool isFacingRight = true;
    private bool double_jump;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f; 

    private bool  isWallJumping;
    private float wall_jump_direction;
    private float wall_jump_time = 0.2f;
    private float wall_jump_counter;
    private float wall_jump_duration = 0.4f;
    private Vector2 wall_jump_power = new Vector2(5f, 10f);

    private bool canDash = true;
    private bool isDashing;
    private float dash_power = 15f;
    private float dash_time = 0.2f;
    private float dash_cooldown = 1f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform ground_check;
    [SerializeField] private LayerMask ground_layer;
    [SerializeField] private LayerMask wall_layer;
    [SerializeField] private Transform wall_check;

    // Start is called before the first frame update
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded() && !Input.GetButton("Jump"))
        {
            double_jump = false;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded() || double_jump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jump_power);

                double_jump = !double_jump;
            }
        }

        //hold to jump higher
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.1f);          
        }

        Wallslide();
        WallJump();
        
        if (!isWallJumping)
        {
            Flip();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isDashing)
        { return; }

        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(ground_check.position, 0.2f,ground_layer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wall_check.position, 0.2f, wall_layer);
    }

    private void Wallslide()
    {
        if (IsWalled() && !IsGrounded() && horizontal !=0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
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
            wall_jump_direction = -transform.localScale.x;
            wall_jump_counter = wall_jump_time;

            CancelInvoke(nameof(StopWallJumping));
        }
       else 
        {
            wall_jump_counter -= Time.deltaTime;
        }

       if (Input.GetButtonDown("Jump") && wall_jump_counter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wall_jump_direction * wall_jump_power.x, wall_jump_power.y);
            wall_jump_counter = 0f;

            if (transform.localScale.x != wall_jump_direction)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wall_jump_duration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f) 
        { 
            isFacingRight = !isFacingRight;
            Vector3 local_scale = transform.localScale;
            local_scale.x *= -1f;
            transform.localScale = local_scale; 
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float original_gravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dash_power, 0f);
        yield return new WaitForSeconds(dash_time);
        rb.gravityScale = original_gravity;
        isDashing = false;
        yield return new WaitForSeconds(dash_cooldown);
        canDash = true;
    }
}
