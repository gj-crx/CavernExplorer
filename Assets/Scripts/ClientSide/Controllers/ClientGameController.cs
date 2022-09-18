using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.IO;
using Types;

namespace ClientSideLogic
{

    public class ClientGameController : MonoBehaviour
    {
        [HideInInspector]
        public static ClientGameController Singleton;
        public DatabaseClientVersion dataBase;

        public GameObject category_NormalUnits;
        public GameObject category_Trees;

        public float ClientUnitSpeedModificator = 1.5f;


        public GameObject prefab_State;
        [Header("Other")]
        public Vector3 MapCenter = new Vector3(0, 0, 0);
        public int testradius = 10;

        private bool IsInitialized = false;

        private void Awake()
        {
            Singleton = this;
            dataBase = new DatabaseClientVersion();
        }
        private void Start()
        {

        }
        private void Update()
        {
            DebugActions();
        }
        public void InitializeClient()
        {
            if (IsInitialized == false)
            {
                ClientStateController.StartReCheckingStates();
                IsInitialized = true;
            }
            else
            {
                return;
            }
            Singleton = this;
            //client side types loading
            TypesData.LoadAllTypes(PrefabManager.Singleton.prefabs_Units, PrefabManager.Singleton.prefabs_Buildings, PrefabManager.Singleton.prefabs_Trees);
        }
        private void DebugActions(GameObject testobj = null, GameObject testobj2 = null)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                UIPlayerActions.LocalPlayer.OrderUnitToMoveServerRpc(dataBase.AllUnits[202].ID, dataBase.AllUnits[0].transform.position + new Vector3(10, 0, 0));
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log(SceneManager.GetActiveScene().GetRootGameObjects().Length);
            }
        }

        
        
        
    }
}
