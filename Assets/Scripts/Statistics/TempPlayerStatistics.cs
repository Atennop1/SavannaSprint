public class TempPlayerStatistics
{
    public int OrangeCoinsCount { get; private set; }
    public int RedCoinsCount { get; private set; }

    public int LifesCount { get; private set; } = 1;

    public float SpaceBetweenObstacles { get; private set; }
    public float Speed { get; private set; }

    public bool IsSpaceValid => SpaceBetweenObstacles != 0;
    public bool IsSpeedValid => Speed != 0;

    private bool _cleared;

    public void IncreaseRedCoins(int count)
    {
        RedCoinsCount = RedCoinsCount < 0 ? 0 : RedCoinsCount;
        RedCoinsCount += count;
    }

    public void DecreaseRedCoins(int count)
    {
        RedCoinsCount -= count;
        RedCoinsCount = RedCoinsCount < 0 ? 0 : RedCoinsCount;
    }

    public void IncreaseOrangeCoins(int count)
    {
        OrangeCoinsCount = OrangeCoinsCount < 0 ? 0 : OrangeCoinsCount;
        OrangeCoinsCount += count;
    }

    public void DecreaseOrangeCoins(int count)
    {
        OrangeCoinsCount -= count;
        OrangeCoinsCount = OrangeCoinsCount < 0 ? 0 : OrangeCoinsCount;
    }

    public void IncreaseLifesCount()
    {
        LifesCount = LifesCount < 0 ? 0 : LifesCount;
        LifesCount++;
    }

    public void IncreaseSpaceBetweenObstacles(float count)
    {
        SpaceBetweenObstacles = SpaceBetweenObstacles < 0 ? 0 : SpaceBetweenObstacles;
        SpaceBetweenObstacles += count;
    }

    public void IncreaseSpeed(float count)
    {
        Speed = Speed < 0 ? 0 : Speed;
        Speed += count;
    }

    public TempPlayerStatistics()
    {
        OrangeCoinsCount = LoadIntValue("tempOrangeCoins");
        RedCoinsCount = LoadIntValue("tempRedCoins");
        LifesCount = LoadIntValue("tempLifesCount", 1);

        SpaceBetweenObstacles = LoadFloatValue("tempSpaceBetweenObstacles");
        Speed = LoadFloatValue("tempSpeed");
    }

    public void Clear()
    {
        OrangeCoinsCount = 0;
        RedCoinsCount = 0;
        LifesCount = 1;

        SaveValues();

        PlayerPrefsSafe.DeleteKey("tempSpaceBetweenObstacles");
        PlayerPrefsSafe.DeleteKey("tempSpeed");

        _cleared = true;
    }

    public void OnDisable()
    {
        if (!_cleared)
            SaveValues();
    }

    private void SaveValues()
    {
        PlayerPrefsSafe.SetInt("tempOrangeCoins", OrangeCoinsCount);
        PlayerPrefsSafe.SetInt("tempRedCoins", RedCoinsCount);
        PlayerPrefsSafe.SetInt("tempLifesCount", LifesCount);

        PlayerPrefsSafe.SetFloat("tempSpaceBetweenObstacles", SpaceBetweenObstacles);
        PlayerPrefsSafe.SetFloat("tempSpeed", Speed);
    }

    private int LoadIntValue(string key, int defaultValue = 0)
    {
        if (PlayerPrefsSafe.HasKey(key))
            return PlayerPrefsSafe.GetInt(key);
        return defaultValue;
    }

    private float LoadFloatValue(string key, float defaultValue = 0)
    {
        if (PlayerPrefsSafe.HasKey(key))
            return PlayerPrefsSafe.GetFloat(key);
        return defaultValue;
    }
}
