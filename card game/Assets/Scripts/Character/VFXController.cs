using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public GameObject buf, debuff;
    private float timeCount = 0;

    private void Update()
    {
        if (buf.activeInHierarchy)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= 1.2f)
            {
                timeCount = 0;
                buf.SetActive(false);
            }
        }

        if (debuff.activeInHierarchy)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= 1.2f)
            {
                timeCount = 0;
                debuff.SetActive(false);
            }
        }
    }
}
