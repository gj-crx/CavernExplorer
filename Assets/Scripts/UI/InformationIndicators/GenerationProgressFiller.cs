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
        private int totalSectors = 0;
        private void Start()
        {
            fillerImage = GetComponent<Image>();
            progressText = transform.Find("ProgressAmount").GetComponent<Text>();
            int Radius = GameSettings.Singleton.MapGeneratorSettings.StartingSectorsCreationRadius;
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int x = -Radius; x <= Radius; x++)
                {
                    totalSectors++;
                }
            }


        }
        
        void Update()
        {
            fillerImage.fillAmount = (float)GameManager.MapGenerator.UIGenerationProgress / totalSectors;
            progressText.text = GameManager.MapGenerator.UIGenerationProgress + " / " + totalSectors;
        }
    }
}
