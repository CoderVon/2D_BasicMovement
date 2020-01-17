using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private Animator animator;

    public float speed, jumpForce;
    public Transform groundCheck;
    public LayerMask ground;


    public bool isGround, isJump,isDash;

    private float horizontalMove;

    bool jumpPressed;
    public int jumpCountAll=2;//总可跳跃次数
    int jumpCountLeft;//剩余可跳跃次数

    [Header("CD的UI组件")]
    public Image cdImage;

    [Header("冲刺参数")]
    public float dashTime;//冲锋时长
    private float dashTimeLeft;//冲锋剩余时间
    private float lastDashTime;//上次冲锋的时间
    public float dashCD;//冲锋CD;
    public float dashSpeed;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        lastDashTime = -1000;
    }

    private void Update()
    {
        //GetButtonDown()放在update里使得每一个机器都能在当前帧瞬间响应按键
        if (Input.GetButtonDown("Jump") && jumpCountLeft > 0)
        {
            jumpPressed = true;
        }
        SwitchAnimation();

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time >= (lastDashTime + dashCD))
            {
                //可以执行dash;
                ReadyToDash();
            }
        }

        cdImage.fillAmount -= 1.0f / dashCD * Time.deltaTime;//CD的UI的填充
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
        Dash();
        if (isDash)
            return;
        GroundMovement();
        Jump();
    }

    void GroundMovement()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        //移动
        rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y);

        //角色朝向
        if (horizontalMove > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);//旋转使用3维向量防止各种奇怪的问题出现
        }
        else if (horizontalMove < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);//旋转使用3维向量防止各种奇怪的问题出现
        }
    }

    void Jump()
    {
        if (isGround)
        {
            jumpCountLeft = jumpCountAll;
            isJump = false;
        }
        if (jumpPressed && isGround)//由于Jump()在FixedUPdate调用，而jumpPressed在update中调用，所以要引进jumpPressed来确保跳跃手感
        {
            isJump = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            jumpCountLeft--;
            jumpPressed = false;
        }
        else if (jumpPressed && jumpCountLeft > 0 && isJump)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            jumpCountLeft--;
            jumpPressed = false;
        }

    }

    void SwitchAnimation()
    {
        animator.SetFloat("running", Mathf.Abs(rigidbody.velocity.x));

        if (isGround)
        {
            animator.SetBool("isFall", false);
        }
        else if (!isGround && rigidbody.velocity.y > 0)
        {
            animator.SetBool("isJump", true);
        }
        else if (rigidbody.velocity.y < 0)
        {
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }
    }

    void ReadyToDash()
    {
        isDash = true;

        dashTimeLeft = dashTime;

        lastDashTime = Time.time;

        cdImage.fillAmount = 1;//CD的UI重置为填充满
    }
    void Dash()
    {
        if (isDash)
        {
            if (dashTimeLeft > 0)
            {
                if (rigidbody.velocity.y > 0 && !isGround)//当冲刺时处于空中，则冲刺过程加上Y轴的跳跃力用来抵消重力影响
                {
                    rigidbody.velocity = new Vector2(dashSpeed * horizontalMove,jumpForce);
                }
                rigidbody.velocity = new Vector2(dashSpeed * horizontalMove, rigidbody.velocity.y);

                dashTimeLeft -= Time.deltaTime;

                ShadowPool.instance.GetFromPool();
            }
            if (dashTimeLeft <= 0)
            {
                isDash = false;
                if (!isGround)
                {
                    //当冲刺结束时还在空中则施加一个小跳
                    rigidbody.velocity = new Vector2(dashSpeed * horizontalMove, jumpForce);
                }
            }
        }
    }
}
