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
    // Start is called once before the first execution of UpdateCkeckpointFields after the MonoBehaviour is created
    void Start()
    {
        click=InputSystem.actions.FindAction("Click");
        click.performed += attemptedInteraction;
        point=InputSystem.actions.FindAction("Point");
    }

    private void attemptedInteraction(InputAction.CallbackContext obj)
    {
        if(spriteDialogue && dataFlow.canvasCtrl.UIMode != EUIMode.DIALOGUE)
        {
            DialogueOptionData dod = spriteDialogue.getDOD;
            if (dod is not null) {
                dataFlow.StartDialogueSequence(spriteDialogue.getDOD);
                spriteDialogue.read = true;
            }
            else
            {
                Debug.Log("Sprite object returned null instead of DOD.");
            }
        }
    }

    // UpdateCkeckpointFields is called once per frame
    void Update()
    {
        mousePosition = point.ReadValue<Vector2>();
        mouseray = Camera.main.ScreenPointToRay(mousePosition);
        raycastHit2D = Physics2D.Raycast(mouseray.origin, mouseray.direction);
        spriteDialogue = raycastHit2D ? raycastHit2D.collider.GetComponent<ObjectWithDialogueInteraction>() : null;
        switch (spriteDialogue)
        {
            case null:
                {
                    SetCursor(ECursorMode.DEFAULT);
                    break;
                }
            default:
                {
                    SetCursor(spriteDialogue.read? ECursorMode.READINTERACTION : ECursorMode.NEWINTERACTION);
                    break;
                }
        }
    }

    void SetCursor(ECursorMode mode)
    {
        if (mode != cursorMode)
        {
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
