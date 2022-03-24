using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingCartStake : MonoBehaviour
{
   [SerializeField] public bool carIsEnabled;
   [SerializeField] private float puppingScale;
    //[SerializeField] private int springStaking;
    // [SerializeField] private int damperSpring;
    [Header("OptionsToCase")]
    [SerializeField] private float cartToCaseSpeed = 1f;
    private GameObject cartToCaseInsta;
    private bool goToCase = false;
    [SerializeField]private Vector3 casePos;
    [SerializeField]private TextMesh caseText;
    private bool countingPoints = false;
    [Header("Select just first cart")]
    [SerializeField]private bool firstCart=false;
   [SerializeField] private GameObject player;
   [SerializeField] private ParticleSystem particleEnd;
    public List<GameObject> producsCar = new List<GameObject>();


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        for (int i=0;i<transform.childCount;i++)
        {
            producsCar.Add(transform.GetChild(i).gameObject);
        }
        foreach (GameObject products in producsCar)
        {
            products.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (other.gameObject.CompareTag("Shopper")&& other.gameObject.GetComponent<Shopper>().pointisgiven==false)
        {
            if (player.GetComponentInParent<followPlayer>().IsBoost==false)
            {
            player.GetComponentInParent<followPlayer>().speedProgress = 2;
            }
            //StartCoroutine(player.GetComponent<PlayerControl>().PushedPj());
            Shopper shopper = other.gameObject.GetComponent<Shopper>();
            StakeShoppingCart();
            shopper.GivePoints();
            other.GetComponent<Shopper>().HittedNPC();

        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            carIsEnabled = false;
            player.GetComponentInParent<followPlayer>().speedProgress = 2;
            Instantiate(GameManager.Instance.particleSys[2], (transform.position), Quaternion.identity);
            GameObject cartInstance = Instantiate(GameManager.Instance.losigCartPrefab, transform.position, transform.rotation,transform);
            Destroy(cartInstance, 2);
            GameManager.Instance.pointsCarBonus -= 1;
            ReconectAfterCollision();


            if(GameManager.Instance.shoppingCarts[1].GetComponent<ShoppingCartStake>().carIsEnabled == false)
            {
              GameManager.Instance.GameOver();
            }
        }
        if (other.CompareTag("Product"))
        {
            Debug.Log("Touch");
            other.GetComponent<Collider>().enabled = false;
            Instantiate(GameManager.Instance.particleSys[3], (transform.position), Quaternion.identity);
            GameManager.Instance.productsObtained++;
            Destroy(other.gameObject);
            Transform animationHolder = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().productAnim;

            //GameObject product = Instantiate(other.gameObject, animationHolder ,animationHolder.position,animationHolder.rotation);
          //  product.transform.SetParent(animationHolder);
            animationHolder.GetComponent<Animator>().SetTrigger("Take");
            GetComponentInParent<ShoppingCarsHolder>().GetProducts();
        }
        if (other.CompareTag("Case"))
        {
            caseText = other.transform.GetChild(1).GetComponent<TextMesh>();
            countingPoints = true;
            other.gameObject.GetComponent<Collider>().enabled = false;
            carIsEnabled = false;
            cartToCaseInsta = Instantiate(GameManager.Instance.cartToCase, transform.position, transform.rotation, transform);
            particleEnd = other.GetComponentInChildren<ParticleSystem>();

            if(firstCart == false)
            {
            
            cartToCaseInsta.transform.SetParent(null);
           //particle = other.transform.GetChild(2).GetComponent<ParticleSystem>();

            }

            if(firstCart == true)
            {
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponentInParent<followPlayer>().enabled = false;
            player.GetComponent<PlayerControl>().enabled = false;
            GameManager.Instance.moveCamToEndPos = true;

            Debug.Log(player.gameObject.name);
            }
            goToCase = true;
            casePos = other.transform.GetChild(0).transform.position;
        }
        if (other.CompareTag("EndPrice"))
        {
            caseText = other.transform.GetChild(1).GetComponent<TextMesh>();
            countingPoints = true;
            other.gameObject.GetComponent<Collider>().enabled = false;
            carIsEnabled = false;
            cartToCaseInsta = Instantiate(GameManager.Instance.cartToCase, transform.position, transform.rotation, transform);
            particleEnd = other.GetComponentInChildren<ParticleSystem>();

        /*    if(firstCart == false)
            {
            cartToCaseInsta.transform.SetParent(null);
           //particle = other.transform.GetChild(2).GetComponent<ParticleSystem>();

            }*/

            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponentInParent<followPlayer>().enabled = false;
            player.GetComponent<PlayerControl>().enabled = false;
            GameManager.Instance.moveCamToEndPos = true;
            goToCase = true;
            casePos = other.transform.GetChild(0).transform.position;
            GameManager.Instance.getPrice = true;
            StartCoroutine(other.GetComponentInParent<EndPrice>().GetingPrice());
        }
    }
    private void Update()
    {
        ActiveShoppingCart();
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
    private void ActiveShoppingCart()
    {
    if(carIsEnabled)
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
           /* foreach (GameObject products in producsCar)
            {
                products.SetActive(true);
            }*/
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
