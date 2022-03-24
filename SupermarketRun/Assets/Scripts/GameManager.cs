using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField] GameObject LvManagerPrefab;
	
	[Header("PrincipalPanels")]
	public GameObject GameCompletedPanel;
	public GameObject GameOverPanel;
	public GameObject StartGamePanel;

	public bool PlayerMove;
	[HideInInspector]
	public bool isStartGame = true;
	public GameObject losigCartPrefab;
	public List<GameObject> shoppingCarts = new List<GameObject>();

	[Header("Points")]
	[SerializeField]private int coinsSaved;
	public int pointsCarBonus = 0;
	public int productsObtained = 0;
	[SerializeField] private Text globalCoints;
	private float totalPoints;
	[SerializeField]private float sumTime;
    [SerializeField]private float countinPointsSpeed;
	public GameObject cartToCase;
	private int finalNum;
	public ParticleSystem[] particleSys;
	[HideInInspector]
	public bool getPrice = false;
	private bool x2Points = false;
	

	[SerializeField] private Transform sCartsParent;
	[Header("NemesisSettings")]
	public GameObject nemesisPrefab;
	public Transform[] namesisTriguerLeft;
	public Transform[] namesisTriguerRight;
	[Header("Cameras")]
	public Camera[] cameras;
	[HideInInspector]
	public bool moveCamToSeg2 = false;
	[HideInInspector]
	public bool moveCamToEndPos = false;
	[HideInInspector]
	public bool moveCamToMMMPos = false;
	[SerializeField] Transform posCamSeg2;
	[SerializeField] Transform posCamEnd;
	[SerializeField] Transform posCameraMMM;
	[SerializeField] float speedCamMove =0.2f;

	[HideInInspector]
	public bool redy = false;

	[Header("InstaceCarts")]
	[SerializeField]private GameObject cartToInstace;
	[SerializeField]private Transform[] instaceCartsPoints;

	[Header("CanvasControl")]
	[SerializeField] GameObject canvas;
	[SerializeField] private GameObject menuPanel;
	public Image speddEffectImg;
	[SerializeField] private Transform endPos;
	[SerializeField] private Slider lvProgressSlider;
	private float checkDistanceToFisnish;
	private float totalDistanceToFinish;
	private Transform playerPos;
	[HideInInspector] public bool arrivedEndPos = false;
	[SerializeField] TMP_Text sliderActualLevel;
	[SerializeField] TMP_Text sliderNextLevel;
	[SerializeField] GameObject buttonGetCoin;
	[SerializeField] private Text cointx2;
	[SerializeField] private Text cointsTxt;
	[SerializeField] private Text lvCompleted;
	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		if(!GameObject.FindGameObjectWithTag("LvManager"))
        {
			Instantiate(LvManagerPrefab);
        }
	}
	// Start is called before the first frame update
	void Start()
    {
		//Inicializa el gammanager automaticamente si no se cambian los nombres
		GameCompletedPanel = GameObject.Find("GameCompleted");
		GameOverPanel= GameObject.Find("GameOver");
		StartGamePanel= GameObject.Find("StartGame");
		canvas = GameObject.Find("Canvas");
		menuPanel = GameObject.Find("MenuPanel");
		speddEffectImg = GameObject.Find("SpeedEffectImg").GetComponent<Image>();
		lvProgressSlider = GameObject.Find("SliderLvProgres").GetComponent<Slider>();
		sliderActualLevel = GameObject.Find("ActualLv").GetComponent<TMP_Text>();
		sliderNextLevel = GameObject.Find("NextlLv").GetComponent<TMP_Text>();
		buttonGetCoin = GameObject.Find("CoinsAnim");
		globalCoints = GameObject.Find("ProductPoints").GetComponent<Text>();
		cointx2 = GameObject.Find("X2CoinsTxt").GetComponent<Text>();
		cointsTxt = GameObject.Find("CoinsGainedTxt").GetComponent<Text>();
		lvCompleted = GameObject.Find("LevelCompletTxt").GetComponent<Text>();

		GameCompletedPanel.SetActive(false);
		GameOverPanel.SetActive(false);




		sliderActualLevel.text = LvManager.current.actualLevel.ToString();
		sliderNextLevel.text = (LvManager.current.actualLevel + 1).ToString();
		
		playerPos = GameObject.FindGameObjectWithTag("Player").transform;
		totalDistanceToFinish = Vector3.Distance(playerPos.position, endPos.position);
		Time.timeScale = 0;
		PlayerMove = false;
		menuPanel.SetActive(true);

		
		//Cargar monedas si hay una partida guardada
		if(LvManager.current.actualLevel!=1)
        {
			Debug.Log("coins");
			if (!PlayerPrefs.HasKey("CurrentPoints"))
			{
				PlayerPrefs.SetInt("CurrentPoints", 0);
				coinsSaved = PlayerPrefs.GetInt("CurrentPoints");
			}
			if (PlayerPrefs.HasKey("CurrentPoints"))
			{
				coinsSaved = PlayerPrefs.GetInt("CurrentPoints");
			}
        }

		globalCoints.text = coinsSaved.ToString();

		for (int i=0; i < sCartsParent.transform.childCount; i++ )
        {
		shoppingCarts.Add(sCartsParent.transform.GetChild(i).gameObject);
			if(shoppingCarts[i].GetComponent<ShoppingCartStake>().carIsEnabled == true)
            {
				pointsCarBonus++;
            }
        }
		if(instaceCartsPoints.Length!=0)
        {

			for(int i=0; i< instaceCartsPoints.Length;i++)
		    {
			StartCoroutine(InstaceCarts(i));
			
			}
			
        }
		
	}
	private IEnumerator InstaceCarts(int posCart)
    {
		yield return new WaitForSeconds(instaceCartsPoints[posCart].GetComponent<CartsAtributes>().waitToStart);
		GameObject instantiateCart = Instantiate(cartToInstace, instaceCartsPoints[posCart].position, instaceCartsPoints[posCart].rotation);
		Destroy(instantiateCart, instaceCartsPoints[posCart].GetComponent<CartsAtributes>().destroySpeed);
		yield return new WaitForSeconds(instaceCartsPoints[posCart].GetComponent<CartsAtributes>().aperarSpeed);
		StartCoroutine(InstaceCarts(posCart));
	}

    // Update is called once per frame
    void Update()
    {
		if(arrivedEndPos  ==false)
		{ 
		checkDistanceToFisnish = Vector3.Distance(playerPos.position, endPos.position);
		lvProgressSlider.value = 1-(checkDistanceToFisnish / totalDistanceToFinish);
		}
		else
        {
			speddEffectImg.enabled = false;
			lvProgressSlider.value = 1;

		}

		//cartBonusText.text = "X "+pointsCarBonus.ToString();
		//productsObtinedText.text = productsObtained.ToString();
		if(pointsCarBonus<=0)
        {
			GameOver();
        }

		if(moveCamToSeg2)
        {
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, posCamSeg2.position ,speedCamMove * Time.deltaTime);
			Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, posCamSeg2.rotation ,speedCamMove * Time.deltaTime);
			if(Camera.main.GetComponent<Camera>().fieldOfView <80)
            {
			Camera.main.GetComponent<Camera>().fieldOfView +=0.1f;
            }
			float distance = Vector3.Distance(Camera.main.transform.position, posCamSeg2.position);
			if(distance<=0.2f)
            {
				moveCamToSeg2 = false;
            }
        }

		if (moveCamToMMMPos)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, posCameraMMM.position, speedCamMove * Time.deltaTime);
			Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, posCameraMMM.rotation, speedCamMove * Time.deltaTime);
		}
			if (moveCamToEndPos)
        {
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, posCamEnd.position ,speedCamMove * Time.deltaTime);
			Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, posCamEnd.rotation ,speedCamMove * Time.deltaTime);
			if (Camera.main.GetComponent<Camera>().fieldOfView > 60)
			{
				Camera.main.GetComponent<Camera>().fieldOfView -= 0.1f;
			}
		}
		
	}

	public void GameCompleted()
	{
		canvas.GetComponent<Animator>().SetTrigger("EntCanvas");
		StartCoroutine(GameC());
	}

	public void GameOver()
	{
		StartCoroutine(GameG());
	}

	public void InstantGameOver()
	{
		Time.timeScale = 0;
		GameOverPanel.SetActive(true);
	}

	IEnumerator GameC()
	{

		yield return new WaitForSeconds(1.5f);
		cointsTxt.text = "+"+finalNum.ToString();
		cointx2.text = (finalNum * 2).ToString();
		lvCompleted.text = "LEVEL"+LvManager.current.actualLevel.ToString();
		GameCompletedPanel.SetActive(true);
	}

	IEnumerator GameG()
	{
		yield return new WaitForSeconds(0.5f);
		Time.timeScale = 0;
		GameOverPanel.SetActive(true);
	}

	public void StartGame()
	{
		canvas.GetComponent<Animator>().SetTrigger("StartCanvas");
		Time.timeScale = 1;
		menuPanel.SetActive(false);
		PlayerMove = true;
		isStartGame = false;
		StartGamePanel.SetActive(false);
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	public void CountingPoints(TextMesh totalPonitsText)
	{
		//totalPoints = (pointsCarBonus*1.5f) * productsObtained;
		float multiplicator =1;
		for(int i=1; i<= pointsCarBonus;i++)
        {
			multiplicator = multiplicator + 0.5f;
        }
			totalPoints = multiplicator * productsObtained;

		if (totalPoints <= 0)
		{
			totalPoints = 1;
		}
		if (sumTime < totalPoints)
		{
		sumTime += (Time.deltaTime * countinPointsSpeed);
		finalNum = (int)sumTime;
		totalPonitsText.text = finalNum.ToString()+"$";
		}
		else
        {
			totalPonitsText.text = finalNum.ToString() + "$";
			totalPonitsText.gameObject.GetComponent<Animator>().SetTrigger("Breath");
			redy = true;
		}


	}
	public IEnumerator SpeddEffectDecress()
    {
		yield return new WaitForSeconds(2f);

		Color newColor = speddEffectImg.color;
		while (speddEffectImg.color.a > 0.25f)
        {
			newColor.a -= (0.005f);
			speddEffectImg.color = newColor;
			yield return new WaitForSeconds(0.01f);
        }
    }

	public void GetCoin()
    {
		x2Points = false;
		buttonGetCoin.GetComponent<Animator>().SetTrigger("GetCoins");
		StartCoroutine(DisplayPoints());
    }
	public void GetCoinX2()
	{
		x2Points = true;
		buttonGetCoin.GetComponent<Animator>().SetTrigger("GetCoins");
		StartCoroutine(DisplayPoints());
	}
	private IEnumerator DisplayPoints()
    {
		yield return new WaitForSeconds(1f);
		if(x2Points)
        {
		coinsSaved += finalNum*2;
        }
		else
        {
		coinsSaved += finalNum;
        }
		globalCoints.text = coinsSaved.ToString(); 
		yield return new WaitForSeconds(1f);
		PlayerPrefs.SetInt("CurrentPoints", coinsSaved);
		LvManager.current.SceneChange();
	}
}
