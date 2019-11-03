using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neiron
{
    float e = 2.7182818284f; 
    protected float outValue;                        //выходное значение после сигмоида
    int id;                                 //№ нейрона в слое
    public float[] weight;                 //Выходные веса
    public float[] inputValue;
    Neiron[] neironNextLayer;               //ссылка на нейроны след. слоя
    protected int amountNeironNextLayer;              //кол. нейронов в след. слое
    int amountNeironPreviusLayer;           //кол. нейронов в пред. слое
    protected float sigma;                           //ошибка нейрона
    float changeW = 0;

    public float OutValue { get { return outValue; } set { outValue = value; } }
    public float Sigma { get { return sigma; } }

    public Neiron(Neiron[] neironNextLayer, int amountPreviusLayerNeiron, int id)   //если скрытый слой
    {
        this.neironNextLayer = neironNextLayer;
        this.id = id;
        amountNeironNextLayer = neironNextLayer.Length;
        this.amountNeironPreviusLayer = amountPreviusLayerNeiron;
        weight = RandomWeight(amountNeironNextLayer); ;
        inputValue = new float[amountPreviusLayerNeiron];
    }

    public Neiron(Neiron[] neironNextLayer, int id)   //если первый слой
    {
        this.neironNextLayer = neironNextLayer;
        this.id = id;
        amountNeironNextLayer = neironNextLayer.Length;
        weight = RandomWeight(amountNeironNextLayer);
    }

    public Neiron(int amountPreviusLayerNeiron)   //если последний слой
    {
        this.amountNeironPreviusLayer = amountPreviusLayerNeiron;
        inputValue = new float[amountPreviusLayerNeiron];
    }

    public Neiron() { }

    float[] RandomWeight(int amountValue)
    {
        float[] weight = new float[amountValue];
        for (int i = 0; i < weight.Length; i++)
        {
            //weight[i] = Random.Range(0, (100 + 1)) / 100;      
            weight[i] = Random.value;
        }
        return weight;
    }

    public virtual void Start()
    {
        Summator();
        TransmitNextLayer();
    }

    float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Pow(e, -x));
    }

    protected float DerivativeSigmoid(float x)
    {
        return x * (1 - x);
    }

    virtual protected void Summator()
    {
        float summ = 0;
        for (int i = 0; i < amountNeironPreviusLayer; i++)
        {
            summ += inputValue[i];
        }
        outValue = summ;
        //outValue = Sigmoid(summ);
    }

    void TransmitNextLayer()
    {
        for (int i = 0; i < amountNeironNextLayer; i++)
        {
            neironNextLayer[i].inputValue[id] = outValue * weight[i];
        }
    }

    public virtual void Training()
    {
        CalculationSigma();
        CalculationNewWeight();
    }

    protected virtual void CalculationSigma()
    {
        float summ = 0;
        for (int i = 0; i < amountNeironNextLayer; i++)
        {
            summ += weight[i] * neironNextLayer[i].Sigma;
        }
        sigma = summ * DerivativeSigmoid(outValue);
    }

    void CalculationNewWeight()
    {
        float grad = 0;
        float oldW = 0;
        float deltaW = 0;
        float E = 0.7f;
        float alpha = 0.1f;
        for (int i = 0; i < amountNeironNextLayer; i++)
        {
            grad = neironNextLayer[i].sigma * outValue;
            deltaW = E * grad + alpha * changeW;
            oldW = weight[i];
            weight[i] = weight[i] + deltaW;
            changeW = weight[i] - oldW;
        }
    }
}

class NeironInLastLayer : Neiron
{
    public float outValueIdeal = 0;


    public NeironInLastLayer(int amountPreviusLayerNeiron)
        : base(amountPreviusLayerNeiron)
    {

    }

    public override void Start()
    {
        Summator();
    }

    public override void Training()
    {
        CalculationSigma();
    }

    protected override void CalculationSigma()
    {
        sigma = (outValueIdeal - OutValue) * DerivativeSigmoid(OutValue);
    }
}

class NeironInFirstLayer : Neiron
{
    public new float inputValue;
    public NeironInFirstLayer(Neiron[] neironNextLayer, int id) : base(neironNextLayer, id)   //если первый слой
    {

    }

    override protected void Summator()
    {
        outValue = inputValue;
    }
}
