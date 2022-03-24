using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    
    public void OpenSettings()
    {
        Time.timeScale = 0;
        settingsPanel.SetActive(true);
    }
    public void ClouseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void ResetProgress()
    {
        LvManager.current.ResetProgress();
    }
    public void GetCoinsAnim()
    {
        GameManager.Instance.GetCoin();
    }
    public void GetCoinsX2Anim()
    {
        GameManager.Instance.GetCoinX2();
    }
    public void TryAgainBtn()
    {
        LvManager.current.RestardLv();
    }
}
