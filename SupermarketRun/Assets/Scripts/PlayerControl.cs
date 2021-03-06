using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	//Script vinculado al PJ

	public static PlayerControl Instance;
	public float movespeed = 0.05f;
	public Animator shopper;
	public Transform productAnim;
	[SerializeField] Transform nemesisRespawnL;
	[SerializeField] Transform nemesisRespawnR;
	[SerializeField] private bool hitted = false;
	[SerializeField] private float hittedRecoverd;
	private followPlayer Player;

	private bool getingToMMMPos = false;
	[SerializeField]	private float MmmXPos; /*Getting X pos to center PJ pos on MMM */


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}
	void Start()
    {
		hitted = false;
		Player = GetComponentInParent<followPlayer>();

	}

    // Update is called once per frame
    void FixedUpdate()
    {
		//Desplazamiento en la horizontal por teclado
		if(Input.GetKey(KeyCode.A)&& hitted==false)
		{
			transform.Translate(new Vector3(-movespeed, 0, 0) * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.D) && hitted == false)
		{
			transform.Translate(new Vector3(movespeed, 0, 0) * Time.deltaTime);
		}

		//Desplazamiento en la horizonatal por contacto o raton
		if (Input.touchCount > 0 && GameManager.Instance.PlayerMove && hitted==false)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Moved)
			{
				transform.position = new Vector3(
				   transform.position.x + (touch.deltaPosition.x * movespeed * Time.deltaTime),
				   transform.position.y,
				   transform.position.z);
			}
		}

		if(!GameManager.Instance.PlayerMove && !GameManager.Instance.isStartGame)
		{
			//Activa animacion de correr
			shopper.SetFloat("run", 1);
		}
		 //lo puso el programador anterior, creo que no se uas. por lo que entiendo, hace Game Over si se midifica la altura del PJ
		if(transform.position.y < -1f)
		{
			GameManager.Instance.InstantGameOver();
		}
		//coloca el PJ en la posicion central y ajusta la velocidad al llegar al MMM
		if(getingToMMMPos ==true)
        {
			transform.position = Vector3.Lerp(transform.position, (new Vector3(0, transform.position.y, transform.position.z)),1.5f*Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		//Speed boster tras contactar con el booster
		if(other.gameObject.CompareTag("SpeedBooster"))
        {
			Player.BoastActivate();
        }

		//Detrmina donde aparecera el NPC Nemesis en funcion del trigger que se contacte
		if (other.gameObject.CompareTag("NemesisRespawnR"))
        {
			Instantiate(GameManager.Instance.nemesisPrefab.gameObject, nemesisRespawnL.position, Quaternion.identity);
        }
		if (other.gameObject.CompareTag("NemesisRespawnL"))
        {
			Instantiate(GameManager.Instance.nemesisPrefab.gameObject, nemesisRespawnR.position, Quaternion.identity);
		}

		//Activa funciones correspondientes al chocar con los NPC
		if (other.gameObject.CompareTag("Nemesis") && other.gameObject.GetComponent<Shopper>().hitted == false /*&& other.gameObject.GetComponent<Shopper>().nemesisLife > 0*/)
		{
			other.GetComponent<Shopper>().HittedNPC();
		}

		//Activa la aparicion de coches obstaculo
		if(other.CompareTag("CartTrigger"))
        {
			other.GetComponentInChildren <Animator>().SetTrigger("CartStart");
        }

		//Activa componentes del segmento 2
		if (other.CompareTag("Segment2Start"))
		{
			GameManager.Instance.moveCamToSeg2=true;
			Player.speed *= Player.speedSegment2;
			GameManager.Instance.speddEffectImg.enabled = true;
			StartCoroutine(GameManager.Instance.SpeddEffectDecress());

		}

		//Activa componentes del segmento 3, MMM
		if (other.CompareTag("Segment3Start"))
        {
			getingToMMMPos = true;
			movespeed = 0;
			MmmXPos = other.transform.position.x;
			followPlayer Player = GetComponentInParent<followPlayer>();
			Player.arivedMMM = true;
			Player.speed = Player.speedMMM;
			GameManager.Instance.arrivedEndPos = true;
			GameManager.Instance.moveCamToMMMPos = true;
		}

	}
	//tempo de espera para poder golpear de nuevo a un NPC Nemesis
    public IEnumerator HitedRecover()
    {
		hitted = true;
		yield return new WaitForSeconds(hittedRecoverd);
		hitted = false;
    }
	/*public IEnumerator PushedPj()
    {
		Debug.Log("Empujon");
		GetComponent<Rigidbody>().AddForce((Vector3.forward * -1) * 1.3f, ForceMode.Impulse);
		yield return new WaitForSeconds(2);
		pushed = false;
	}*/
}
