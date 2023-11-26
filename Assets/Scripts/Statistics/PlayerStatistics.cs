using UnityEngine;

public class PlayerStatistics
{
    public int HighScoreCount { get; private set; }
    public int Score { get; private set; }

    public int OrangeCoinsCount { get; private set; }
    public int RedCoinsCount { get; private set; }

    public void IncreaseScore(int count)
    {
        Score += count;
        if (Score > HighScoreCount)
            HighScoreCount = Score;

        if (HighScoreCount >= 5000)
        {
            PlayerPrefsSafe.SetInt("isUnlocked3DKnight", 1);
            PlayerPrefsSafe.SetInt("isUnlocked2DKnight", 1);
        }

        if (HighScoreCount >= 6666)
        {
            PlayerPrefsSafe.SetInt("isUnlocked3DDemon", 1);
            PlayerPrefsSafe.SetInt("isUnlocked2DDemon", 1);
        }

        if (!Social.localUser.authenticated) 
            return;
        
        Social.ReportScore(HighScoreCount, GPS.leaderboard_best_runners, (success) => { });

        if (HighScoreCount > 0)
            Social.ReportProgress(GPS.achievement_foundation_of_the_foundations, 101f, (success) => { });

        if (HighScoreCount >= 5000)
            Social.ReportProgress(GPS.achievement_real_knight, 101f, (success) => { });

        if (HighScoreCount >= 6666)
            Social.ReportProgress(GPS.achievement_demonic_runner, 101f, (success) => { });
    }

    public void IncreaseRedCoins(int count)
    {
        RedCoinsCount = RedCoinsCount < 0 ? 0 : RedCoinsCount;
        RedCoinsCount += count;
    }

    public void DecreaseRedCoins(int count)
    {
        if (count > RedCoinsCount) return;
        
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
        if (count > OrangeCoinsCount) return;
        
        OrangeCoinsCount -= count;
        OrangeCoinsCount = OrangeCoinsCount < 0 ? 0 : OrangeCoinsCount;
    }


    public PlayerStatistics()
    {
        OrangeCoinsCount = LoadValue("allOrangeCoins");
        RedCoinsCount = LoadValue("allRedCoins");

        HighScoreCount = LoadValue("maxScore");
        Score = LoadValue("tempScore");
    }

    public void Clear()
    {
        Score = 0;
        SaveValues();
    }
    
    public void OnDisable()
    {
        SaveValues();
    }

    private void SaveValues()
    {
        PlayerPrefsSafe.SetInt("allOrangeCoins", OrangeCoinsCount);
        PlayerPrefsSafe.SetInt("allRedCoins", RedCoinsCount);

        PlayerPrefsSafe.SetInt("maxScore", HighScoreCount);
        PlayerPrefsSafe.SetInt("tempScore", Score);
    }

    private int LoadValue(string key)
    {
        if (PlayerPrefsSafe.HasKey(key))
            return PlayerPrefsSafe.GetInt(key);
        return 0;
    }
}
