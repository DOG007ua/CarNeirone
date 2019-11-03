using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class NeironNetWork : IComparable
{
    int[] amountNeironInLayer = { 3, 6, 4 };
    int amountLayer = 0;
    public Neiron[][] massNeiron;               //кол. слоев\\//№ нейрона
    public List<float> list;


    public int[] AmountNeironInLayer { get { return amountNeironInLayer; } }
    public NeironNetWork()
    {
        amountLayer = amountNeironInLayer.Length;
        CreateNeiron();
        MyClass.rand = new Random();
    }

    int IComparable.CompareTo(object o)
    {
        NeironNetWork temp = (NeironNetWork)o;
        if (this.AbsError > temp.AbsError) return 1;
        if (this.AbsError < temp.AbsError) return -1;
        else return 0;
    }

    public float AbsError { get; set; }

    public Neiron[][] MassNeiron
    {
        get { return massNeiron; }
        set { massNeiron = value; }
    }

    public float[] Start(float[] inputValue)
    {
        for (int i = 0; i < inputValue.Length; i++)
        {
            ((NeironInFirstLayer)massNeiron[0][i]).inputValue = inputValue[i];
        }

        for (int numberLayer = 0; numberLayer < amountLayer; numberLayer++)
        {
            for (int i = 0; i < amountNeironInLayer[numberLayer]; i++)
            {
                massNeiron[numberLayer][i].Start();
            }
        }
        float[] rezult = new float[amountNeironInLayer[amountLayer - 1]];
        for (int i = 0; i < amountNeironInLayer[amountLayer - 1]; i++)
        {
            rezult[i] = massNeiron[amountLayer - 1][i].OutValue;
        }
        return rezult;
    }

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

    void CreateNeiron()
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
}

public class GeneticAlgoritm
{
    int bestCar = 4;
    int maxCar;

    public GeneticAlgoritm(int maxCar)
    {
        this.maxCar = maxCar;
    }


    /// <summary>
    /// Возвращает лучшие машины по дистанции
    /// </summary>
    /// <returns></returns>
    Car[] FindCarDistance(Car[] massCar)
    {
        Array.Sort(massCar);
        Car[] returnCar = new Car[bestCar];
        for (int i = 0; i < bestCar; i++)
        {
            returnCar[i] = massCar[massCar.Length - 1 - i];
        }
        return returnCar;
    }

    public NeironNetWork[] CreateNewCar(Car[] massCar)
    {
        NeironNetWork[] massNeironNetWork = new NeironNetWork[maxCar];
        Car[] massBestCar = FindCarDistance(massCar);
        int amountBestParents = (int)(maxCar * 0.4);
        int amountBestParentsKrosoving = (int)(maxCar * 0.1);
        int amountWinnersParentsKrosoving = (int)(maxCar * 0.3);
        int amountWinnersRandomParents = maxCar - amountBestParents - amountBestParentsKrosoving - amountWinnersParentsKrosoving;



        /*for (int i = 0; i < maxCar; i++)
        {
            //int v = MyClass.rand.Next(0, massBestCar.Length);
            //massNeironNetWork[i] = Mutation(massBestCar[v].neironNetWork);
        }*/
        return massNeironNetWork;
    }

    NeironNetWork[] BestParents(NeironNetWork[] massNeironNetWork, int amountReturn)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];
        for (int i = 0; i < amountReturn; i++)
        {
            returnNeiron[i] = Mutation(massNeironNetWork[i]);
        }
        return returnNeiron;
    }

    NeironNetWork[] BestParentsKrosoving(NeironNetWork[] massNeironNetWork, int amountReturn)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];
        for (int i = 0; i < amountReturn; i++)
        {
            returnNeiron[i] = ExchangeWeigth(massNeironNetWork);
        }
        return returnNeiron;
    }

    NeironNetWork[] WinnersParentsKrosoving(NeironNetWork[] massNeironNetWork, int amountReturn)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];
        NeironNetWork[] parentNeiron = new NeironNetWork[2];        
        for (int i = 0; i < amountReturn; i++)
        {
            parentNeiron[0] = massNeironNetWork[MyClass.rand.Next(0, bestCar)];
            parentNeiron[1] = massNeironNetWork[MyClass.rand.Next(0, bestCar)];
            returnNeiron[i] = ExchangeWeigth(parentNeiron);
        }
        return returnNeiron;
    }

    NeironNetWork[] WinnersRandomParents(NeironNetWork[] massNeironNetWork, int amountReturn)
    {
        NeironNetWork[] returnNeiron = new NeironNetWork[amountReturn];
        for (int i = 0; i < amountReturn; i++)
        {
            returnNeiron[i] = Mutation(massNeironNetWork[MyClass.rand.Next(0, bestCar)]);
        }
        return returnNeiron;
    }



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
                    returnNeiron.MassNeiron[i][j].weight[k] = MutationWeigth(massNeironNetWork[poz].MassNeiron[i][j].weight[k]);
                }
            }
        }
        return returnNeiron;
    }

    NeironNetWork Mutation(NeironNetWork neironNetWork)
    {
        neironNetWork.list = new List<float>();
        List<float> listW = new List<float>();

        NeironNetWork newNetWork = new NeironNetWork();

        for (int i = 0; i < neironNetWork.AmountNeironInLayer.Length - 1; i++)
        {
            for(int j = 0; j < neironNetWork.AmountNeironInLayer[i]; j++)
            {
                for(int k = 0; k < neironNetWork.MassNeiron[i][j].weight.Length; k++)
                {

                    float v = MutationWeigth(neironNetWork.MassNeiron[i][j].weight[k]);
                    newNetWork.MassNeiron[i][j].weight[k] = v;
                }                
            }            
        }
        Thread.Sleep(20);
        return newNetWork;
    }

    float MutationWeigth(float value)
    {
        float summ = 0;
        float procentMutation = 0.001f;
        int stepMutation = 300;         // делённое на 1000
        float ver = MyClass.rand.Next(0, 10000) / 10000.0f;

        if(ver < procentMutation)
        {
            summ = MyClass.rand.Next(-stepMutation, stepMutation) / 1000.0f;
            value += summ;
        }
        return value;
    }
}



