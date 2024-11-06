using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public GameObject menuPop;
    public GameObject gamePop;
    public GameObject settingPop;


    void Start()
    {
        menuPop.SetActive(true);
        gamePop.SetActive(false);
    }

    public void OpenGameMenu()
    {
        menuPop.SetActive(false);
        gamePop.SetActive(true);
    }
    
    public void CloseGameMenu()
    {
        menuPop.SetActive(true); 
        gamePop.SetActive(false);
    }
    public void OpenSettingMenu()
    {
        menuPop.SetActive(false);
        settingPop.SetActive(true);
    }

    public void CloseSettingMenu()
    {
        menuPop.SetActive(true);
        settingPop.SetActive(false);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
