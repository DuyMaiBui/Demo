using UnityEngine;
using UnityEngine.UI;

public class RadialLayout : MonoBehaviour
{
    public Vector2 size;
    public float radius;
    [Range(0f, 360f)]
    public float maxAngle;
    [Range(0f, 360f)]
    public float beginAngle;

    public bool perfectCircle;

    private RectTransform[] rects;
    private float stepAngle;

    public void UpdateEditor()
    {
        rects = new RectTransform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            rects[i] = transform.GetChild(i).GetComponent<RectTransform>();
            rects[i].sizeDelta = size;
        }

        if (perfectCircle)
        {
            maxAngle = 360f;
            stepAngle = maxAngle / transform.childCount;
            beginAngle = 90 - stepAngle;
        }
        stepAngle = maxAngle / transform.childCount;
        float angle = beginAngle;
        for (int i = 0; i < rects.Length; i++)
        {
            rects[i].transform.position = transform.position + new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            angle += stepAngle;
        }
    }
}
