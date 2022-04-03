using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingCarsHolder : MonoBehaviour
{
    //metodo para vidualizar los items de los carros al contactar con un producto
    public void GetProducts()
    {
        //Inicializa la lista con los carros activos
        List<GameObject> carsActive = new List<GameObject>();
        foreach (GameObject shoppingCars in GameManager.Instance.shoppingCarts)
        {
            if(shoppingCars.GetComponent<ShoppingCartStake>().carIsEnabled)
            {
                carsActive.Add(shoppingCars);
            }
        }

        List<GameObject> productsCar = new List<GameObject>();

//activa el primer item de cada carro antes de poder activar el segundo item
            int a = 0;
        for (int i=0; i< carsActive.Count;i++)
        {
         
            if(a<carsActive[i].GetComponentInChildren<ShoppingCartStake>().producsCar.Count-1)
            {
                if (!carsActive[i].GetComponentInChildren<ShoppingCartStake>().producsCar[a].activeSelf)
                {
                    carsActive[i].GetComponentInChildren<ShoppingCartStake>().producsCar[a].SetActive(true);
                Debug.Log(i);
                    break;
                }
                if(carsActive[carsActive.Count-1].GetComponentInChildren<ShoppingCartStake>().producsCar[a].activeSelf)
                {
                    a++;
                    i = 0;
                }
            }
           

         
        }

    }
   
}
