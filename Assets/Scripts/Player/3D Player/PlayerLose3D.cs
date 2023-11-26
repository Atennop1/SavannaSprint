public class PlayerLose3D : PlayerLose
{
    protected override void TemplateCostLayoutSetup()
    {
        RebornCostLayout.padding.left = 0;
        if (StatisticsView.TempStatisticsModel.LifesCount < 2) RebornCostLayout.padding.left -= 66;
        else if (StatisticsView.TempStatisticsModel.LifesCount < 19) RebornCostLayout.padding.left -= 33;

        RespawnButton.interactable = _statisticsChanger.StatisticsView.StatisticsModel.OrangeCoinsCount >= 500 * StatisticsView.TempStatisticsModel.LifesCount;
        RebornCostText.text = (500 * StatisticsView.TempStatisticsModel.LifesCount).ToString();
    }
}