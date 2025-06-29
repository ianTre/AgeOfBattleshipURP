using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;


public class ShipPanelControl : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI namelabel;
  [SerializeField] TextMeshProUGUI shipUsableNumber;
  [SerializeField] Image panelImage;



    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    public void SetData(ShipInformationScriptableObject shipInformationScriptableObject)
    {
        
        RectTransform shipInfoPanel = GetComponent<RectTransform>();
        shipInfoPanel.gameObject.SetActive(true);
        GameObject panel = GameObject.Find(shipInformationScriptableObject.panelname);
        RectTransform shipPanels = panel.GetComponent<RectTransform>();
        namelabel.text = "Name: " + shipInformationScriptableObject.shipName.ToString();
        shipUsableNumber.text = "Number of ships available: " + shipInformationScriptableObject.quantity;
        shipInfoPanel.anchoredPosition = new Vector2(shipPanels.anchoredPosition.x, shipInfoPanel.anchoredPosition.y);
        panelImage.sprite = shipInformationScriptableObject.image;
      
    }

    public void sendConsoleMessage(string message)
    {
        Debug.Log(message); 
    }


    public void ClearPanel(ShipInformationScriptableObject shipInformationScriptableObject)
    {
        RectTransform shipInfoPanel = GetComponent<RectTransform>();
        shipInfoPanel.gameObject.SetActive(false);
    }

  
}
