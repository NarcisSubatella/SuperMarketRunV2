using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
	public static followPlayer Instance;
	public float speed;
	public float internalSpeed;
	public float speedSegment2;
	public float speedMMM;
	public float boost = 0;
	public float Boosttimer;
	private float internalTimerBoost;
	public bool IsBoost = false;
	public float speedProgress = 0;
	[HideInInspector]
	public bool arivedMMM;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}
	// Start is called before the first frame update
	void Start()
    {
		internalSpeed = speed;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if(GameManager.Instance.PlayerMove)
		{
			if(arivedMMM == false)
            {
				if(IsBoost)
                {
					if (internalTimerBoost > 0)
					{
						internalTimerBoost -= Time.deltaTime;
					}
					else if (internalTimerBoost <= 0)
					{
					
						speedProgress = internalSpeed;
						speed = internalSpeed;

						GameManager.Instance.speddEffectImg.enabled = false;
						GetComponentInChildren<Animator>().speed = 1;
						IsBoost = false;

					}
				}
				if(speedProgress< speed)
				{
				speedProgress= speedProgress + 0.09f;
				}

				transform.Translate(new Vector3(0, 0, speedProgress) * Time.deltaTime);
			}
			if (arivedMMM == true)
			{
				if (speedProgress > speed)
				{
					speedProgress = speedProgress - 0.09f;
				}
				transform.Translate(new Vector3(0, 0, speedProgress) * Time.deltaTime);
			}

		}
				
	}


	public void BoastActivate()
	{
		IsBoost = true;
		internalTimerBoost = Boosttimer;
		speed = boost;
		GameManager.Instance.speddEffectImg.enabled = true;
		GetComponentInChildren<Animator>().speed = 3;
	}

	IEnumerator BoastReset()
	{
		yield return new WaitForSeconds(0.5f);
		//boast = 0;
	}
}
