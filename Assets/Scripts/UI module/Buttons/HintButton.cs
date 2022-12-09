using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class HintButton : MonoBehaviour
    {
        [SerializeField]
        private float hintLifeTime = 3.5f;
        private Text hintText;
        private Color normalColor;
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(HideHint);
            
            hintText = GetComponent<Text>();
            normalColor = hintText.color;

            gameObject.SetActive(false);
        }
        public void HideHint()
        {
            gameObject.SetActive(false);
        }
        private IEnumerator HintTimer()
        {
            yield return new WaitForSeconds(hintLifeTime);
            gameObject.SetActive(false);
        }
        private IEnumerator Fading()
        {
            while (hintText.color.a > 0)
            {
                hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, hintText.color.a - Time.deltaTime / hintLifeTime);
                yield return new WaitForFixedUpdate();
            }
        }
        private void OnEnable()
        {
            hintText.color = normalColor;
            StartCoroutine(HintTimer());
            StartCoroutine(Fading());
        }
    }
}
