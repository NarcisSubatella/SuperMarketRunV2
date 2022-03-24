using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LvManager : MonoBehaviour
{
    public static LvManager current;
    public int actualLevel;
   // public int totalPoints;
    public bool resetProgress;

    

    private void Awake()
    {
        current = this;
            
        DontDestroyOnLoad(current);

      //PlayerPrefs.DeleteAll();
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

      /*  if(PlayerPrefs.HasKey("CurrentLv") && actualLevel==0)
        {
            actualLevel = 1;
        }*/
        
    }
    private void Start()
    {
        
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
