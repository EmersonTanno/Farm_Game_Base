using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    #region Variables
    [SerializeField] float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask collision;
    private Animator myAnimator;
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
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        MovePointer();

        IdleAnimation();
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
        }
    }
    #endregion

    #region Movement
    private void MovePointer()
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, collision))
                {
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    if (Input.GetAxisRaw("Horizontal") == 1)
                    {
                        ActivateAnimation("walk_right");
                    }
                    if (Input.GetAxisRaw("Horizontal") == -1)
                    {
                        ActivateAnimation("walk_left");
                    }
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, collision))
                {
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    if (Input.GetAxisRaw("Vertical") == 1)
                    {
                        ActivateAnimation("walk_back");
                    }
                    if (Input.GetAxisRaw("Vertical") == -1)
                    {
                        ActivateAnimation("walk_front");
                    }
                }
            }


        }
    }
    #endregion
}
