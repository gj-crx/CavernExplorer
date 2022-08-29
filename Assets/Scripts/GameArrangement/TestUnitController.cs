using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnitController : MonoBehaviour
{
    public Unit unit;
    public GameObject testtarget;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Test();
    }
    public void Test()
    {
        unit.GetWayTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
