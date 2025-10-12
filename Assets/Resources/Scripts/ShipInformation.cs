using UnityEngine;

[CreateAssetMenu(fileName = "ShipData", menuName = "ScriptableObjects/ShipInformationScriptableObject", order = 1)]
public class ShipInformationScriptableObject : ScriptableObject
{
    public string shipName;
    public Sprite image;
    public int quantity;
    public string panelname;
    public GameObject PrefabToInstantiate;
}