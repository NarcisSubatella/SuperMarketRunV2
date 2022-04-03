using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopper : MonoBehaviour
{
	//Gestion de NPCs

	public enum ShopperType
	{
		CartShopper,
		StandingShopper,
		Nemesis
	}
	public ShopperType type;
	public float shopperSpeed;
	public bool pointisgiven = false;
	public bool hitted = false;
	private Animator animator;
	private Transform Pl;
	private float plSpeed;
	private float plSpeedPro;
	private float fallingSpeed=4;/*Relentizar la velocidad de caida para que se vean en pantalla*/

	[SerializeField]private GameObject shoppingcart;
	[SerializeField] private bool starting;
	[SerializeField] private float timeToStart =0;
	private float distancePl;

	[Header("Nemesis")]
	public int nemesisLife = 3;
	[SerializeField] private float pushForce;
	public Transform[] productsOnCart; /*Hay q colocar los productos del carro del NPC Nemesis */
	public int productsCount; /*Cantidad de veces que podemos quitarle productos */
	private bool stop = false;
	private Vector3 direction;


	void Start()
	{
		starting = false;
		if (type == ShopperType.CartShopper)
        {
		shoppingcart = transform.Find("ShoppingCart").gameObject; /*Encontrar su propio carro del NPC */
        }
		if(type == ShopperType.Nemesis)
        {
			Pl = GameObject.FindGameObjectWithTag("Player").transform;
			plSpeed = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<followPlayer>().speed;
			plSpeedPro = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<followPlayer>().speedProgress;
			nemesisLife = productsCount;
			ProductsControl();
		}
		animator = GetComponent<Animator>();
		StartCoroutine(waitToStart());
    }

	// Update is called once per frame
	void FixedUpdate()
	{
		if (type == ShopperType.CartShopper)
		{
			if (starting)
			{
				transform.Translate(new Vector3(0, 0, shopperSpeed) * Time.deltaTime);
			}
		}
		if (type == ShopperType.Nemesis)
        {
			if(starting && stop==false)
            {
				//recoge la distancia entre el Nemesis y el Player
				Vector3 playerPos = new Vector3(0, 0, Pl.position.z);
				Vector3 pos = new Vector3(0, 0, transform.position.z);
				distancePl = Vector3.Distance(playerPos, pos);

				if(nemesisLife>=1)
                {
					/*Si hay mucha distancia con el player, incrementa la velocidad del Nemesis */
					if(distancePl>0.2f )
					{
					transform.Translate(new Vector3(0, 0, (plSpeed*1.2f)) * Time.deltaTime);
					}
					//si hay poca distancia con el player, el Nemesis iguala la velocidad del player
					if (distancePl < 0.2f )
					{

					transform.Translate(new Vector3(0, 0, plSpeed) * Time.deltaTime);
					}
                }
				//si no tiene vida, animacion de caida y se destruye
				else
                {
					Destroy(transform.GetChild(0).gameObject, 4);
					transform.Translate(new Vector3(0, 0, fallingSpeed) * Time.deltaTime);
				}
			}
			else
            {
				transform.Translate(new Vector3(0, 0, 0));
			}
		}
		//activa la animacion de correr si la velocidad del nemesis es mayor de 0
		if(shopperSpeed <= 0)
        {
			animator.SetBool("Running", false);
        }
    }

	public void GivePoints()
	{
		//contaviliza los carros activos
		if(!pointisgiven)
		{
			if (type == ShopperType.CartShopper)
			{
				GameManager.Instance.pointsCarBonus += 1;
			}

		/*	else if (type == ShopperType.StandingShopper)
			{
				GameManager.Instance.pointsCarBonus += 1;
			}*/
			//followPlayer.Instance.BoastActivate(5);
			pointisgiven = true; /*âra que solo añada una vez al Int */
		}
		
	}
	// funcion que se activa al golpear NPCs
	public void HittedNPC()
    {
		if (type == ShopperType.CartShopper)
        {
		GetComponent<Animator>().SetTrigger("Hitted");/*Animacion caida*/
		Instantiate(GameManager.Instance.particleSys[1], (transform.position + new Vector3(0,1,0)), Quaternion.identity); /*instacia particulas */
		StartCoroutine(StopMoving());
		Destroy(this.gameObject, 2);
        }
		if(type == ShopperType.Nemesis)
        {
			hitted =true;
			Vector3 playerPosX = new Vector3(Pl.position.x, 0, 0);
			Vector3 posX = new Vector3(transform.position.x, 0, 0);
			direction = posX - playerPosX;
			Pl.GetComponent<PlayerControl>().StartCoroutine("HitedRecover");
			GameManager.Instance.productsObtained++; /*obtiene productos */
			//controlar la direccion q recive el golpe
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().productAnim.GetComponent<Animator>().SetTrigger("Take");
				if (direction.x<0)
				{
				animator.SetTrigger("NemesisHitetLeft");
				Pl.GetComponent<Rigidbody>().AddForce(Vector3.right * pushForce, ForceMode.Impulse);
				}
				else
				{
					animator.SetTrigger("NemesisHitetRight");
				Pl.GetComponent<Rigidbody>().AddForce((Vector3.right*-1) * pushForce, ForceMode.Impulse);
				}
			if(nemesisLife>1)
            {
				nemesisLife--; 
				productsCount--;
				ProductsControl();
            }
			else
            {
				GetComponent<Animator>().SetTrigger("Hitted");
				Instantiate(GameManager.Instance.particleSys[1], transform.position, Quaternion.identity);
				nemesisLife = 0;
				Destroy(this.gameObject, 3);
			}
		}
    }

	//tiempo de espera para activas un NPC
    private IEnumerator waitToStart()
    {
		yield return new WaitForSeconds(timeToStart);
		starting = true;
    }

	//controlña los productos activos
	private void ProductsControl()
    {
		for (int i = 0; i < productsOnCart.Length; i++)
		{
			if (i >= productsCount)
			{
				productsOnCart[i].gameObject.SetActive(false);
			}
			else
			{
				productsOnCart[i].gameObject.SetActive(true);
			}
		}
	}
	//para los Npc Al llegar al final de un segmento
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Segment2Start")&& type== ShopperType.CartShopper)
        {
			shopperSpeed = 0;
		}
		if (other.CompareTag("Segment3Start"))
		{
			shopperSpeed = 0;
			stop = true;
		}
	}
	//gestiona los tiempos de parada de los Npc
    private IEnumerator StopMoving()
    {
		shopperSpeed = fallingSpeed;
		shoppingcart.SetActive(false);
		yield return new WaitForSeconds(1);
		shopperSpeed = 0;
    }
}
