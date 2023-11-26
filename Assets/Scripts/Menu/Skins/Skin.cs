using UnityEngine;
using VoxelImporter;

[System.Serializable]
[CreateAssetMenu(menuName = "SkinInfo", fileName = "New SkinInfo")]
public class Skin : ScriptableObject
{
    [SerializeField] private Material _skinMaterial;
    [SerializeField] private RuntimeAnimatorController _skinAnimatorController;

    [Space]
    [SerializeField] private Shader _standartShader;
    [SerializeField] private Shader _diffuseShader;

    [Space]
    [SerializeField] private Material _dustMaterial;
    [SerializeField] private Mesh[] _gameOverParticlesMeshes;

    [Space]
    [SerializeField] private SkinType _type;
    [SerializeField] private int _cost;
    [SerializeField] private float _3DCostCoefficient;

    private Shader _selectedShader;

    public bool IsSelected { get; private set; }
    public bool IsUnlocked { get; private set; }

    public void Init(bool is3DMode)
    {
        _selectedShader = PlayerPrefs.GetInt("shaderIntPP") == 0 ? _diffuseShader : _standartShader;
        IsUnlocked = PlayerPrefsSafe.GetInt("isUnlocked" + (is3DMode ? "3D" : "2D") + _type) == 1;
        IsSelected = PlayerPrefs.GetString("ActiveSkin" + (is3DMode ? "3D" : "2D")) == _type.ToString();
    }

    public bool TryBuy(bool is3DMode, PlayerStatistics statistics)
    {
        var currentCoins = is3DMode ? statistics.OrangeCoinsCount : statistics.RedCoinsCount;
        var currentCost = is3DMode ? (int)(_cost * _3DCostCoefficient) : _cost;

        if (currentCoins < currentCost) return false;
        TryGiveAchievement();

        IsUnlocked = true;
        PlayerPrefsSafe.SetInt("isUnlocked" + (is3DMode ? "3D" : "2D") + _type, 1);

        if (is3DMode) statistics.DecreaseOrangeCoins(currentCost);
        else statistics.DecreaseRedCoins(_cost);

        return true;

    }

    public void Select(bool is3DMode)
    {
        IsSelected = true;
        PlayerPrefs.SetString("ActiveSkin" + (is3DMode ? "3D" : "2D"), _type.ToString());
    }

    public void SetupSkinMaterial(VoxelFrameAnimationObject animationObject)
    {
        var material = _skinMaterial;
        material.shader = _selectedShader;
        animationObject.playMaterial0 = material;
    }

    public void SetupSkinAnimator(Animator animator)
    {
        animator.runtimeAnimatorController = _skinAnimatorController;
    }

    public void SetupGameOverParticles(ParticleSystemRenderer renderer)
    {
        renderer.SetMeshes(_gameOverParticlesMeshes, _gameOverParticlesMeshes.Length);
    }

    public void SetupDust(ParticleSystemRenderer renderer)
    {
        renderer.material = _dustMaterial;
    }

    public void UnSelect()
    {
        IsSelected = false;
    }

    private void TryGiveAchievement()
    {
        if (Social.localUser.authenticated && _type == SkinType.Golden)
            Social.ReportProgress(GPS.achievement_richer_than_midas, 101f, (success) => { });
    }
}
