using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public void VisualizeDeath(Vector3 deadRotation, Color deadColor)
    {
        transform.eulerAngles = deadRotation;
        GetComponent<SpriteRenderer>().color = deadColor;
    }
}