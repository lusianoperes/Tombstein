using UnityEngine;

public class RestrictMouseMovement : MonoBehaviour
{
    private void Start()
    {
    
        Cursor.lockState = CursorLockMode.Confined;
        
    }
}