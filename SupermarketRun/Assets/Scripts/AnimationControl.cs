using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    [Header("End get coins animation")]
    private bool animGetCoins = false;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform buttonGetCoin;
    [SerializeField] private Transform coinsContadorPos;
    [SerializeField] private float speedMoveCoins = 0.2f;
    private int coinsInstanciate;
    GameObject coinInstance;

    //No utilizado

    private void Update()
    {
        if(animGetCoins)
        {
           // GameObject coinInstance = Instantiate(coinPrefab, buttonGetCoin.position, Quaternion.identity);
            coinInstance.transform.position = Vector3.Lerp(buttonGetCoin.position, coinsContadorPos.position, speedMoveCoins * Time.deltaTime);
        }
    }

    public void GetCurrencyButton()
    {
        animGetCoins = true;
        StartCoroutine(InstaceCoins());
    }
    private IEnumerator InstaceCoins()
    {
        coinInstance = Instantiate(coinPrefab, buttonGetCoin.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        if (coinsInstanciate < 10)
        {
            StartCoroutine(InstaceCoins());
        }
        else
        {
            StopCoroutine(InstaceCoins());
        }
    }

}
