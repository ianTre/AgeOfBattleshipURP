using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "ShipData", menuName = "ScriptableObjects/ShipInformationScriptableObject", order = 1)]
public class ShipInformationScriptableObject : ScriptableObject
{
    public string shipName;
    public Sprite image;
    public int quantity;
    public string panelname;
    public GameObject PrefabToInstantiate;
}