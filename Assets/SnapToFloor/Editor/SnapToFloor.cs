using UnityEngine;
using UnityEditor;

public class SnapToFloor : MonoBehaviour
{
    [MenuItem("GameObject/Snap To Floor _%#m", false, 2000)]
    private static void SnapBounds()
    {
        Snap(true);
    }

    [MenuItem("GameObject/Snap Pivot To Floor _%#p", false, 2001)]
    private static void SnapPivot()
    {
        Snap(false);
    }

    [MenuItem("GameObject/Snap +X (Right) _%#RIGHT", false, 2002)]
    private static void SnapPositiveX()
    {
        SnapAxis(Vector3.right);
    }

    [MenuItem("GameObject/Snap -X (Left) _%#LEFT", false, 2003)]
    private static void SnapNegativeX()
    {
        SnapAxis(Vector3.left);
    }

    [MenuItem("GameObject/Snap +Z (Forward) _%#UP", false, 2004)]
    private static void SnapPositiveZ()
    {
        SnapAxis(Vector3.forward);
    }

    [MenuItem("GameObject/Snap -Z (Back) _%#DOWN", false, 2005)]
    private static void SnapNegativeZ()
    {
        SnapAxis(Vector3.back);
    }

    private static void Snap(bool useBounds)
    {
        foreach (var transform in Selection.transforms)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
            {
                float offset = useBounds ? FindOffset(transform) : 0f;
                string undoName = useBounds
                    ? "Snap To Floor"
                    : "Snap Pivot To Floor";

                Undo.RecordObject(transform, undoName);
                transform.position = hit.point + Vector3.up * offset;
            }
        }
    }

    private static void SnapAxis(Vector3 direction)
    {
        foreach (var transform in Selection.transforms)
        {
            if (Physics.Raycast(transform.position, -direction, out RaycastHit hit))
            {
                float offset = FindAxisOffset(transform, direction);

                Undo.RecordObject(transform, "Snap To " + direction);
                transform.position = hit.point + direction * offset;
            }
        }
    }

    private static float FindOffset(Transform parent)
    {
        Bounds bounds = new Bounds(parent.position, Vector3.zero);

        foreach (Renderer renderer in parent.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return parent.position.y - (bounds.center.y - bounds.extents.y);
    }

    private static float FindAxisOffset(Transform parent, Vector3 direction)
    {
        Bounds bounds = new Bounds(parent.position, Vector3.zero);

        foreach (Renderer renderer in parent.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        if (direction == Vector3.right)
            return bounds.max.x - parent.position.x;

        if (direction == Vector3.left)
            return parent.position.x - bounds.min.x;

        if (direction == Vector3.forward)
            return bounds.max.z - parent.position.z;

        if (direction == Vector3.back)
            return parent.position.z - bounds.min.z;

        return 0f;
    }
}