using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lvCompled : MonoBehaviour
{
    private bool countingPoints = false;
    [SerializeField] private GameObject flyingGuy;
    private int totalPoints;
    private int pointsCarBonus;
    private int productsObtained;
    private float sumTime;
    [SerializeField] private Text totalPonitsText;
    private Rigidbody[] RBs;
    [SerializeField] private Camera finishCam;

    //No utiliazado

    void Start()
    {
        RBs = flyingGuy.transform.GetComponentsInChildren<Rigidbody>();
        SetEnabled(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (countingPoints)
        {
            if (sumTime < totalPoints)
            {
                Debug.Log(totalPoints);
                sumTime += (Time.deltaTime * 15);
                int finalNum = (int)sumTime;
                Debug.Log(finalNum);
                totalPonitsText.text = finalNum.ToString();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.enabled = false;
            finishCam.enabled = true;
            GameManager.Instance.PlayerMove = false;
            totalPonitsText.GetComponent<Text>().enabled = true;
            pointsCarBonus = GameManager.Instance.pointsCarBonus;
            productsObtained = GameManager.Instance.productsObtained;
            totalPoints = pointsCarBonus * productsObtained;
            countingPoints = true;
            Rigidbody FlyGRB = flyingGuy.GetComponent<Rigidbody>();
            SetEnabled(true);
            GameManager.Instance.GameCompleted();
        }
    }
    void SetEnabled(bool enabled)
    {
        bool isKinematic = !enabled;
        foreach (Rigidbody rigidbody in RBs)
        {
            rigidbody.isKinematic = isKinematic;
            if(enabled)
            {
              rigidbody.AddForce(Vector3.up * totalPoints * 20, ForceMode.Acceleration);
               // rigidbody.AddExplosionForce(50, Vector3.up, 50f, 70f, ForceMode.Impulse);
            }
        }

    }


}
