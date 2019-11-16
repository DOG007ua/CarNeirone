using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using MyRand = UnityEngine.Random;

public class GeneticAlgoritm
{
    float stepMutationMax;
    float stepMutationNow;
    float mutationProbability;
    int numberMutation = 0;
    public int NumberMutation
    {
        get { return numberMutation; }
        set
        {
            numberMutation = value;
        }
    }

    public float StepMutationNow { get => stepMutationNow; set => stepMutationNow = value; }

    //public static float stepMutationPublic = 0;

    public GeneticAlgoritm()
    {
        stepMutationMax = 0.5f;
        mutationProbability = 0.1f;
    }

    public GeneticAlgoritm(float stepMutationMax, float mutationProbability)
    {
        this.stepMutationMax = stepMutationMax;
        this.mutationProbability = mutationProbability;
    }
    /// <summary>
    /// Беру из машин нейронную сеть
    /// </summary>
    /// <param name="massCar"></param>
    /// <returns></returns>
    NeironNetWork[] ReturnNeironNetWorkCar(Car[] massCar)
    {
        NeironNetWork[] massNeironNetWork = new NeironNetWork[massCar.Length];        
        for (int i = 0; i < massCar.Length; i++)
        {
            massNeironNetWork[i] = massCar[i].neironNetWork;
        }
        return massNeironNetWork;
    }
    /// <summary>
    /// Проверка на то, что нейронная сеть не развивается и нужно её сбросить
    /// </summary>
    /// <param name="massCar"></param>
    /// <returns>true - если нужно перезагрузить сеть</returns>
    bool NeedRestartGeneration(Car[] massCar)
    {
        int maxLap = 0;
        foreach (Car car in massCar)
        {
            if (car.numberLap > maxLap) maxLap = car.numberLap;
        }
        if (numberMutation == 12 && maxLap < 1) return true;
        if (numberMutation == 20 && maxLap < 2) return true;
        return false;
    }
    /// <summary>
    /// Перезагрузка нейронной сети
    /// </summary>
    /// <param name="neironNetWork"></param>
    void RestartGeneration(NeironNetWork[] neironNetWork)
    {
        NumberMutation = 0;
        foreach (NeironNetWork neiron in neironNetWork)
        {
            neiron.CreateNeiron();
        }
    }

