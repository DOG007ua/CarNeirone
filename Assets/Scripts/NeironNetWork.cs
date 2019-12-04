using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MyRand = UnityEngine.Random;

public class NeironNetWork : IComparable, ICloneable
{
    int[] amountNeironInLayer = { 3, 5, 5, 2 };   //Количество слоёв и нейронов в каждом слое
    int amountLayer = 0;
    Neiron[][] massNeiron;               //кол. слоев\\//№ нейрона
    public float delta = 0;    

    public int[] AmountNeironInLayer { get { return amountNeironInLayer; } }
    public NeironNetWork()
    {
        amountLayer = amountNeironInLayer.Length;
        CreateNeiron();
    }

    int IComparable.CompareTo(object o)
    {
        NeironNetWork temp = (NeironNetWork)o;
        if (this.AbsError > temp.AbsError) return 1;
        if (this.AbsError < temp.AbsError) return -1;
        else return 0;
    }

    /// <summary>
    /// Для обратного распространения ошибки
    /// </summary>
    public float AbsError { get; set; }

    public Neiron[][] MassNeiron
    {
        get { return massNeiron; }
        set { massNeiron = value; }
    }

    /// <summary>
    /// Начало работы сети 
    /// </summary>
    /// <param name="inputValue">входные данные</param>
    /// <returns></returns>
    public float[] Start(float[] inputValue)
    {
        //наполнение входными значениями первого слоя
        for (int i = 0; i < inputValue.Length; i++)
        {
            ((NeironInFirstLayer)massNeiron[0][i]).inputValue = inputValue[i];
        }
        //Активация нейронов скрытого слоя
        for (int numberLayer = 0; numberLayer < amountLayer; numberLayer++)
        {
            for (int i = 0; i < amountNeironInLayer[numberLayer]; i++)
            {
                massNeiron[numberLayer][i].Start();
            }
        }
        //наполнение выходного массива значениями выходного слоя
        float[] rezult = new float[amountNeironInLayer[amountLayer - 1]];
        for (int i = 0; i < amountNeironInLayer[amountLayer - 1]; i++)
        {
            rezult[i] = massNeiron[amountLayer - 1][i].OutValue;
        }
        return rezult;
    }
    /// <summary>
    /// Обучение обратным распростанением ошибки
    /// </summary>
    /// <param name="needValue">Передаётся нужные значения для выходного слоя</param>
    public void Training(float[] needValue)
    {
        for (int i = 0; i < amountNeironInLayer[amountLayer - 1]; i++)
        {
            ((NeironInLastLayer)massNeiron[amountLayer - 1][i]).outValueIdeal = needValue[i];
        }

        for (int numberLayer = amountLayer - 1; numberLayer >= 0; numberLayer--)
        {
            for (int i = 0; i < amountNeironInLayer[numberLayer]; i++)
            {
                massNeiron[numberLayer][i].Training();
            }
        }
    }
    /// <summary>
    /// Создание нейронов
    /// </summary>
    public void CreateNeiron()
    {
        massNeiron = new Neiron[amountNeironInLayer.Length][];
        massNeiron[amountLayer - 1] = new NeironInLastLayer[amountNeironInLayer[amountLayer - 1]];
        for (int i = 0; i < amountLayer; i++)
        {
            massNeiron[i] = new Neiron[amountNeironInLayer[i]];
        }

        for (int i = 0; i < amountNeironInLayer[amountLayer - 1]; i++)          //последний слой
        {
            massNeiron[amountLayer - 1][i] = new NeironInLastLayer(amountNeironInLayer[amountLayer - 2]);
        }

        for (int i = 0; i < amountNeironInLayer[0]; i++)          //первый слой
        {
            massNeiron[0][i] = new NeironInFirstLayer(massNeiron[1], i);
        }

        for (int numberLayer = 1; numberLayer < amountLayer - 1; numberLayer++) //скрытые слои
        {
            for (int i = 0; i < amountNeironInLayer[numberLayer]; i++)
            {
                massNeiron[numberLayer][i] = new Neiron(massNeiron[numberLayer + 1], amountNeironInLayer[numberLayer - 1], i);
            }
        }
    }

    public object Clone()
    {
        NeironNetWork netWork = new NeironNetWork();
        for (int i = 0; i < massNeiron.Length; i++)
        {
            for (int j = 0; j < massNeiron[i].Length; j++)
            {
                netWork.massNeiron[i][j].weight = massNeiron[i][j].weight;
            }
        }
        return netWork;
    }
}





