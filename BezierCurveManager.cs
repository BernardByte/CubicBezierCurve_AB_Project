using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveManager : MonoBehaviour
{
    public GameObject pointPrefab; // Prefab for the points
    public float sphereRadius = 0.1f; // Radius for the spheres
    public int resolution = 20; // Resolution of the curve

    //public Vector3[] controlPoints; // Array to hold control points

    private CubicBezierCurve cubicBezierCurve;
    
    // mouse position holder
    public Vector3 currentMousePosition;
    public Vector3 PreviousMousePosition;
    // circular control points array
    [SerializeField] private const int maxMouseArraySize = 50;
    [SerializeField] private Vector2[] rightCursorMovements = new Vector2[maxMouseArraySize];
    [SerializeField] private Vector2[] rightCursorPositionsInOrder;
    [SerializeField] private int rRearIndex = -1;
    [SerializeField] private int rFrontIndex = -1;

    

    void Start()
    {


        // CubicBezierCurve code
        cubicBezierCurve = gameObject.AddComponent<CubicBezierCurve>();
        cubicBezierCurve.pointPrefab = pointPrefab;
        cubicBezierCurve.sphereRadius = sphereRadius;
        cubicBezierCurve.Smooth_Resolution_number = resolution;
        cubicBezierCurve.maxArraySize = maxMouseArraySize;


        
    }

    void Update()
    {
        // Here, you should update your controlPoints array as needed
        // For example, update controlPoints based on user input, animation, etc.
        currentMousePosition = GetMousePosition();
        // Call DrawCurve with the updated controlPoints array
        if(currentMousePosition != PreviousMousePosition)
        {
            AddMousePositionRight(ref rightCursorMovements, currentMousePosition);
        }
        rightCursorPositionsInOrder = RightMousePositionInOrder();
        cubicBezierCurve.DrawCurve(rightCursorPositionsInOrder);

        PreviousMousePosition = currentMousePosition;
    }

    Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -10.0f;
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return newPosition;
    }

    //-------------------------------------------------------------------------------
    // function for Mouse Position Right


    public void AddMousePositionRight(ref Vector2[] rightMouseMovements, Vector3 cursorPosition)
    {

        if ((rRearIndex + 1) % maxMouseArraySize == rFrontIndex)
        {
            DeleteMousePositionRight();
            //Debug.Log("Circular array is full");
            //return;
        }

        if (rFrontIndex == -1)
        {
            rFrontIndex = 0;
        }

        rRearIndex = (rRearIndex + 1) % maxMouseArraySize;
        rightMouseMovements[rRearIndex].x = cursorPosition.x;
        rightMouseMovements[rRearIndex].y = cursorPosition.y;
    }


    public void DeleteMousePositionRight()
    {
        if (IsEmptyRight())
        {
            Debug.Log("Cirtualr array is empty. Unable to dequeue.");
        }


        if (rFrontIndex == rRearIndex)
        {
            rFrontIndex = -1;
            rRearIndex = -1;
        }
        else
        {
            rFrontIndex = (rFrontIndex + 1) % maxMouseArraySize;
        }
    }



    public bool IsEmptyRight()
    {
        return rFrontIndex == -1 && rRearIndex == -1;
    }

    // Get all mouse positions

    public Vector2[] RightMousePositionInOrder()
    {
        if (IsEmptyRight())
        {
            Debug.Log("Cirtualr array is empty. Unable to dequeue.");
            return new Vector2[0];
        }

        Vector2[] mousePositions = new Vector2[maxMouseArraySize];

        int index = 0;

        for (int i = rFrontIndex; i != rRearIndex; i = (i + 1) % maxMouseArraySize)
        {
            mousePositions[index] = rightCursorMovements[i];
            index++;
        }

        mousePositions[index] = rightCursorMovements[rRearIndex];

        return mousePositions;
    }

}
