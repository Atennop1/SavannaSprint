using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "SkinInfo", fileName = "New SkinInfo")]
public class SkinInfo : ScriptableObject
{
    public Material skinMaterial;
    public RuntimeAnimatorController skinAnimator;

    [Space]
    public Shader standartShader;
    public Shader diffuseShader;

    [Space]
    public Material dustMaterial;
    public Mesh[] gameOverParticlesMeshes;

    [HideInInspector] public Shader currentShader;

    public void Init()
    {
        if (PlayerPrefs.GetInt("shaderIntPP") == 0)
            currentShader = diffuseShader;
        else
            currentShader = standartShader;
    }
}
