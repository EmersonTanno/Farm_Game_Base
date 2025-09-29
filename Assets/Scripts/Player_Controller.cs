using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region Variables
    [SerializeField] float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask collision;
    private Animator myAnimator;
    private Vector2 inputDirection;
    private bool isMoving = false;
    private bool isWatering = false;
    #endregion

    #region Core
    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        movePoint.parent = null;
    }

    void Update()
    {
        MovePlayer();

        MovePointer();

        IdleAnimation();
    }
    #endregion

    #region InputSystem
    public void SetMove(InputAction.CallbackContext value)
    {
        inputDirection = value.ReadValue<Vector2>();

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            inputDirection = new Vector2(Mathf.Sign(inputDirection.x), 0);
        }
        else if (Mathf.Abs(inputDirection.y) > Mathf.Abs(inputDirection.x))
        {
            inputDirection = new Vector2(0, Mathf.Sign(inputDirection.y));
        }
        else
        {
            inputDirection = Vector2.zero;
        }
    }

    public void SetWater(InputAction.CallbackContext value)
    {
        if (isMoving) return;

        AnimatorStateInfo stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Player_Idle_Front"))
        {
            StartCoroutine(Water("water_front"));
        }
        else if (stateInfo.IsName("Player_Idle_Back"))
        {
            StartCoroutine(Water("water_back"));
        }
        else if (stateInfo.IsName("Player_Idle_Left"))
        {
            StartCoroutine(Water("water_left"));
        }
        else if (stateInfo.IsName("Player_Idle_Right"))
        {
           StartCoroutine(Water("water_right"));
        }
        else
        {
            return;
        }
    }
    #endregion

    #region Animation
    private void ActivateAnimation(string animation)
    {
        myAnimator.SetBool("walk_front", false);
        myAnimator.SetBool("walk_back", false);
        myAnimator.SetBool("walk_left", false);
        myAnimator.SetBool("walk_right", false);

        myAnimator.SetBool(animation, true);
    }

    private void IdleAnimation()
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= .0f)
        {
            myAnimator.SetBool("walk_front", false);
            myAnimator.SetBool("walk_back", false);
            myAnimator.SetBool("walk_left", false);
            myAnimator.SetBool("walk_right", false);

            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }
    #endregion

    #region Movement
    private void MovePlayer()
    {
        if (isWatering) return;

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    private void MovePointer()
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            if (inputDirection != Vector2.zero)
            {
                Vector3 targetPos = movePoint.position + new Vector3(inputDirection.x, inputDirection.y, 0f);

                if (!Physics2D.OverlapCircle(targetPos, .2f, collision))
                {
                    movePoint.position = targetPos;

                    if (inputDirection.x == 1) ActivateAnimation("walk_right");
                    if (inputDirection.x == -1) ActivateAnimation("walk_left");
                    if (inputDirection.y == 1) ActivateAnimation("walk_back");
                    if (inputDirection.y == -1) ActivateAnimation("walk_front");
                }
            }
        }
    }
    #endregion

    #region Actions
    private IEnumerator Water(string animation)
    {
        isWatering = true;
        myAnimator.SetBool(animation, true);

        yield return new WaitForSeconds(1f);
        myAnimator.SetBool(animation, false);
        isWatering = false;
    }
    #endregion
}
