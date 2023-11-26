using UnityEngine;
using UnityEngine.UI;

public class AgeConfirm : MonoBehaviour
{
    [SerializeField] private InputField _ageField;
    [SerializeField] private AudioSource _selectSource;
    
    public void Click()
    {
        _selectSource.Play();
        if (_ageField.text == "") 
            return;
        
        var age = int.Parse(_ageField.text);
        PlayerPrefs.SetInt("age", age);
        gameObject.SetActive(false);
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://doc-hosting.flycricket.io/savanna-sprint-two-worlds-privacy-policy/69a693a6-78ef-4332-9b77-a35defafbc14/privacy");
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey("age"))
            gameObject.SetActive(false);
    }
}