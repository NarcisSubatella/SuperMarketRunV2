using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
	//Vinculado al padre del PJ

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

	void Start()
    {
		internalSpeed = speed;
	}

    void FixedUpdate()
    {
		if(GameManager.Instance.PlayerMove)
		{
			if(arivedMMM == false)
            {
				if(IsBoost)/*aplica booster */
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
				//Progresion de la velocidad hasta llegar a la velocidad maxima
				if(speedProgress< speed)
				{
				speedProgress= speedProgress + 0.09f;
				}

				transform.Translate(new Vector3(0, 0, speedProgress) * Time.deltaTime);
			}
			//Desminucion progresiva de velociadad al llegar al MMM
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

	//Funcion del boost de velocidad
	public void BoastActivate()
	{
		IsBoost = true;
		internalTimerBoost = Boosttimer;
		speed = boost;
		GameManager.Instance.speddEffectImg.enabled = true;
		GetComponentInChildren<Animator>().speed = 3;
	}
}
