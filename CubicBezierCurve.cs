
// takes control points from another class
// for vector2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicBezierCurve : MonoBehaviour
{
    public GameObject pointPrefab; // Prefab for the points
    public float sphereRadius = 0.1f; // Radius for the spheres
    public int Smooth_Resolution_number = 20; // Resolution of the curve
    public int maxArraySize;

    // for tracking control points. if they exceeds from maxArraySize (in BeizerCurveManager) then delete oldest control point.
    private GameObject[] controlPointSpheres = new GameObject[150]; // Array to track control points
    private int currentPointIndex = 0;
    public void DrawCurve(Vector2[] controlPoints)
    {
        if (controlPoints.Length < 4)
        {
            Debug.LogError("Need at least 4 control points to create a cubic Bezier curve.");
            return;
        }
        
        
        // Remove old control points if necessary
        while (controlPoints.Length > maxArraySize)
            {
                // Remove the oldest control point
                Destroy(controlPointSpheres[currentPointIndex]);
                controlPointSpheres[currentPointIndex] = null;
                currentPointIndex = (currentPointIndex + 1) % maxArraySize;
            }

        // Create spheres at control points
        for (int i = 0; i < controlPoints.Length; i++)
        {
            int sphereIndex = (currentPointIndex + i) % maxArraySize;
            if (controlPointSpheres[sphereIndex] != null)
            {
                Destroy(controlPointSpheres[sphereIndex]);
            }

            GameObject sphere = Instantiate(pointPrefab, controlPoints[i], Quaternion.identity);
            sphere.transform.localScale = Vector2.one * sphereRadius;
            controlPointSpheres[sphereIndex] = sphere;
        }

        // Draw cubic Bezier curves between control points
        for (int i = 0; i < controlPoints.Length - 3; i += 3)
        {
            DrawCubicBezierCurve(controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3]);
        }
    }

    private void DrawCubicBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Vector2 previousPoint = p0;
        for (int i = 1; i <= Smooth_Resolution_number; i++)
        {
            float t = i / (float)Smooth_Resolution_number;
            Vector2 point = CalculateBezierPoint(t, p0, p1, p2, p3);
            Debug.DrawLine(previousPoint, point, Color.green, 0.1f); // Line visible for 0.1 seconds
            previousPoint = point;
        }
    }

    private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0; // (1-t)^3 * P0
        p += 3 * uu * t * p1; // 3(1-t)^2 * t * P1
        p += 3 * u * tt * p2; // 3(1-t) * t^2 * P2
        p += ttt * p3;        // t^3 * P3

        return p;
    }
}
