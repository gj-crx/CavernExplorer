using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class FadingRedScreenEffect : MonoBehaviour
    {
        public float EffectFadingSpeed = 1;

        [HideInInspector]
        public static FadingRedScreenEffect Singleton;
        private Image image;

        private void Awake()
        {
            Singleton = this;
        }
        void Start()
        {
            image = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (image.color.a > 0)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - EffectFadingSpeed * Time.deltaTime);
            }
        }
        public void ResetColor()
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }
    }
}
