using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Car : MonoBehaviour, IComparable
{

    public float[] distanceRay = new float[3];
    public float distance = 0;
    public float speed;
    public float[] neironParam = new float[2];

    float minSpeed = 0.3f;
    float maxSpeed = 1.5f;
    
    float maxAcceleretion = 0.03f;
    float maxAngleRotation = 0.2f;
    
    public bool stolknovenie = false;
    public NeironNetWork neironNetWork;
    Vector3[] vector = new Vector3[3];
    Color[] colorRay = new Color[3];

    public int coefSpeedGame = 1;

    public Rigidbody carPrefab;

    public List<float> listW = new List<float>();

    public event MyClass.CollisionCar eventCollisionCar;

    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
            if (speed < minSpeed) speed = minSpeed;
            if (speed > maxSpeed) speed = maxSpeed;
        }
    }

    public int CompareTo(object o)
    {
        Car p = o as Car;
        if (p != null)
            return this.distance.CompareTo(p.distance);
        else
            throw new Exception("Невозможно сравнить два объекта");
    }


    void Start()
    {
        colorRay[0] = new Color(255, 0, 0);
        colorRay[1] = new Color(0, 255, 0);
        colorRay[2] = new Color(0, 0, 255);
        speed = minSpeed;
        neironNetWork = new NeironNetWork();
    }

    void Update()
    {
        if (stolknovenie) return;
        CoefSpeedGame();
        CalculationDistanceRay();
        //KeyDown();
        WorkNeiron();
        Move();
        CalculationDistanceMove();
        SetList();
    }

    void CoefSpeedGame()
    {
        minSpeed = 0.3f * coefSpeedGame;
        maxSpeed = 1.5f * coefSpeedGame;

        maxAcceleretion = 0.03f * coefSpeedGame;
        maxAngleRotation = 0.2f * coefSpeedGame;
    }

    void SetList()
    {
        listW.Clear();
        for (int i = 0; i < neironNetWork.AmountNeironInLayer.Length - 1; i++)
        {
            for (int j = 0; j < neironNetWork.AmountNeironInLayer[i]; j++)
            {
                for (int k = 0; k < neironNetWork.MassNeiron[i][j].weight.Length; k++)
                {
                    listW.Add(neironNetWork.MassNeiron[i][j].weight[k]);
                }
            }
        }
    }

    /// <summary>
    /// работа нейрона
    /// </summary>
    void WorkNeiron()
    {        
        float[] massRez = neironNetWork.Start(distanceRay);
        ControlNeiron(massRez[0] - massRez[1], massRez[2] - massRez[3]);

        neironParam[0] = massRez[0] - massRez[1];
        neironParam[1] = massRez[3] - massRez[2];
    }

    void CalculationDistanceRay()
    {
        RaycastHit[] hit = new RaycastHit[3];
        Ray[] ray = new Ray[3];


        vector[0] = (Quaternion.AngleAxis(-30, Vector3.down) * transform.forward);
        vector[1] = (Quaternion.AngleAxis(0, Vector3.down) * transform.forward); 
        vector[2] = (Quaternion.AngleAxis(30, Vector3.down) * transform.forward) ;


        for (int i = 0; i < 3; i++)
        {
            ray[i] = new Ray(transform.position, vector[i]);
            if (Physics.Raycast(ray[i], out hit[i]) && hit[i].transform.gameObject.tag == "Finish")
            {
                Debug.DrawRay(transform.position, vector[i] * hit[i].distance, colorRay[i]);
                distanceRay[i] = hit[i].distance;
            }
        }
    }

    public void Move()
    {
        transform.position += transform.forward * Speed;
    }

    /// <summary>
    /// Пройденное расстояние
    /// </summary>
    void CalculationDistanceMove()
    {
        distance += speed * Time.deltaTime;
    }

    /// <summary>
    /// Ускорение
    /// </summary>
    /// <param name="acceleretion"></param>
    public void SetSpeed(float acceleretion)
    {
        Speed += acceleretion * Time.deltaTime;
    }

    public void Rotation(float angle)
    {
        transform.Rotate(Vector3.down * angle);
    }

    public void Rotation()
    {
        transform.Rotate(Vector3.down * maxAngleRotation);
    }

    /// <summary>
    /// Сюда нейрон передаёт ускорение скорости и поворота
    /// </summary>
    /// <param name="speed">скорость от -1 до 1</param>
    /// <param name="rotation">ускорение от -1 до 1</param>
    void ControlNeiron(float koefSpeed, float koefRotation)
    {
        SetSpeed(koefSpeed * maxAcceleretion);
        Rotation(koefRotation * maxAngleRotation);
    }

    void KeyDown()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SetSpeed(maxAcceleretion);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SetSpeed(-maxAcceleretion);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Rotation(maxAngleRotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rotation(-maxAngleRotation);
        }
    }

    void OnTriggerEnter(Collider myCollision)
    {
        if(myCollision.tag == "Finish")
        {
            Stloknovenie();
        }
        
    }

    void Stloknovenie()
    {

        speed = 0;
        stolknovenie = true;
        carPrefab.GetComponent<MeshRenderer>().enabled = false;
        carPrefab.GetComponent<BoxCollider>().enabled = false;
        eventCollisionCar();
    }    
}

public class NeironParam
{
    public float koefSpeed;
    public float koefRotation;
}

