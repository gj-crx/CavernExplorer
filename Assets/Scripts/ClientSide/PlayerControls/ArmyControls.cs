using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace ClientSideLogic
{
    public static class ArmyControls
    {
        private static void OrderArmyToAttackMove(Vector3 Target)
        {
            foreach (var Unit in UIPlayerActions.LocalPlayer.clientSide_ControlledState.MainArmy)
            {
                Unit.CurrentTargetID = -1;
                UIPlayerActions.LocalPlayer.OrderUnitToMoveServerRpc(Unit.ID, Target);
            }
        }
        public static void CheckOrderToAttackMove()
        {
            if (UIPlayerActions.IsOrderingArmyMovement)
            {
                Vector2 ClickedScreenPoint = Vector2.zero;
                if (UIPlayerActions.PhoneControls)
                {
                    if (Input.touchCount > 0) ClickedScreenPoint = Input.GetTouch(0).position;
                }
                else
                {
                    if (Input.GetMouseButtonDown(0)) ClickedScreenPoint = Input.mousePosition;
                }
                if (ClickedScreenPoint != Vector2.zero)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(ClickedScreenPoint);
                    if (Physics.Raycast(ray, out hit))
                    {
                        OrderArmyToAttackMove(ray.GetPoint(hit.distance));
                    }
                }
            }
        }
    }
}
