using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigGUI : MonoBehaviour {
    public ConfigItem[] configItems;
    public GUIStyle configStyle;
    public Rect configArea;

    private List<ConfigItem> weaponConfigs = new List<ConfigItem>();
    private List<ConfigItem> shipConfigs = new List<ConfigItem>();

	// Use this for initialization
	void Start () {
        foreach (ConfigItem ci in configItems) {
            if (ci.isWeaponConfig) {
                weaponConfigs.Add(ci);
            } else {
                shipConfigs.Add(ci);
            }
        }
	
	}

    void OnGUI() {
        GUILayout.BeginArea(new Rect((Screen.width * 0.5f) - (configArea.width* 0.5f), 0,
            configArea.width, configArea.height));

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        DrawWeaponConfigs();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        DrawShipConfigs();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public void DrawWeaponConfigs() {
        foreach (ConfigItem ci in weaponConfigs) {
            if (GUILayout.Button(ci.display, configStyle, GUILayout.MaxWidth(128), GUILayout.MaxHeight(128))) {
                GameManager.gameManager.CurrentWeaponChoice = ci.relatedGunChoice;
            }
        }
    }

    public void DrawShipConfigs() {
        foreach (ConfigItem ci in shipConfigs) {
            if (GUILayout.Button(ci.display, configStyle, GUILayout.MaxWidth(128), GUILayout.MaxHeight(128))) {
                //TODO: implement choosing of new ship prefab models;
            }
        }
    }
}
