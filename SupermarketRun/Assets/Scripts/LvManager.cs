using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LvManager : MonoBehaviour
{
    public static LvManager current;
    public int actualLevel;

    [Header("Active to reset progression")]
    public bool resetProgress;

    

    private void Awake()
    {
        current = this;
            
        DontDestroyOnLoad(current);

        //Almacena en playerpref
      if(resetProgress==false)
        { 
            if (!PlayerPrefs.HasKey("CurrentLv"))
            {
            PlayerPrefs.SetInt("CurrentLv", 1);
            actualLevel = PlayerPrefs.GetInt("CurrentLv");
            Debug.Log("creandoCurrent");
            }
            if(PlayerPrefs.HasKey("CurrentLv"))
            {
            Debug.Log("Cargando current");
            actualLevel = PlayerPrefs.GetInt("CurrentLv");
                if (PlayerPrefs.GetInt("CurrentLv") != 1)
                { 
                SceneManager.LoadScene("Level" + (actualLevel));
                }
            }
        }
      if(resetProgress==true)
        {
            PlayerPrefs.DeleteAll();
        }
    }
    public void SceneChange()
    {
        actualLevel += 1;
        PlayerPrefs.SetInt("CurrentLv", actualLevel);
        Debug.Log(actualLevel);
        SceneManager.LoadScene("Level" + (actualLevel));
    }
    public void RestardLv()
    {
        SceneManager.LoadScene("Level" + (actualLevel));
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level1");
    }
}
