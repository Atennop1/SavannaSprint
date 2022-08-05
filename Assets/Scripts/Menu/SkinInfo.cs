using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "SkinInfo", fileName = "New SkinInfo")]
public class SkinInfo : ScriptableObject
{
    [field: SerializeField] public Material SkinMaterial { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController SkinAnimator { get; private set; }

    [SerializeField] private Shader _standartShader;
    [SerializeField] private Shader _diffuseShader;

    [field: SerializeField, Space] public Material DustMaterial { get; private set; }
    [field: SerializeField] public Mesh[] GameOverParticlesMeshes { get; private set; }

    public Shader CurrentShader { get; private set; }

    public void Init()
    {
        if (PlayerPrefs.GetInt("shaderIntPP") == 0)
            CurrentShader = _diffuseShader;
        else
            CurrentShader = _standartShader;
    }
}
