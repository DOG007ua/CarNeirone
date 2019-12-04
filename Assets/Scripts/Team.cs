using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    int amountCar = 30;
    GameObject[] massCar;
    Color colorTeam;
    GeneticAlgoritm geneticAlgoritm;
    GameObject pointStart;
    public List<int> amountCarLifeList = new List<int>();       //количество машинок в живых на каждом поколении
    public int amountCarLife;                                   //колличество живых машинок сейчас
    int idTeam;
    GameObject carPrefab;
    GameObject teamPositionOnIerarhi;

    public GeneticAlgoritm GeneticAlgoritmObj
    {
        get { return geneticAlgoritm; }
        set { geneticAlgoritm = value; }
    }

    public Color ColorTeam
    {
        get { return colorTeam; }
    }

    public int ID
    {
        get { return idTeam; }
    }

    //canTake.GameObj.transform.SetParent(toTake.PositionTake.transform);
    public Team(Color color, int idTeam, GameObject carPrefab, GameObject pointStart)
    {
        teamPositionOnIerarhi = new GameObject();
        teamPositionOnIerarhi.transform.SetParent(GameObject.Find("Team").transform);
        teamPositionOnIerarhi.name = "Team" + (idTeam + 1);
        this.idTeam = idTeam;
        colorTeam = color;
        this.carPrefab = carPrefab;
        this.pointStart = pointStart;
        geneticAlgoritm = new GeneticAlgoritm();
        CreateMassCar();
    }

    void CreateMassCar()
    {
        massCar = new GameObject[amountCar];
        for (int i = 0; i < amountCar; i++)
        {
            massCar[i] = CreateCar(i, colorTeam);
            massCar[i].transform.SetParent(teamPositionOnIerarhi.transform);
        }
    }


    GameObject CreateCar(int id, Color color)
    {
        amountCarLife = amountCar;
        GameObject carCreate = Instantiate(carPrefab,
                               pointStart.transform.position,
                               pointStart.transform.rotation) as GameObject;
        Car carScript = carCreate.GetComponent<Car>();
        carScript.eventCollisionCar += CollisionFunction;
        carScript.id = id + 1;
        carScript.ColorTeam = color;
        carScript.TeamObj = this;
        return carCreate;
    }

    /// <summary>
    /// Его вызывает событие при столкновении машины
    /// </summary>
    /// <param name="car"></param>
    void CollisionFunction(Car car)
    {
        amountCarLife--;
        if (amountCarLife == 0)
        {
            NewGeneration();
        }
    }
    public void NewGeneration()
    {
        amountCarLifeList.Add(amountCarLife);
        amountCarLife = amountCar;
        Car[] car = new Car[amountCar];
        amountCarLife = amountCar;
        for (int i = 0; i < amountCar; i++)
        {
            car[i] = massCar[i].GetComponent<Car>();
        }
        CreateNewNeironNetWork(car);
        RestartPositionCar(massCar);
    }

    void CreateNewNeironNetWork(Car[] massCar)
    {
        NeironNetWork[] massNeiron = geneticAlgoritm.CreateNewCar(massCar);
        for (int i = 0; i < massCar.Length; i++)
        {
            massCar[i].NewGeneration(massNeiron[i]);
        }
    }

    void RestartPositionCar(GameObject[] massCar)
    {
        for (int i = 0; i < massCar.Length; i++)
        {
            massCar[i].transform.position = pointStart.transform.position;
            massCar[i].transform.rotation = pointStart.transform.rotation;
        }
    }

}
