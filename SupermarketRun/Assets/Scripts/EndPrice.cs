using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPrice : MonoBehaviour
{
    //Scipt que contienen los Premios finales, controla la rotación constante de este y las particulas que se activarán al obtenerlos

    public ParticleSystem[] particleEfects;
    private Transform price;

    private void Start()
    {
        price = transform.GetChild(0);
    }

    private void Update()
    {
       price.Rotate(50 * Time.deltaTime, 0 , 0 );
    }
    public IEnumerator GetingPrice()
    {
        particleEfects[0].Play();
        yield return new WaitForSeconds(3);
        particleEfects[1].Play();
        price.transform.GetChild(0).gameObject.SetActive(false);
        price.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        particleEfects[2].Play();
        yield return new WaitForSeconds(0.5f);
        particleEfects[3].Play();
        yield return new WaitForSeconds(0.5f);
        particleEfects[4].Play();
       // yield return new WaitForSeconds(0.5f);
     //   particleEfects[5].Play();
       // yield return new WaitForSeconds(0.5f);
        //particleEfects[6].Play();

        //Tras la activación d eparticulas, se llama la función para terminar el nivel
        GameManager.Instance.GameCompleted();

    }
}
