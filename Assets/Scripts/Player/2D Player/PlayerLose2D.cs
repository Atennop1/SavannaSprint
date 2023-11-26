public class PlayerLose2D : PlayerLose
{
    protected override void TemplateCostLayoutSetup()
    {
        RebornCostLayout.padding.left = -278;
        if (StatisticsView.TempStatisticsModel.LifesCount < 5) RebornCostLayout.padding.left -= 16;

        RespawnButton.interactable = _statisticsChanger.StatisticsView.StatisticsModel.RedCoinsCount >= 200 * StatisticsView.TempStatisticsModel.LifesCount;
        RebornCostText.text = (200 * StatisticsView.TempStatisticsModel.LifesCount).ToString();
    }
}