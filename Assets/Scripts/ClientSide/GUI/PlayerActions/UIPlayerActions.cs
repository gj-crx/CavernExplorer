using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIPlayerActions
{ //serves as a buffer between UI player input and transfering actions to the server
    public static Player LocalPlayer = null;
    public static bool PhoneControls = false;
    public static bool IsOrderingArmyMovement = false;

    public static Vector3 GetPointOfClick()
    {
        Vector2 ClickedScreenPoint = Vector2.zero;
        if (UIPlayerActions.PhoneControls)
        {
            ClickedScreenPoint = Input.GetTouch(0).position;
        }
        else
        {
            ClickedScreenPoint = Input.mousePosition;
        }
        if (ClickedScreenPoint != Vector2.zero)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(ClickedScreenPoint);
            if (Physics.Raycast(ray, out hit))
            {
                return ray.GetPoint(hit.distance);
            }
            else {  return Vector3.zero; }
        }
        else
        {
            return Vector3.zero;
        }
    }
    public static bool UserClicked()
    {
        if (PhoneControls) return Input.touchCount > 0;
        else return Input.GetMouseButtonDown(0);
    }
}
