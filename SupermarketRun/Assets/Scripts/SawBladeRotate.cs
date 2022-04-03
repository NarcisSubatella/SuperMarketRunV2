using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeRotate : MonoBehaviour
{
    void Update()
    {
		if(GameManager.Instance.PlayerMove == true)
		{
			transform.Rotate(new Vector3(4f, 0, 0));
		}
		
    }
}