    /// <summary>
    /// Отбор машин
    /// </summary>
    /// <param name="massCar"></param>
    /// <returns></returns>
    public NeironNetWork[] CreateNewCar(Car[] massCar)
    {
        int amountCar = massCar.Length;
        NeironNetWork[] massNeironNetWorkResult = new NeironNetWork[amountCar];     //тут будет новая нейронная сеть
        Array.Sort(massCar);                                                        //соритрую машины по пройденной дистанции
        NeironNetWork[] massNeironNetWorkBuffer = ReturnNeironNetWorkCar(massCar);
        if (NeedRestartGeneration(massCar))
        {
            RestartGeneration(massNeironNetWorkBuffer);
            return massNeironNetWorkBuffer;
        }
        numberMutation++;
        int amountBestParentsTop = (int)(amountCar * 0.40f);                    //Кооличество машин в каждой группе
        int amountBestParentsTop2 = (int)(amountCar * 0.35f);
        //int amountBestParentsTop5 = (int)(maxCar * 0.10f);
        int amountBestParentsTop3 = amountCar - amountBestParentsTop2 - amountBestParentsTop;


        //Первым параметром передаю новый массив НС, созданной на основании лучший родителей
        //Вторым - отступ в данном массиве
        //Третим - массив, в который записываю новую НС
        //Отступ в новой НС
        Array.Copy(BestParent(massNeironNetWorkBuffer, amountBestParentsTop, 1), 0, massNeironNetWorkResult, 0, amountBestParentsTop);
        Array.Copy(BestParent(massNeironNetWorkBuffer, amountBestParentsTop2, 2), 0, massNeironNetWorkResult, amountBestParentsTop, amountBestParentsTop2);
        Array.Copy(BestParent(massNeironNetWorkBuffer, amountBestParentsTop3, 3), 0, massNeironNetWorkResult, amountBestParentsTop + amountBestParentsTop2, amountBestParentsTop3);
        //Array.Copy(BestParent(massNeironNetWorkBuffer, amountBestParentsTop6, 6), 0, massNeironNetWorkResult, amountBestParentsTop + amountBestParentsTop3 + amountBestParentsTop5, amountBestParentsTop6);
        return massNeironNetWorkResult;
    }
    /// <summary>
    /// Выбираю из лучших случайного родителя и создаю на основании него новую нейронную сеть
    /// </summary>
    /// <param name="massNeironNetWork">НС на основании которой нужно создать новую НС</param>
    /// <param name="amountReturn">Колличество НС в данной группе</param>
    /// <param name="amountBestParent">Колличество лучших родителей</param>
    /// <returns></returns>
    NeironNetWork[] BestParent(NeironNetWork[] massNeironNetWork, int amountReturn, int amountBestParent)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];
        for (int i = 0; i < amountReturn; i++)
        {
            int pos = MyClass.rand.Next(0, amountBestParent);
            returnNeiron[i] = Mutation(massNeironNetWork[pos]);
            //returnNeiron[i].id = i;
            //returnNeiron[i].idParent = massNeironNetWork[pos].id;
            //returnNeiron[i].placeParent = massNeironNetWork[pos].place;
        }
        return returnNeiron;
    }

    //Заготовка для перемешивания весов лучших родителей
    NeironNetWork[] BestParentsKrosoving(NeironNetWork[] massNeironNetWork, int amountReturn)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];


        for (int i = 0; i < amountReturn; i++)
        {
            returnNeiron[i] = ExchangeWeigth(massNeironNetWork);
        }
        return returnNeiron;
    }
    //Заготовка для перемешивания весов лучших родителей
    NeironNetWork[] WinnersParentsKrosoving(NeironNetWork[] massNeironNetWork, int amountReturn)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];
        NeironNetWork[] parentNeiron = new NeironNetWork[2];
        for (int i = 0; i < amountReturn; i++)
        {
            parentNeiron[0] = massNeironNetWork[MyClass.rand.Next(0, amountReturn)];
            parentNeiron[1] = massNeironNetWork[MyClass.rand.Next(0, amountReturn)];
            returnNeiron[i] = ExchangeWeigth(parentNeiron);
        }
        return returnNeiron;
    }
    //Заготовка для перемешивания весов случайных лучших родителей
    NeironNetWork[] WinnersRandomParents(NeironNetWork[] massNeironNetWork, int amountReturn)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];
        for (int i = 0; i < amountReturn; i++)
        {
            returnNeiron[i] = Mutation(massNeironNetWork[MyClass.rand.Next(0, amountReturn)]);
        }
        return returnNeiron;
    }
    //Заготовка для простого обмена весов
    NeironNetWork ExchangeWeigth(NeironNetWork[] massNeironNetWork)
    {
        NeironNetWork returnNeiron = new NeironNetWork();

        for (int i = 0; i < returnNeiron.AmountNeironInLayer.Length - 1; i++)
        {
            for (int j = 0; j < returnNeiron.AmountNeironInLayer[i]; j++)
            {
                for (int k = 0; k < returnNeiron.MassNeiron[i][j].weight.Length; k++)
                {
                    int poz = MyClass.rand.Next(0, 2);
                    //returnNeiron.MassNeiron[i][j].weight[k] = MutationWeigth(massNeironNetWork[poz].MassNeiron[i][j].weight[k]);
                }
            }
        }
        return returnNeiron;
    }

    /// <summary>
    /// Мутация весов внутри НС
    /// </summary>
    /// <param name="neironNetWork"></param>
    /// <returns></returns>
    NeironNetWork Mutation(NeironNetWork neironNetWork)
    {
        NeironNetWork newNetWork = new NeironNetWork();

        for (int i = 0; i < neironNetWork.AmountNeironInLayer.Length - 1; i++)
        {
            for (int j = 0; j < neironNetWork.AmountNeironInLayer[i]; j++)
            {
                for (int k = 0; k < neironNetWork.MassNeiron[i][j].weight.Length; k++)
                {
                    newNetWork.MassNeiron[i][j].weight[k] = MutationWeigth(neironNetWork.MassNeiron[i][j].weight[k], ref newNetWork.delta);

                }
            }
        }
        
        //Thread.Sleep(10);   //Задежка для случайных чисел. Она нужна???????????
        return newNetWork;
    }
    /// <summary>
    /// Мктация конкретного веса
    /// </summary>
    /// <param name="value">значение веса</param>
    /// <param name="delta">тут я накапливаю изменения для всей НС. Для анализа обучения</param>
    /// <returns></returns>
    float MutationWeigth(float value, ref float delta)
    {
        float exchangeWeigth = 0;
        float step = stepMutationMax / (numberMutation * numberMutation);
        StepMutationNow = step;
        float probabilityNow = MyRand.value;       //вероятность отклонения для данного веса

        if (probabilityNow < mutationProbability)
        {
            exchangeWeigth = MyRand.Range(-step, step);
            value += exchangeWeigth;
            delta += Math.Abs(exchangeWeigth);
        }
        return value;
    }
}