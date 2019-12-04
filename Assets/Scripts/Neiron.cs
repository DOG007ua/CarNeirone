using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neiron
{
    float e = 2.7182f; 
    protected float outValue;                        //выходное значение после сигмоида
    int id;                                 //№ нейрона в слое
    public float[] weight;                 //Выходные веса
    float[] inputValue;
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
    /// <summary>
    /// Случайное заполнение весами
    /// </summary>
    /// <param name="amountValue"></param>
    /// <returns></returns>
    float[] RandomWeight(int amountValue)
    {
        float[] weight = new float[amountValue];
        for (int i = 0; i < weight.Length; i++)
        {    
            weight[i] = Random.Range(-1.0f, 1.0f);
           
        }
        return weight;
    }
    /// <summary>
    /// Начало приёма данных с пред. слоя и поддготовка к передаче на след. слой
    /// </summary>
    public virtual void Start()
    {
        Summator();
        TransmitNextLayer();
    }
    /// <summary>
    /// Сглаживание выходного значения нейронна
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Pow(e, -x));
    }

    protected float TanActivationFun(float x)
    {
        if (x > 20) x = 20;
        else if(x < -20) x = -20;

        float v = (Mathf.Pow(e, x) + Mathf.Pow(e, -x));
        return (Mathf.Pow(e, x) - Mathf.Pow(e, -x)) / (Mathf.Pow(e, x) + Mathf.Pow(e, -x));
    }
    /// <summary>
    /// Производная сигмоида. Учитывается при обратном распространении ошибка
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    protected float DerivativeSigmoid(float x)
    {
        return x * (1 - x);
    }
    /// <summary>
    /// Сумматор нейрона
    /// </summary>
    virtual protected void Summator()
    {
        float summ = 0;
        for (int i = 0; i < amountNeironPreviusLayer; i++)
        {
            summ += inputValue[i];
        }
        outValue = TanActivationFun(summ);
    }
    /// <summary>
    /// Подготовка значений для передачи на след. слой
    /// </summary>
    void TransmitNextLayer()
    {
        for (int i = 0; i < amountNeironNextLayer; i++)
        {
            neironNextLayer[i].inputValue[id] = outValue * weight[i];
        }
    }
    /// <summary>
    /// Обратное распространение ошибки
    /// </summary>
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
    /// <summary>
    /// Расчёт новых весов для обратного распространения ошибки
    /// </summary>
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
/// <summary>
/// Выходной нейрон
/// </summary>
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
/// <summary>
/// Входной нейрон
/// </summary>
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
