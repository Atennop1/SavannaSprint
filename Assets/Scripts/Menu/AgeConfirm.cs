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
        Application.OpenURL("https://pages.flycricket.io/3d-runner-1/privacy.html");
    }
}