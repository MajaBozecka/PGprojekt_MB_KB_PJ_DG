using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CursorController : MonoBehaviour
{
    public Vector2 mousePosition;
    public Ray mouseray;
    public RaycastHit2D raycastHit2D;
    public SpriteDialogue spriteDialogue;
    [SerializeField]
    private InputAction click;
    [SerializeField]
    private InputAction point;
    [SerializeField]
    private DataFlowController dataFlow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        click=InputSystem.actions.FindAction("Click");
        click.performed += attemptedInteraction;
        point=InputSystem.actions.FindAction("Point");
    }

    private void attemptedInteraction(InputAction.CallbackContext obj)
    {
        raycastHit2D = Physics2D.Raycast(mouseray.origin, mouseray.direction);
        spriteDialogue = raycastHit2D ? raycastHit2D.collider.GetComponent<SpriteDialogue>() : null;
        if(spriteDialogue)
        {
            dataFlow.OnButtonTest(spriteDialogue.dialogueSequenceId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = point.ReadValue<Vector2>();
        mouseray = Camera.main.ScreenPointToRay(mousePosition);
    }
}
