using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region Variables
    //Movement
    [SerializeField] float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask collision;
    private Vector2 inputDirection;
    private Vector2 facingDirection = Vector2.down;
    private bool justTurned = false;

    //Animation
    private Animator myAnimator;

    //Conditions
    private bool isMoving = false;
    private bool isWatering = false;
    private bool isPlanting = false;

    //Plants & Soil
    [SerializeField] GameObject plowedSoil;
    [SerializeField] private PlantType plantToPlant;
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
        if (isWatering || isPlanting) return;

        if (value.performed)
        {
            inputDirection = value.ReadValue<Vector2>();

            if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
                inputDirection = new Vector2(Mathf.Sign(inputDirection.x), 0);
            else if (Mathf.Abs(inputDirection.y) > Mathf.Abs(inputDirection.x))
                inputDirection = new Vector2(0, Mathf.Sign(inputDirection.y));
            else
                inputDirection = Vector2.zero;
        }
        else if (value.canceled)
        {
            inputDirection = Vector2.zero;
        }
    }


    public void SetAction(InputAction.CallbackContext value)
    {
        if (!value.performed) return;
        if (isMoving || isWatering || isPlanting) return;

        Item receivedItem = InventoryManager.Instance.UseSelectedItem();

        if (receivedItem == null)
        {
            return;
        }

        if (receivedItem.type == ItemType.Seed)
        {
            StartCoroutine(Plant(receivedItem.plant));
            return;
        }
        else if (receivedItem.type == ItemType.Tool)
        {
            if (receivedItem.action == ActionType.Plowing)
            {
                PlowSoil();
                return;
            }
            else if (receivedItem.action == ActionType.Water)
            {
                PutWater();
                return;
            }
        }
    }

    public void SetHarvest(InputAction.CallbackContext value)
    {
        if (!value.performed) return;
        if (isMoving || isWatering || isPlanting) return;

        Harvest();
    }
    #endregion

    #region Animation
    private void ActivateAnimation(string animation)
    {
        myAnimator.SetBool("walk_front", false);
        myAnimator.SetBool("walk_back", false);
        myAnimator.SetBool("walk_left", false);
        myAnimator.SetBool("walk_right", false);
        myAnimator.SetBool("idle_f", false);
        myAnimator.SetBool("idle_b", false);
        myAnimator.SetBool("idle_r", false);
        myAnimator.SetBool("idle_l", false);

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
        if (isWatering || isPlanting) return;

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    private void MovePointer()
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            if (inputDirection != Vector2.zero)
            {
                if (inputDirection == facingDirection && !justTurned)
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
                else
                {
                    facingDirection = inputDirection;
                    justTurned = true;

                    if (inputDirection.x == 1) ActivateAnimation("idle_r");
                    if (inputDirection.x == -1) ActivateAnimation("idle_l");
                    if (inputDirection.y == 1) ActivateAnimation("idle_b");
                    if (inputDirection.y == -1) ActivateAnimation("idle_f");

                    if (justTurned == true)
                        StartCoroutine(ResetTurned());
                }
            }
        }
    }

    private IEnumerator ResetTurned()
    {
        yield return new WaitForSeconds(0.2f);
        justTurned = false;
    }

    #endregion

    #region Actions

    private void PutWater()
    {
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
            }
        }

        yield return new WaitForSeconds(1f);
        myAnimator.SetBool(animation, false);
        isWatering = false;
    }

    private void PlowSoil()
    {
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
            return;
        }

        Instantiate(plowedSoil, spawnPos, Quaternion.identity);
    }

    private IEnumerator Plant(PlantType plant)
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

        yield return new WaitForSeconds(.5f);

        foreach (Vector3 offset in offsets)
        {
            Vector3 plantPos = transform.position + offset;

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
                    soil.PlantSeed(plant);
                }
            }
        }

        yield return new WaitForSeconds(.5f);
        myAnimator.SetBool("planting", false);
        isPlanting = false;
    }
    
    private void Harvest()
    {
        if (isMoving || isWatering || isPlanting) return;

        AnimatorStateInfo stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);

        Vector3 harvestPos = movePoint.position;

        if (stateInfo.IsName("Player_Idle_Front"))
        {
            harvestPos += Vector3.down;
        }
        else if (stateInfo.IsName("Player_Idle_Back"))
        {
            harvestPos += Vector3.up;
        }
        else if (stateInfo.IsName("Player_Idle_Left"))
        {
            harvestPos += Vector3.left;
        }
        else if (stateInfo.IsName("Player_Idle_Right"))
        {
            harvestPos += Vector3.right;
        }

        float tileSize = 1f;
        harvestPos = new Vector3(
            Mathf.Floor(harvestPos.x) + tileSize / 2f,
            Mathf.Floor(harvestPos.y) + tileSize / 2f,
            0f
        );

        Collider2D hit = Physics2D.OverlapCircle(harvestPos, 0.1f, soilCollision);
        if (hit != null)
        {
            Soil_Controller soil = hit.GetComponent<Soil_Controller>();
            if (!soil.currentPlant)
            {
                return;
            }
            soil.Harvest();
        }
    }
    #endregion
}
