using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    public Vector2 mousePosition;
    public Ray mouseray;
    public RaycastHit2D raycastHit2D;
    public ObjectWithDialogueInteraction spriteDialogue;
    [SerializeField]
    private InputAction click;
    [SerializeField]
    private InputAction point;
    [SerializeField]
    private DataFlowController dataFlow;
    [SerializeField]
    private Texture2D defaultCursor;
    [SerializeField]
    private Vector2 defaultHotSpot;
    [SerializeField]
    private Texture2D NewItemCursor;
    [SerializeField]
    private Vector2 NewHotSpot;
    [SerializeField]
    private Texture2D readItemCursor;
    [SerializeField]
    private Vector2 readHotSpot;
    public ECursorMode cursorMode;

    private float lastInteractionTime = 0f;
    private float interactionCooldown = 0.3f; 
    void Start()
    {
        click=InputSystem.actions.FindAction("Click");
        click.performed += attemptedInteraction;
        point=InputSystem.actions.FindAction("Point");
    }

    private void attemptedInteraction(InputAction.CallbackContext obj)
    {
        if (!obj.performed) return;

        if (Time.time - lastInteractionTime < interactionCooldown) return;

        if (dataFlow.canvasCtrl.UIMode != EUIMode.NOTHING) return;

        if (raycastHit2D)
        {
            Debug.Log("Zarejestrowano klikniêcie w obiekt o nazwie: " + raycastHit2D.collider.gameObject.name);
            dataFlow.PlayClickSound();
        }

     
        LocationChanger clickedDoor = raycastHit2D ? raycastHit2D.collider.GetComponent<LocationChanger>() : null;
        if (clickedDoor != null)
        {
            lastInteractionTime = Time.time;

            FindFirstObjectByType<LocationManager>().ChangeLocation(clickedDoor.targetLocationId);
            return;
        }

        CollectibleItem clickedCollectible = raycastHit2D ? raycastHit2D.collider.GetComponent<CollectibleItem>() : null;
        if (clickedCollectible != null)
        {
            lastInteractionTime = Time.time;

            clickedCollectible.Collect();
            return; 
        }

        // --- SPRAWDZANIE DIALOGÓW ---
        if (spriteDialogue)
        {
            DialogueOptionData dod = spriteDialogue.getDOD;
            if (dod is not null)
            {
                lastInteractionTime = Time.time;
                dataFlow.testedObjectWithDialogueInteraction = spriteDialogue;

                dataFlow.StartDialogueSequence(dod);
                spriteDialogue.read = true;
            }
            else
            {
                Debug.Log("Sprite object returned null instead of DOD.");
            }
        }
    }

    void Update()
    {
        if (dataFlow.canvasCtrl.UIMode != EUIMode.NOTHING)
        {
            SetCursor(ECursorMode.DEFAULT);
            return;
        }
        mousePosition = point.ReadValue<Vector2>();
        mouseray = Camera.main.ScreenPointToRay(mousePosition);
        raycastHit2D = Physics2D.Raycast(mouseray.origin, mouseray.direction);


        spriteDialogue = raycastHit2D ? raycastHit2D.collider.GetComponent<ObjectWithDialogueInteraction>() : null;

        LocationChanger hoveredDoor = raycastHit2D ? raycastHit2D.collider.GetComponent<LocationChanger>() : null;

        if (hoveredDoor != null)
        {
            SetCursor(ECursorMode.NEWINTERACTION); 
        }
        else if (spriteDialogue != null)
        {
            SetCursor(spriteDialogue.read ? ECursorMode.READINTERACTION : ECursorMode.NEWINTERACTION);
        }
        else
        {
            SetCursor(ECursorMode.DEFAULT);
        }
    }

    void SetCursor(ECursorMode mode)
    {
        if (mode != cursorMode)
        {
            if (cursorMode == ECursorMode.DEFAULT && (mode == ECursorMode.NEWINTERACTION || mode == ECursorMode.READINTERACTION))
            {
                dataFlow.PlayHoverSound();
            }

            cursorMode = mode;
            switch(mode)
            {
                case ECursorMode.NEWINTERACTION:
                    {
                        Cursor.SetCursor(NewItemCursor, NewHotSpot, CursorMode.Auto);
                        break;
                    }
                case ECursorMode.READINTERACTION:
                    {
                        Cursor.SetCursor(readItemCursor, readHotSpot, CursorMode.Auto);
                        break;
                    }
                default:
                    {
                        Cursor.SetCursor(defaultCursor, defaultHotSpot, CursorMode.Auto);
                        break;
                    }
            }
        }
    }
    public enum ECursorMode:byte
    {
        DEFAULT,
        NEWINTERACTION,
        READINTERACTION
    }
}
