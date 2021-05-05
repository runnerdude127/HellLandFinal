using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Animator anim;

    //public float speed = 6f;

    public float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;
    private Vector3 velocity;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;



    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Attack1());
        }else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartCoroutine(Attack2());
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Death();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Life();
        }
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (isGrounded)
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    Walk();
                }                
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    Run();
                }                                    
            }

            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            Idle();
        }
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Idle()
    {
        anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

    private void Death()
    {
        anim.SetTrigger("Death");
    }

    private void Life()
    {
        anim.SetTrigger("Life");
    }

    private void Walk()
    {
        anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
        moveSpeed = walkSpeed;
    }

    private void Run()
    {
        anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
        moveSpeed = runSpeed;
    }

    private void Jump()
    {
        anim.SetTrigger("Jump");
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private IEnumerator Attack1()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0.5f);
        anim.SetTrigger("Attack1");

        yield return new WaitForSeconds(2.0f);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0.5f);
    }
    private IEnumerator Attack2()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0.5f);
        anim.SetTrigger("Attack2");

        yield return new WaitForSeconds(2.0f);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0);
    }
}
