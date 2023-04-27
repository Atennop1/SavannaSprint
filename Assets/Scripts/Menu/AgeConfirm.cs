using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yodo1.MAS;

public class AgeConfirm : MonoBehaviour
{
    [SerializeField] private InputField input;
    [SerializeField] private AudioSource selectSource;
    
    public void Click()
    {
        selectSource.Play();
        if (input.text != "")
        {
            int age = int.Parse(input.text);
            PlayerPrefs.SetInt("age", age);
            gameObject.SetActive(false);
        }
    }
    public void OpenUrlPrivacyPolicy()
    {
        Application.OpenURL("https://doc-hosting.flycricket.io/savanna-sprint-two-worlds-privacy-policy/69a693a6-78ef-4332-9b77-a35defafbc14/privacy");
    }
}