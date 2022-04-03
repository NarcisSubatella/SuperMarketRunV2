using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchToStart : MonoBehaviour
{
 //Inicializa el juego
    void Update()
    {
        if (Input.touchCount==1)
        {
            GameManager.Instance.StartGame();
        }
    }
    
}
