using UnityEngine;

public class DragToRotate : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1)] private float manualRotationMultiplier = 0.25f;
    [SerializeField, Range(10, 30)] private float autoRotationMultiplier = 10;
    
    private bool manualRotation = false;
    
    private Vector3 positionLastFrame;
    private Vector3 positionCurrentFrame;

    private Camera mainCamera;
    
    private void Start()
    {
        positionLastFrame = Vector3.zero;
        positionCurrentFrame = Vector3.zero;

        manualRotation = false;
        
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            manualRotation = true;

        if (Input.GetKeyUp(KeyCode.Mouse0))
            manualRotation = false;

        if (manualRotation)
        {
            positionCurrentFrame =  mainCamera.WorldToScreenPoint(Input.mousePosition) - positionLastFrame;
            transform.Rotate(transform.up, -Vector3.Dot(positionCurrentFrame, mainCamera.transform.right) * manualRotationMultiplier, Space.World);
        }
        else
        {
            transform.Rotate(Vector3.up * (autoRotationMultiplier * -Time.deltaTime), Space.World);
        }
        
        positionLastFrame = mainCamera.WorldToScreenPoint(Input.mousePosition);
    }
    
}