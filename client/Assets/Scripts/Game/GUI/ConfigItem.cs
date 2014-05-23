using UnityEngine;
using System.Collections;

[System.Serializable]
public class ConfigItem {
    public GUIContent display;
    public GunType relatedGunChoice;
    public GameObject shipModelPrefab;
    public GameObject shipShieldEffectPrefab;
    public bool isWeaponConfig;
}