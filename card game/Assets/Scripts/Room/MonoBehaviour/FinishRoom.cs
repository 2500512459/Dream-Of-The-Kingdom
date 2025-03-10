using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishRoom : MonoBehaviour
{
    [Header("¹ã²¥")]
    public ObjectEventSO loadMapEvent;
    private void OnMouseDown()
    {
        //·µ»ØµØÍ¼
        loadMapEvent.RaiseEvent(null, this);
    }
}
