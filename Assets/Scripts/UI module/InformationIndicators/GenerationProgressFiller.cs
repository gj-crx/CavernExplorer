using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Generation;

namespace UI
{
    public class GenerationProgressFiller : MonoBehaviour
    {
        private Image fillerImage;
        private Text progressText;
        private int totalProgressNeeded = 0;
        private void OnEnable()
        {
            fillerImage = GetComponent<Image>();
            progressText = transform.Find("ProgressAmount").GetComponent<Text>();
            int Radius = GameManager.MapGenerator.CurrentGenSettings.StartingSectorsCreationRadius;
            totalProgressNeeded = 0;
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    totalProgressNeeded++;
                }
            }
            totalProgressNeeded += 1;

        }
        
        void Update()
        {
            fillerImage.fillAmount = (float)GameManager.MapGenerator.UIGenerationProgress / totalProgressNeeded;
            progressText.text = GameManager.MapGenerator.UIGenerationProgress + " / " + totalProgressNeeded;
        }
    }
}
