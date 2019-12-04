using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameControl : MonoBehaviour 
{
    Team[] massTeam;
    int amountTeam = 5;

    public GameObject carPrefab;
    public GameObject pointStart;
    GameObject[] massCar;
    public GameObject startCameraPosition;

    bool cameraOnCar = false;
    bool mouseOnCar = false;

    InformationCar informationCar = new InformationCar();

    Car carOutCamera;                                           //ссылка на машину, на которой находится камера

    void Start()
    {
        CreateTeam();
    }
	
	void Update () {
        MouseInformation();
        OnMouseDown();
    }

    void CreateTeam()
    {        
        Color[] massColor = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };
        massTeam = new Team[amountTeam];
        for(int i = 0; i < amountTeam; i++)
        {
            massTeam[i] = new Team(massColor[i], i, carPrefab, pointStart);
        }
        massTeam[0].GeneticAlgoritmObj = new GeneticAlgoritm(0.5f, 0.1f);
        massTeam[1].GeneticAlgoritmObj = new GeneticAlgoritm(0.5f, 0.2f);
        massTeam[2].GeneticAlgoritmObj = new GeneticAlgoritm(0.5f, 0.3f);
        massTeam[3].GeneticAlgoritmObj = new GeneticAlgoritm(0.5f, 0.4f);
        massTeam[4].GeneticAlgoritmObj = new GeneticAlgoritm(0.5f, 0.5f);
    }

    public void NewGenerationAllTeam()
    {
        for (int i = 0; i < amountTeam; i++)
        {
            massTeam[i].NewGeneration();
        }
    }













    /*

    void CreateMassCar()
    {
        massCar = new GameObject[amountCar];
        for(int i = 0; i < amountCar; i++)
        {
            massCar[i] = CreateCar(i);
        }
    }

    GameObject CreateCar(int id)
    {
        amountCarLife = amountCar;
        GameObject carCreate = Instantiate(carPrefab,
                               pointStart.transform.position,
                               pointStart.transform.rotation) as GameObject;
        Car carScript = carCreate.GetComponent<Car>();
        carScript.eventCollisionCar += CollisionFunction;
        carScript.id = id + 1;
        return carCreate;
    }

    void CreateNewNeironNetWork(Car[] massCar)
    {        
        NeironNetWork[] massNeiron = geneticAlgoritm.CreateNewCar(massCar);
        for (int i = 0; i < massCar.Length; i++)
        {
            massCar[i].NewGeneration(massNeiron[i]);
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
    /// <summary>
    /// Перебросить все машины на старт
    /// </summary>
    /// <param name="massCar"></param>
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
    /// <summary>
    /// Его вызывает событие при столкновении машины
    /// </summary>
    /// <param name="car"></param>
    void CollisionFunction(Car car)
    {
        amountCarLife--;
        if(amountCarLife == 0)
        {
            NewGeneration();
        }
    }
    */
    void MouseInformation()
    {
        RaycastHit rayHit;
        Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out rayHit))
        {
            Car car = rayHit.collider.GetComponent(typeof(Car)) as Car;
            if (car != null)
            {
                mouseOnCar = true;

                informationCar.id = car.id;
                informationCar.delta = car.neironNetWork.delta;
                informationCar.speed = (float)Math.Round(car.Speed, 3);
                informationCar.rotate = (float)Math.Round(car.RotationReturn, 3);
            }
            else
            {
                mouseOnCar = false;
            }
        }        
    }

    bool MouseDownOnCar()
    {
        RaycastHit rayHit;
        Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit))
        {
            Car car = rayHit.collider.GetComponent(typeof(Car)) as Car;
            if (car != null)
            {
                carOutCamera = car;
                car.CameraOncar = true;
                Camera cam = Camera.allCameras[0];
                cam.transform.SetParent(car.cameraPosition.transform);
                cam.transform.position = car.cameraPosition.transform.position;
                cam.transform.rotation = car.cameraPosition.transform.rotation;
                cameraOnCar = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseDownOnCar();
        }
    }

    void ResetCameraPosition()
    {
        Camera cam = Camera.allCameras[0];
        cam.transform.SetParent(startCameraPosition.transform);
        cam.transform.position = startCameraPosition.transform.position;
        cam.transform.rotation = startCameraPosition.transform.rotation;
    }

    void OnGUI()
    {
        GUI.contentColor = Color.red;
        if (GUI.Button(new Rect(Screen.width / 2, 40, 150, 30), "Новое поколение"))
        {
            NewGenerationAllTeam();
        }
        GUI_GameInformation();



        if (mouseOnCar)
        {
            GUI_InformationCar();
        }

        if(cameraOnCar)
        {
            GUI_InCar();
        }
    }

    /*void GUI_GameInformation()
    {
        GUI.contentColor = Color.yellow;

        int top = 40;
        int left = 40;
        GUI.Box(new Rect(left, top, 150, 80), "Информация");
        GUI.Label(new Rect(left, top + 20, 150, 40), "Живых машин: " + amountCarLife);
        GUI.Label(new Rect(left, top + 40, 150, 40), "Мутация № " + geneticAlgoritm.NumberMutation);
        GUI.Label(new Rect(left, top + 60, 150, 40), "Шаг мутации " + Math.Round(geneticAlgoritm.StepMutationNow, 6));
    }*/

    void GUI_GameInformation()
    {
        int width = 150;
        int step = 10;
        for (int i = 0; i < amountTeam; i++)
        {
            GUI.contentColor = massTeam[i].ColorTeam;
            int top = 40;
            int left = 40 + (width + step )* i;
            GUI.Box(new Rect(left, top, 150, 80), "Команда №" + (massTeam[i].ID + 1));
            GUI.Label(new Rect(left + 3, top + 20, 150, 40), "Живых машин: " + massTeam[i].amountCarLife);
            GUI.Label(new Rect(left + 3, top + 40, 150, 40), "Мутация № " + massTeam[i].GeneticAlgoritmObj.NumberMutation);
            GUI.Label(new Rect(left + 3, top + 60, 150, 40), "Шаг мутации " + Math.Round(massTeam[i].GeneticAlgoritmObj.StepMutationNow, 6));
        }

       
    }

    void GUI_InCar()
    {
        GUI.contentColor = Color.green;

        if (GUI.Button(new Rect(Screen.width / 2, 80, 120, 50), "Сбросить камеру"))
        {
            ResetCameraPosition();
            cameraOnCar = false;
            carOutCamera.CameraOncar = false;
        }
    }

    void GUI_InformationCar()
    {
        GUI.contentColor = Color.green;

        GUI.Box(new Rect(Input.mousePosition.x + 30, Screen.height - Input.mousePosition.y, 150, 100), "Машина № " + informationCar.id);
        GUI.Label(new Rect(Input.mousePosition.x + 30, Screen.height - Input.mousePosition.y + 15, 150, 40), "Скорость: " + informationCar.speed);
        GUI.Label(new Rect(Input.mousePosition.x + 30, Screen.height - Input.mousePosition.y + 30, 150, 40), "Поворот: " + informationCar.rotate);
        GUI.Label(new Rect(Input.mousePosition.x + 30, Screen.height - Input.mousePosition.y + 45, 150, 40), "Delta: " + informationCar.delta);
    }
}

class InformationCar
{
    public int id;
    public float speed;
    public float rotate;
    public float delta;
}
