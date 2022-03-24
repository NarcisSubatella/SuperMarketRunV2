using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerControl : MonoBehaviour
{
	public enum Triggers
	{
		GameOver,
		GameCompleted
	}
	public Triggers State;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(State == Triggers.GameCompleted)
			{
				GameManager.Instance.GameCompleted();
				GameManager.Instance.PlayerMove = false;
			}
			else if(State == Triggers.GameOver)
			{
				/*GameManager.Instance.GameOver();
				GameManager.Instance.PlayerMove = false;*/
			}
		}
	}
}
