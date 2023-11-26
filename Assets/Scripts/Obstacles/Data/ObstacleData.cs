using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleData
{
    [field: SerializeField] public float Chance { get; private set; }

    [Header("LODs")]
    [SerializeField] private GameObject _LOD0;
    [SerializeField] private GameObject _LOD1;
    [SerializeField] private GameObject _LOD2;
    
    [field: SerializeField] public bool IsForOneLine { get; private set; }

    [Header("Info")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _coinsOffset;

    [Space]
    [SerializeField] private List<CoinsStyle> _coinsStyles = new List<CoinsStyle>();

    public CoinsStyle CoinsStyle { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 CoinsPosition { get; private set; }
    public GameObject SelectedLOD { get; private set; }

    public void Setup(LinePosition linePosition, float zPosition)
    {
        CoinsStyle = _coinsStyles[Random.Range(0, _coinsStyles.Count)];
        Position = new Vector3(IsForOneLine ? (float)linePosition * 3.3f : 0, 0, zPosition) + _offset;
        CoinsPosition = new Vector3(IsForOneLine ? Position.x + _coinsOffset.x : 3.3f * Random.Range(1, -2), Position.y + _coinsOffset.y, Position.z + _coinsOffset.z);
    }

    public void SetupLOD()
    {
        if (PlayerPrefs.GetInt("GraphicsQuality") == 0)
            SelectedLOD = _LOD2;
        else if (PlayerPrefs.GetInt("GraphicsQuality") == 1)
            SelectedLOD = _LOD1;
        else
            SelectedLOD = _LOD0;
    }
}