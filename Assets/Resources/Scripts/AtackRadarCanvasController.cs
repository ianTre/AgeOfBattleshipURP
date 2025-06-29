using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class AtackRadarCanvasController : MonoBehaviour
{
    private float spotSize= 60;
    [SerializeField] Image gridImage;
    public static AtackRadarCanvasController instance;
    

    void Awake() {
        instance = this;    
    }

    void Start()
    {

    }
    public void CreateGrid(int rows, int columns)
    {
        float screenHeight,screenWidth;
        screenHeight = Screen.height * 0.8f;
        screenWidth = Screen.width * 0.8f;
        float maxYValue =  screenHeight / (rows * 1.1f );
        float maxXValue = screenWidth / (columns * 1.1f );
        spotSize = maxXValue < maxYValue ? maxXValue : maxYValue;
        gridImage.rectTransform.sizeDelta = new Vector2(spotSize,spotSize);
        float yPosition;
        //(float)(oldPos.y - (Math.Ceiling(spotSize * 1.1)) * i )
        for (int i = 0; i < rows; i++)
        {
            yPosition = gridImage.rectTransform.position.y - ((float)Math.Ceiling(spotSize * 1.1) * i );
            for (int j = 0; j < columns; j++)
            {
                var obj = Instantiate(gridImage,this.transform);
                obj.gameObject.SetActive(true);
                var oldPos =obj.rectTransform.position;
                obj.rectTransform.position = new Vector3((float)(oldPos.x + (Math.Ceiling(spotSize * 1.1)) * j ), yPosition, oldPos.z);    
            }
            
        }
    }
}
