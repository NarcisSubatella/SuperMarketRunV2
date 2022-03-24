using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(GameManager.Instance.PlayerMove == true)
		{
			transform.Rotate(new Vector3(4f, 0, 0));
		}
		
    }
}
