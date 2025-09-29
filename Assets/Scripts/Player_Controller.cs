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
    private bool isPlanting = false;
    [SerializeField] GameObject plowedSoil;
    public LayerMask soilCollision;
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
        if (!value.performed) return;
        if (isMoving || isPlanting) return;

        AnimatorStateInfo stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Player_Idle_Front"))
        {
            StartCoroutine(Water("water_front", "front"));
        }
        else if (stateInfo.IsName("Player_Idle_Back"))
        {
            StartCoroutine(Water("water_back", "back"));
        }
        else if (stateInfo.IsName("Player_Idle_Left"))
        {
            StartCoroutine(Water("water_left", "left"));
        }
        else if (stateInfo.IsName("Player_Idle_Right"))
        {
            StartCoroutine(Water("water_right", "right"));
        }
    }


    public void SetPlant(InputAction.CallbackContext value)
    {
        if (!value.performed) return;

        if (isMoving || isWatering || isPlanting) return;

        AnimatorStateInfo stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);

        Vector3 spawnPos = movePoint.position;

        if (stateInfo.IsName("Player_Idle_Front"))
        {
            spawnPos += Vector3.down;
        }
        else if (stateInfo.IsName("Player_Idle_Back"))
        {
            spawnPos += Vector3.up;
        }
        else if (stateInfo.IsName("Player_Idle_Left"))
        {
            spawnPos += Vector3.left;
        }
        else if (stateInfo.IsName("Player_Idle_Right"))
        {
            spawnPos += Vector3.right;
        }

        float tileSize = 1f;
        spawnPos = new Vector3(
            Mathf.Floor(spawnPos.x) + tileSize / 2f,
            Mathf.Floor(spawnPos.y) + tileSize / 2f,
            0f
        );

        Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.1f, soilCollision);
        if (hit != null)
        {
            Debug.Log("Já existe algo nesse tile, não pode plantar!");
            return;
        }

        Instantiate(plowedSoil, spawnPos, Quaternion.identity);
    }

    public void SetSeed(InputAction.CallbackContext value)
    {
        if (isMoving || isWatering || isPlanting) return;

        AnimatorStateInfo stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Player_Idle_Front"))
        {
            StartCoroutine(Plant("front"));
        }
        else if (stateInfo.IsName("Player_Idle_Back"))
        {
            StartCoroutine(Plant("back"));
        }
        else if (stateInfo.IsName("Player_Idle_Left"))
        {
            StartCoroutine(Plant("left"));
        }
        else if (stateInfo.IsName("Player_Idle_Right"))
        {
            StartCoroutine(Plant("right"));
        }

        //StartCoroutine(Plant());
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
    private IEnumerator Water(string animation, string direction)
    {
        isWatering = true;
        myAnimator.SetBool(animation, true);

        Vector3 waterPos = movePoint.position;

        if (direction == "front")
        {
            waterPos += Vector3.down;
        }
        else if (direction == "back")
        {
            waterPos += Vector3.up;
        }
        else if (direction == "left")
        {
            waterPos += Vector3.left;
        }
        else if (direction == "right")
        {
            waterPos += Vector3.right;
        }

        float tileSize = 1f;
        waterPos = new Vector3(
            Mathf.Floor(waterPos.x) + tileSize / 2f,
            Mathf.Floor(waterPos.y) + tileSize / 2f,
            0f
        );

        Collider2D hit = Physics2D.OverlapCircle(waterPos, 0.1f, soilCollision);
        if (hit != null)
        {
            Soil_Controller soil = hit.GetComponent<Soil_Controller>();
            if (soil != null)
            {
                soil.SetWater(true);
                Debug.Log("Solo arado encontrado → Regado!");
            }
        }

        yield return new WaitForSeconds(1f);
        myAnimator.SetBool(animation, false);
        isWatering = false;
    }

    private IEnumerator Plant(string direction)
    {
        isPlanting = true;
        myAnimator.SetBool("planting", true);

        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            Vector3.down,
            Vector3.up,
            Vector3.left,
            Vector3.right
        };

        float tileSize = 1f;

        foreach (Vector3 offset in offsets)
        {
            Vector3 plantPos = movePoint.position + offset;

            plantPos = new Vector3(
                Mathf.Floor(plantPos.x) + tileSize / 2f,
                Mathf.Floor(plantPos.y) + tileSize / 2f,
                0f
            );

            Collider2D hit = Physics2D.OverlapCircle(plantPos, 0.1f, soilCollision);
            if (hit != null)
            {
                Soil_Controller soil = hit.GetComponent<Soil_Controller>();
                if (soil != null)
                {
                    soil.SetPlanted(true);
                    Debug.Log($"Solo arado encontrado → Plantado em {plantPos}!");
                }
            }
        }

        yield return new WaitForSeconds(1f);
        isPlanting = false;
        myAnimator.SetBool("planting", false);
    }
    #endregion
}
