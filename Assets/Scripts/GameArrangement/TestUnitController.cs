using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnitController : MonoBehaviour
{
    public Unit unit;
    public GameObject testtarget;
    public bool AnimationTest;
    void Start()
    {
        if (AnimationTest)
        {
            GetComponent<UnityEngine.Animation>().Play("minatour run");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Test();
    }
    public void Test()
    {
        unit.unitMovement.GetWayTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
