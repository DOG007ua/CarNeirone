using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameControl : MonoBehaviour {

    int amountCar = 200;
    int amountLifeCar;
    public GameObject carPrefab;
    public GameObject pointStart;
    GameObject[] massCar;
    GeneticAlgoritm geneticAlgoritm;
    public int coefSpeedGame = 1;
    public int amountCarLife;



    void Start () {

        CreateMassCar();
        geneticAlgoritm = new GeneticAlgoritm(amountCar);
    }
	
	void Update () {
		
	}

    void CreateMassCar()
    {
        massCar = new GameObject[amountCar];
        for(int i = 0; i < amountCar; i++)
        {
            massCar[i] = CreateCar();
        }
    }

    GameObject CreateCar()
    {
        amountLifeCar = amountCar;
        GameObject carCreate = Instantiate(carPrefab,
                               pointStart.transform.position,
                               pointStart.transform.rotation) as GameObject;
        Car carScript = carCreate.GetComponent<Car>();
        carScript.eventCollisionCar += CollisionFunction;
        return carCreate;
    }

    void CreateNewNeironNetWork(Car[] massCar)
    {        
        NeironNetWork[] massNeiron = geneticAlgoritm.CreateNewCar(massCar);
        for (int i = 0; i < massCar.Length; i++)
        {
            massCar[i].GetComponent<Car>().neironNetWork = massNeiron[i];
            massCar[i].distance = 0;
            massCar[i].stolknovenie = false;
            massCar[i].carPrefab.GetComponent<MeshRenderer>().enabled = true;
            massCar[i].carPrefab.GetComponent<BoxCollider>().enabled = true;
            massCar[i].coefSpeedGame = coefSpeedGame;
        }
    }

    public void NewGeneration()
    {
        amountCarLife = amountCar;
        Car[] car = new Car[amountCar];
        amountLifeCar = amountCar;
        for (int i = 0; i < amountCar; i++)
        {
            car[i] = massCar[i].GetComponent<Car>();            
        }
        CreateNewNeironNetWork(car);
        RestartPositionCar(massCar);
    }

    void RestartPositionCar(GameObject[] massCar)
    {
        for (int i = 0; i < massCar.Length; i++)
        {
            massCar[i].transform.position = pointStart.transform.position;
            massCar[i].transform.rotation = pointStart.transform.rotation;
        }
    }

    void DeleteCar()
    {
        for(int i = 0; i < massCar.Length; i++)
        {
            Car carScript = massCar[i].GetComponent<Car>();
            carScript.eventCollisionCar -= CollisionFunction;
            Destroy(massCar[i].gameObject);
        }
    }

    void CollisionFunction()
    {
        int amountStolnovenie = 0;
        for (int i = 0; i < amountCar; i++)
        {
            Car car = massCar[i].GetComponent<Car>();
            if (car.stolknovenie)
            {
                amountStolnovenie++;
            }
            amountCarLife = amountCar - amountStolnovenie;
        }
        if (amountStolnovenie == amountCar)
        {
            NewGeneration();
        }

    }

    void OnGUI()
    {

        /*GUI.contentColor = Color.red;
        Car car = massCar[0].GetComponent<Car>();
        for (int i = 0; i < car.neironNetWork.AmountNeironInLayer.Length; i++)        
        {
            for (int j = 0; j < car.neironNetWork.AmountNeironInLayer[i]; j++)
            {
                float v = ((massCar[0].GetComponent<Car>().neironNetWork.MassNeiron[i][j])).OutValue;
                GUI.Label(new Rect(100 + i * 100, 50 + j * 30, 50, 30), Math.Round(v, 3).ToString());
            }
        }*/
        GUI.contentColor = Color.red;
        if (GUI.Button(new Rect(Screen.width / 2, 40, 100, 30), "Рестарт"))
        {
            NewGeneration();
        }
        //double val = massCar[0].GetComponent<Car>().neironNetWork[]


        //GUI.Label(new Rect(Screen.width - 50, 10, 40, 40), money.ToString());
    }


}
