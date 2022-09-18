using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ClientSideLogic;
using ServerSideLogic;
using ServerSideLogic.Matchmaking;
using Types;

public class Player : NetworkBehaviour
{
    public NetPlayerLobbyActions PlayerLobbyActions;

    public sbyte ConnectedLobbyID = 0;

    public StateClientSide clientSide_ControlledState;
    public State serverSide_ControlledState;

    //SERVER SIDE ONLY
    public PlayerAccount account;
    //


    //public State ControlledState;

    private void Awake()
    {
        PlayerLobbyActions = GetComponent<NetPlayerLobbyActions>();
    }
    private void Start()
    {
        if (IsLocalPlayer)
        {
            gameObject.name = "LocalPlayer";
            UIPlayerActions.LocalPlayer = this;
            ConnectToServerServerRpc();
        }
    }
    private void Update()
    {
        if (IsLocalPlayer)
        {
            ArmyControls.CheckOrderToAttackMove();
        }
    }

    [ServerRpc]
    public void TrainUnitOrderServerRpc(int BuildingToTrainReferenceID, byte QueuedUnitTypeID)
    {
        //server side check if it's enough resources to build
        LobbyManager.Lobbies[ConnectedLobbyID].GameRunningInLobby.DB.Buildings[BuildingToTrainReferenceID].EnqueueUnitTraining(QueuedUnitTypeID);
    }
    [ServerRpc]
    public void CreateBuildingOrderServerRpc(byte UnitTypeID, Vector3 BuildingPosition)
    {
        //server side check if it's enough resources to build
        if (UnitLogic.IsPossibleToBuildAUnit(UnitTypeID, serverSide_ControlledState))
        {
            UnitLogic.SubstractResourcesForUnitCost(UnitTypeID, serverSide_ControlledState);
            Building NewBuilding = new Building(TypesData.BuildingTypes[UnitTypeID], LobbyManager.Lobbies[ConnectedLobbyID].GameRunningInLobby, BuildingPosition, serverSide_ControlledState, UnitTypeID);
        }
    }
    [ServerRpc]
    public void OrderUnitToMoveServerRpc(int UnitGlobalID, Vector3 Target, sbyte MovementOrderType = 0)
    {
        Unit OrderedUnit = LobbyManager.Lobbies[ConnectedLobbyID].GameRunningInLobby.DB.AllUnits[UnitGlobalID];
        if (OrderedUnit.behavior != null)
        {
            OrderedUnit.behavior.CurrentTargetID = -1;
            OrderedUnit.behavior.HaveOrder = true;
        }
        bool b = OrderedUnit.GetWayTarget(Target);
        Debug.Log("found " + b + " by " + UnitGlobalID + " " + OrderedUnit.Type.UnitTypeName + " target " + Target);
    }
    [ServerRpc]
    public void ConnectToServerServerRpc()
    {
        account = LobbyManager.ConnectToServer(this);
        Debug.Log("Logged as " + account.Nickname);
    }
    [ServerRpc]
    public void StartLobbyOrderServerRpc()
    {

    }
    public void AssignControlledState(short StateID, int PlayerOwnerID)
    {
        if (PlayerOwnerID == (int)OwnerClientId)
        {
            clientSide_ControlledState = ClientGameController.Singleton.dataBase.States[StateID];
        }
    }
    [ClientRpc]
    public void UIPreparingGameStartedClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("in game gui started");
        ClientSideLogic.UI.UIController.Singleton.StartInGameGUI();
    }


}
