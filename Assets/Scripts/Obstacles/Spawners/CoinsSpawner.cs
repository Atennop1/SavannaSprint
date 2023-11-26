using UnityEngine;

public enum CoinsStyle 
{ 
    Line, 
    Jump, 
    Ramp, 
    None 
}

public class CoinsSpawner : Spawner
{
    [Header("Coins Stuff")]
    [SerializeField] private float _minimumCoinsCountInItem = 13;
    [SerializeField] private float _minimumSpaceBetweenCoins = 15;
    [SerializeField] private float _coinYPosition = 0.45f;

    private float _lengthOfCoinsParabola;
    private float _currentCoinsCountInItem;
    private float _currentSpaceBetweenCoins;

    private Pool<Component> _pool;

    public void CreateCoins(CoinsStyle style, Vector3 pos, GameObject parentObject)
    {
        if (style == CoinsStyle.None)
            return;

        if (_currentCoinsCountInItem == 0)
            Start();

        var coinPos = Vector3.zero;
        switch (style)
        {
            case CoinsStyle.Line:
                for (var i = -((int)_currentCoinsCountInItem / 2) + 1; i < (int)_currentCoinsCountInItem / 2 + 1; i++)
                {
                    coinPos.y = _coinYPosition;
                    coinPos.z = i * (_currentSpaceBetweenCoins / _currentCoinsCountInItem);
                    _pool?.GetFreeElement(coinPos + pos, parentObject.transform);
                }

                break;

            case CoinsStyle.Jump:
                for (var i = -((int)_currentCoinsCountInItem / 2) + 1; i < (int)_currentCoinsCountInItem / 2 + 1; i++)
                {
                    coinPos.y = Mathf.Max(-0.4f / (int)_lengthOfCoinsParabola * Mathf.Pow(i, 2) + (Is3DMode ? 4 : 3.5f), _coinYPosition);
                    coinPos.z = i * (_currentSpaceBetweenCoins * (Is3DMode ? 1.0f : 1) / _currentCoinsCountInItem);
                    _pool?.GetFreeElement(coinPos + pos, parentObject.transform);
                }

                break;

            case CoinsStyle.Ramp:
                for (var i = -((int)_minimumCoinsCountInItem / 2) + 1; i < (int)_minimumCoinsCountInItem / 2 + 1; i++)
                {
                    coinPos.y = Mathf.Min(Mathf.Max(1.6f * (i + 2), _coinYPosition), 5);
                    coinPos.z = i * (_minimumSpaceBetweenCoins / _minimumCoinsCountInItem);
                    _pool?.GetFreeElement(coinPos + pos, parentObject.transform);
                }

                break;
        }
    }

    public void UpdateValues()
    {
        _currentCoinsCountInItem += 0.06f;
        _currentSpaceBetweenCoins += 0.06f;
        _lengthOfCoinsParabola += Is3DMode ? 0.032f : 0.024f;
    }

    private void Start()
    {
        _pool = GetPool(ObstaclesData.Coin.transform)?.Pool;
        _lengthOfCoinsParabola = 1 + (int)((PlayerForwardMovement.Speed - 10) / 2.5f);

        _currentCoinsCountInItem = 0;
        _currentCoinsCountInItem += _minimumCoinsCountInItem * (PlayerForwardMovement.Speed / 15);

        _currentSpaceBetweenCoins = 0;
        _currentSpaceBetweenCoins += _minimumSpaceBetweenCoins * (PlayerForwardMovement.Speed / 15);
    }
}
