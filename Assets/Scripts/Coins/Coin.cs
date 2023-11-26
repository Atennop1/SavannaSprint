using UnityEngine;
public class Coin : MonoBehaviour 
{ 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerStatisticsChanger playerStatisticsChanger)) 
            return;
        
        playerStatisticsChanger.PickUpCoin();
        gameObject.SetActive(false);
    }
}