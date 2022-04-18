using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalActions : MonoBehaviour
{
    private bool is_join = false;

    public void onJoin(InputAction.CallbackContext context)
    {
        is_join = context.ReadValueAsButton();
        is_join = context.action.triggered;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
