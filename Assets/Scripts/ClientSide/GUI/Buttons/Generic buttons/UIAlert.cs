using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientSideLogic.UI
{
    public class UIAlert : MonoBehaviour
    {
        public float AlertFadeTime = 3;
        private float timer_FadeTime = 0;
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(AlertClicked);
        }
        private void Update()
        {
            if (timer_FadeTime > AlertFadeTime)
            {
                AlertClicked();
            }
            else timer_FadeTime += Time.deltaTime;
        }
        // Update is called once per frame
        private void AlertClicked()
        {
            timer_FadeTime = 0;
            gameObject.SetActive(false);
        }
    }
}
