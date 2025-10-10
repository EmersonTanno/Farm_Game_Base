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
    public LayerMask bedCollision;
    private Vector2 inputDirection;
    private Vector2 facingDirection = Vector2.down;
    private bool justTurned = false;

    //Animation
    [SerializeField] private Animator myAnimator;

    //Conditions
    private bool isMoving = false;
    private bool isWatering = false;
    private bool isPlanting = false;
    private bool isPlowing = false;
    [HideInInspector] public bool isHarvesting = false;

    //Plants & Soil
    [SerializeField] GameObject plowedSoil;
    [SerializeField] private PlantType plantToPlant;
    public LayerMask soilCollision;
    #endregion

    #region Core
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
        if (CheckMove()) return;

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
        if (CheckAction()) return;

        Vector2 pos = transform.position;
        if (Physics2D.OverlapCircle(transform.position, .2f, bedCollision) || Physics2D.OverlapCircle(pos + GetSide(), .2f, bedCollision))
        {
            Time_Controll.Instance.ChangeDay();
        }
        else
        {
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
    }

    public void SetHarvest(InputAction.CallbackContext value)
    {
        if (!value.performed) return;
        if (CheckAction()) return;

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
        if (CheckMove()) return;

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    private void MovePointer()
    {
        if (CheckMove())
        {
            if (Vector3.Distance(transform.position, movePoint.position) >= .05f)
            {
                movePoint.position = transform.position;
                inputDirection = Vector2.zero;
            }
        }
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
                    if (Physics2D.OverlapCircle(targetPos, .2f, bedCollision))
                    {
                        Vector2 side = GetSide();
                        if (side == Vector2.down || side == Vector2.up)
                        {
                            movePoint.position = transform.position;
                            return;
                        }
                    }
                    if (Physics2D.OverlapCircle(transform.position, .2f, bedCollision))
                    {
                        Vector2 side = GetSide();
                        if (side == Vector2.down || side == Vector2.up)
                        {
                            movePoint.position = transform.position;
                            return;
                        }
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
        if (CheckAction()) return;

        StartCoroutine(Water());
    }

    private IEnumerator Water()
    {
        isWatering = true;
        myAnimator.SetBool("water", true);

        Vector2 waterPos = movePoint.position;

        Vector2 newWaterPos = GetSide();

        waterPos += newWaterPos;

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
        myAnimator.SetBool("water", false);
        isWatering = false;
    }

    private void PlowSoil()
    {
        if (CheckAction()) return;

        isPlowing = true;
        myAnimator.SetBool("plow", true);
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
        yield return new WaitForSeconds(.1f);
        isPlanting = false;
    }

    private void Harvest()
    {
        if (CheckAction()) return;

        Vector2 harvestPos = movePoint.position;

        Vector2 newHarvestPos = GetSide();

        harvestPos += newHarvestPos;

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
            soil.Harvest(myAnimator, this);
        }
    }
    #endregion

    #region Animation Functions
    public void PlowAnimation()
    {
        Vector2 spawnPos = movePoint.position;
        Vector2 newSpawnPos = GetSide();

        spawnPos += newSpawnPos;

        float tileSize = 1f;
        spawnPos = new Vector3(
            Mathf.Floor(spawnPos.x) + tileSize / 2f,
            Mathf.Floor(spawnPos.y) + tileSize / 2f,
            0f
        );

        Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.1f, soilCollision);
        if (hit != null)
        {
            Soil_Controller soil = hit.GetComponent<Soil_Controller>();
            soil.ResetSoil();

            myAnimator.SetBool("plow", false);
            isPlowing = false;
            return;
        }

        Instantiate(plowedSoil, spawnPos, Quaternion.identity);

        myAnimator.SetBool("plow", false);
        isPlowing = false;
    }
    #endregion

    #region Axiliar Functions
    private Vector2 GetSide()
    {
        if (facingDirection == Vector2.down)
        {
            return Vector2.down;
        }
        if (facingDirection == Vector2.up)
        {
            return Vector2.up;
        }
        if (facingDirection == Vector2.right)
        {
            return Vector2.right;
        }
        if (facingDirection == Vector2.left)
        {
            return Vector2.left;
        }

        return Vector2.zero;
    }

    private bool CheckAction()
    {
        if (isMoving || isWatering || isPlanting || isHarvesting || isPlowing || InventoryManager.Instance.inventoryActive)
        {
            return true;
        }

        return false;
    }

    private bool CheckMove()
    {
        if (isWatering || isPlanting || isHarvesting || isPlowing || InventoryManager.Instance.inventoryActive)
        {
            return true;
        }

        return false;
    }
    #endregion
}
