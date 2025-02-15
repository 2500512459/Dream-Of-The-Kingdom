using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private float offsetSpeed = 0.1f;
    //ÈÃÏß¹ö¶¯
    private void Update()
    {
        if (lineRenderer != null)
        {
            var offset = lineRenderer.material.mainTextureOffset;
            offset.x += offsetSpeed * Time.deltaTime;
            lineRenderer.material.mainTextureOffset = offset;
        }
    }
}
