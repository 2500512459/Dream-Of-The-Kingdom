using UnityEngine;

public class CardTransform
{
    public Vector3 pos;
    public Quaternion rotation;

    public CardTransform(Vector3 pos, Quaternion rotation)
    {
        this.pos = pos;
        this.rotation = rotation;
    }
}
