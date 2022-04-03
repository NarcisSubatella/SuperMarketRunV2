using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingCartStake : MonoBehaviour
{
   [SerializeField] public bool carIsEnabled;
   [SerializeField] private float puppingScale;

    [Header("OptionsToCase")]
    [SerializeField] private float cartToCaseSpeed = 1f;
    private GameObject cartToCaseInsta;
    private bool goToCase = false;
    private TextMesh caseText;
    private Vector3 casePos;
    private bool countingPoints = false;
    [Header("Select just first cart")]
    [SerializeField]private bool firstCart=false; /*Debe indicarse en el inspector cual es el ultimoi carro */
   
    private GameObject player;
    private ParticleSystem particleEnd;

    public List<GameObject> producsCar = new List<GameObject>(); /*productos de cada carro */


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        //Inicializa la lista de productos en el carro
        for (int i=0;i<transform.childCount;i++)
        {
            producsCar.Add(transform.GetChild(i).gameObject);
        }
        //Una vez inicializada la lista, los desacticva
        foreach (GameObject products in producsCar)
        {
            products.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Al contactar un NPC Shopper
        if (other.gameObject.CompareTag("Shopper")&& other.gameObject.GetComponent<Shopper>().pointisgiven==false)
        {
            //Ya no se utiliza, es para reletizar al Pj tras constactar a un NPC Shopper
           /* if (player.GetComponentInParent<followPlayer>().IsBoost==false)
            {
            player.GetComponentInParent<followPlayer>().speedProgress = 2;
            }*/

            Shopper shopper = other.gameObject.GetComponent<Shopper>();
            StakeShoppingCart(); /*Añade un carro al PJ*/
            shopper.GivePoints(); /*Contaviliza un carro añadido */
            other.GetComponent<Shopper>().HittedNPC(); /*Acciones ocurridas al NPC */

        }

        //Al contactar con un obstaculo
        if (other.gameObject.CompareTag("Obstacle"))
        {
            //player.GetComponentInParent<followPlayer>().speedProgress = 2;

            carIsEnabled = false; /*se desactiva el carro que contacto */
            Instantiate(GameManager.Instance.particleSys[2], (transform.position), Quaternion.identity); /*Efecto particulas*/
            GameObject cartInstance = Instantiate(GameManager.Instance.losigCartPrefab, transform.position, transform.rotation,transform); /*Se instacia un carro con la animación de perdida de carro */
            Destroy(cartInstance, 2); /*Se destruya el carro intanciado */
            GameManager.Instance.pointsCarBonus -= 1; /*Se resta 1 a la contavilidad de carros */
            ReconectAfterCollision(); /*Reconecta los carros activos restantes*/

            /*Game Over si se desactivca el ultimo carro*/
            if(GameManager.Instance.shoppingCarts[1].GetComponent<ShoppingCartStake>().carIsEnabled == false)
            {
              GameManager.Instance.GameOver();
            }
        }

        //Al contactar con un ittem colectable
        if (other.CompareTag("Product"))
        {
            other.GetComponent<Collider>().enabled = false;
            Instantiate(GameManager.Instance.particleSys[3], (transform.position), Quaternion.identity); /*Efevto partiuculas */
            GameManager.Instance.productsObtained++;/*Contaviliza el item*/
            Destroy(other.gameObject);
            Transform animationHolder = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().productAnim; /*Busca el contenedor de la animacion de recoleccion */

            animationHolder.GetComponent<Animator>().SetTrigger("Take"); /*Activa animación de recoger item */
            GetComponentInParent<ShoppingCarsHolder>().GetProducts(); /*Función de visualización de productos en el carro, se activa un item */
        }

        //Al llegar a la zona de cajas
        if (other.CompareTag("Case"))
        {
            
            caseText = other.transform.GetChild(1).GetComponent<TextMesh>();/*Recoge el texto para mostrar la puntuación*/
            countingPoints = true;
            other.gameObject.GetComponent<Collider>().enabled = false;
            carIsEnabled = false;/*Desactiva un carro*/
            cartToCaseInsta = Instantiate(GameManager.Instance.cartToCase, transform.position, transform.rotation, transform); /*Instacia un carro con la animacion de desplazamiento a la caja de cobro */
            particleEnd = other.GetComponentInChildren<ParticleSystem>(); /*Efecto particulas */

            if(firstCart == false)
            {
            
            cartToCaseInsta.transform.SetParent(null);

            }
            //si es el ultimo carro
            if(firstCart == true)
            {
            player.GetComponentInParent<followPlayer>().enabled = false;/*Para el PJ*/
            player.GetComponent<PlayerControl>().enabled = false;
            GameManager.Instance.moveCamToEndPos = true; /*Activa travelling camera */

            }
            goToCase = true;
            casePos = other.transform.GetChild(0).transform.position; /*Almacena la posicion dende devera de dirigirse el carro */
        }
        //Al llegar a la meta final
        if (other.CompareTag("EndPrice"))
        {
            caseText = other.transform.GetChild(1).GetComponent<TextMesh>();/*Recoge el texto para mostrar la puntuación*/
            countingPoints = true;
            other.gameObject.GetComponent<Collider>().enabled = false;
            carIsEnabled = false;
            cartToCaseInsta = Instantiate(GameManager.Instance.cartToCase, transform.position, transform.rotation, transform);
            particleEnd = other.GetComponentInChildren<ParticleSystem>();

            player.GetComponentInParent<followPlayer>().enabled = false;
            player.GetComponent<PlayerControl>().enabled = false;
            GameManager.Instance.moveCamToEndPos = true;
            goToCase = true;
            casePos = other.transform.GetChild(0).transform.position;
            GameManager.Instance.getPrice = true;
            StartCoroutine(other.GetComponentInParent<EndPrice>().GetingPrice()); /*Lanza todas las particulas y finaleiza el nivel */
        }
    }
    private void Update()
    {
        ActiveShoppingCart();

        //Envia los carros a las cajas correspondientes al llegar al MMM
        if(goToCase)
        {
            
            if(firstCart ==true && Vector3.Distance(cartToCaseInsta.transform.position, casePos) > 0.3f)
            {
            player.transform.position = Vector3.Lerp(player.transform.position, casePos, cartToCaseSpeed * Time.deltaTime);
            }
            if(firstCart == true && Vector3.Distance(cartToCaseInsta.transform.position, casePos) < 0.3f)
            {
                player.GetComponent<Animator>().SetTrigger("Stop");
                if(GameManager.Instance.redy == true && countingPoints ==true)
                {
                    countingPoints = false;
                    if(GameManager.Instance.getPrice == false)
                    {
                    Instantiate(GameManager.Instance.particleSys[0],caseText.transform.position, Quaternion.identity);
                    GameManager.Instance.GameCompleted();
                    }

                }

            }
            if (firstCart ==false && Vector3.Distance(cartToCaseInsta.transform.position,casePos)>0.2f)
            {
                cartToCaseInsta.transform.position = Vector3.Lerp(cartToCaseInsta.transform.position, casePos, cartToCaseSpeed * Time.deltaTime);
              if(Vector3.Distance(cartToCaseInsta.transform.position, casePos) < 0.5f)
                {
                    if(cartToCaseInsta.transform.childCount>0)
                    {
                    cartToCaseInsta.GetComponentInChildren<ParticleSystem>().Play();
                    GameObject particlesChild = cartToCaseInsta.transform.GetChild(0).gameObject;
                    Destroy(particlesChild, 0.5f);
                    }
                }
            }
        }
       if(countingPoints)
        {
           GameManager.Instance.CountingPoints(caseText);
        }
    }
    //Activa o desactiva el carro
    private void ActiveShoppingCart()
    {
    if(carIsEnabled)
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
        }
    else
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            foreach (GameObject products in producsCar)
            {
                products.SetActive(false);
            }
        }
    }
    //Añade un carro al PJ
    public void StakeShoppingCart()
    {
        List<GameObject> cartsActive = GameManager.Instance.shoppingCarts;
        for (int i = 0; i < cartsActive.Count; i++)
        {
           

            if (cartsActive[i].GetComponent<ShoppingCartStake>().carIsEnabled == false)
            {
                cartsActive[i].GetComponent<ShoppingCartStake>().carIsEnabled=true;
                cartsActive[i].GetComponent<Animator>().SetTrigger("start");
                break;
            }
            

        }
    }
    // En caso de perder algun carro que no sea el último, actualiza los carros activos para que no queden vacios en la cola de carros
    public void ReconectAfterCollision()
    {
        List<GameObject> cartsActive = GameManager.Instance.shoppingCarts;
        for (int i = 0; i < cartsActive.Count-1; i++)
        {
            if (cartsActive[i].GetComponent<ShoppingCartStake>().carIsEnabled == false && cartsActive[i+1].GetComponent<ShoppingCartStake>().carIsEnabled == true)
            {
                cartsActive[i].GetComponent<ShoppingCartStake>().carIsEnabled = true;
                cartsActive[i+1].GetComponent<ShoppingCartStake>().carIsEnabled = false;

            }

        }
    }
}
