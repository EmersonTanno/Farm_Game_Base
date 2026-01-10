using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region Variables

    public static Player_Controller Instance { get; private set; }

    //Movement
    [SerializeField] float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask collision;
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

    //Reactions
    [SerializeField]private ThoughtBubbleController reactions;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
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

    void LateUpdate()
    {
        CheckWarp();
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

        WorldObjectID obj1 = TileMapController.Instance.GetGrid().GetObjectGrid().GetGridObject(pos);
        WorldObjectID obj2 = TileMapController.Instance.GetGrid().GetObjectGrid().GetGridObject(pos + GetSide());
        int nPCId = CheckNPC(pos);

        if(nPCId != 0)
        {
            NPCController.Instance.InteractWithNPC(nPCId);
            return;
        }

        if (obj1 == WorldObjectID.Bed || obj2 == WorldObjectID.Bed)
        {
            Time_Controll.Instance.ActivateBedCanvas();
            return;
        }

        if (obj2 == WorldObjectID.ShippingBox)
        {
            Sell_Box_Controller.Instance.AddItem(InventoryManager.Instance.SellSelectedItem());
            return;
        }

        Item item = InventoryManager.Instance.UseSelectedItem();
        if (item == null) return;

        HandleItem(item);
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

            myAnimator.SetBool("idle_f", false);
            myAnimator.SetBool("idle_b", false);
            myAnimator.SetBool("idle_r", false);
            myAnimator.SetBool("idle_l", false);

            Vector2 side = GetSide();
            if (side == Vector2.down)
                myAnimator.SetBool("idle_f", true);
            if (side == Vector2.up)
                myAnimator.SetBool("idle_b", true);
            if (side == Vector2.right)
                myAnimator.SetBool("idle_r", true);
            if (side == Vector2.left)
                myAnimator.SetBool("idle_l", true);
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

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.01f)
        {
            transform.position = movePoint.position;
            return;
        }

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

                    if (!Physics2D.OverlapCircle(targetPos, .2f, collision) && CheckPlayerMoveGrid(targetPos))
                    {
                        movePoint.position = targetPos;

                        if (inputDirection.x == 1) ActivateAnimation("walk_right");
                        if (inputDirection.x == -1) ActivateAnimation("walk_left");
                        if (inputDirection.y == 1) ActivateAnimation("walk_back");
                        if (inputDirection.y == -1) ActivateAnimation("walk_front");
                    }
                    else
                    {
                        return;
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

    private void CheckWarp()
    {
        if (Vector3.Distance(transform.position, movePoint.position) > 0.01f)
            return;

        
        WarpTile warp = TileMapController.Instance.GetGrid().GetWarpGrid().GetGridObject(transform.position);
        if(warp != null)
        {
            WarpController.Instance.ExecuteWarp(warp);
        }
    }

    #endregion

    #region Action Handler
    private void HandleItem(Item item)
    {
        switch (item.type)
        {
            case ItemType.Seed:
                StartPlant(item.plant);
                break;

            case ItemType.Tool:
                HandleTool(item);
                break;

            default:
                break;
        }
    }

    private void HandleTool(Item item)
    {
        switch (item.action)
        {
            case ActionType.Plowing:
                PlowSoil();
                break;

            case ActionType.Water:
                PutWater();
                break;

            default:
                break;
        }
    }
    #endregion

    #region Actions
    private void PutWater()
    {
        if (CheckAction()) return;
        if (!Status_Controller.Instance.UseEnergy(2)) return;

        StartCoroutine(Water());
    }

    private IEnumerator Water()
    {
        isWatering = true;
        myAnimator.SetBool("water", true);

        TileMapController.Instance.WaterSoil(new Vector2(transform.position.x, transform.position.y) + GetSide());

        yield return new WaitForSeconds(1f);
        myAnimator.SetBool("water", false);
        isWatering = false;
    }

    private void PlowSoil()
    {
        if (CheckAction()) return;
        if (!Status_Controller.Instance.UseEnergy(5)) return;

        isPlowing = true;
        myAnimator.SetBool("plow", true);
    }

    private void StartPlant(PlantType plant)
    {
        if (CheckAction()) return;

        StartCoroutine(Plant(plant));
    }

    private IEnumerator Plant(PlantType plant)
    {
        isPlanting = true;
        myAnimator.SetBool("planting", true);

        Vector2[] offsets = new Vector2[]
        {
            Vector2.zero,
            Vector2.down,
            Vector2.up,
            Vector2.left,
            Vector2.right,
            new Vector2(1, 1),
            new Vector2(-1, -1),
            new Vector2(-1, 1),
            new Vector2(1, -1),
        };

        yield return new WaitForSeconds(.5f);

        foreach (Vector2 offset in offsets)
        {
            TileMapController.Instance.PlantSoil(new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), plant);
        }

        yield return new WaitForSeconds(.5f);
        myAnimator.SetBool("planting", false);
        yield return new WaitForSeconds(.1f);
        isPlanting = false;
    }

    private void Harvest()
    {
        if (CheckAction() || !TileMapController.Instance.CanHarvest(new Vector2(transform.position.x, transform.position.y) + GetSide())) return;

        StartCoroutine(HarvestConclusion());
    }
    #endregion

    #region Animation Functions
    public void PlowAnimation()
    {
        myAnimator.SetBool("plow", false);
        isPlowing = false;

        TileMapController.Instance.PlowSoil(new Vector2(transform.position.x, transform.position.y) + GetSide());
    }


    public IEnumerator HarvestConclusion()
    {
        isHarvesting = true;
        myAnimator.SetBool("harvest", true);
        yield return new WaitForSeconds(0.75f);

        TileMapController.Instance.Harvest(new Vector2(transform.position.x, transform.position.y) + GetSide());

        myAnimator.SetBool("harvest", false);
        isHarvesting = false;
    }
    #endregion

    #region Auxiliar Functions
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
        if (isMoving || isWatering || isPlanting || isHarvesting || isPlowing || InventoryManager.Instance.inventoryActive || Time_Controll.Instance.bedActive || Shop_Manager.Instance.shopActive || Sell_Controller.Instance.active || DialogueManager.Instance.dialogueActive)
        {
            return true;
        }

        return false;
    }

    private bool CheckMove()
    {
        if (isWatering || isPlanting || isHarvesting || isPlowing || InventoryManager.Instance.inventoryActive || Time_Controll.Instance.bedActive || Shop_Manager.Instance.shopActive || Sell_Controller.Instance.active)
        {
            return true;
        }

        return false;
    }

    public bool CheckPlayerActions()
    {
        if (isMoving || isWatering || isPlanting || isHarvesting || isPlowing)
        {
            return true;
        }

        return false;
    }

    private bool CheckPlayerMoveGrid(Vector2 pos)
    {
        if(TileMapController.Instance.GetGrid().GetMovementGrid().GetGridObject(pos) == false)
        {
            return false;
        }

        WorldObjectID objectGridValue = TileMapController.Instance.GetGrid().GetObjectGrid().GetGridObject(pos);

        if(objectGridValue == WorldObjectID.Bed)
        {
            Vector2 side = GetSide();
            if (side == Vector2.down || side == Vector2.up)
            {
                return false;
            }
        }

        if(TileMapController.Instance.GetGrid().GetObjectGrid().GetGridObject(transform.position) == WorldObjectID.Bed)
        {
            Vector2 side = GetSide();
            if (side == Vector2.down || side == Vector2.up)
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    #region Reactions
    public void ShowReaction(ThoughtEmoteEnum reaction)
    {
        reactions.ShowBalloon(reaction);
    }
    #endregion

    #region NPC Interaction
    private int CheckNPC(Vector2 currentPosition)
    {
        Vector2 side = GetSide();
        Grid<int> npcGrid = TileMapController.Instance.GetGrid().GetNpcGrid();

        int firstTile = npcGrid.GetGridObject(currentPosition + side);

        if (firstTile != 0)
            return firstTile;

        int secondTile = npcGrid.GetGridObject(currentPosition + (side * 2));

        if (secondTile != 0)
            return secondTile;

        return 0;
    }
    #endregion
}
