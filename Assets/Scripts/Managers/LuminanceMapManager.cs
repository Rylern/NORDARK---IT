using UnityEngine;
using UnityEngine.Assertions;

public class LuminanceMapManager : MonoBehaviour
{
    private const float DEFAULT_MINIMUM_VALUE = 0.01f;
    private const float DEFAULT_MAXIMUM_VALUE = 10000;
    [SerializeField] private GameObject luminanceMapPassAndVolume;
    [SerializeField] private Material luminanceMapPassMaterial;
    [SerializeField] private LuminanceMapLegend luminanceMapLegend;
    
    void Awake()
    {
        Assert.IsNotNull(luminanceMapPassAndVolume);
        Assert.IsNotNull(luminanceMapPassMaterial);
        Assert.IsNotNull(luminanceMapLegend);

        luminanceMapPassMaterial.SetColorArray("colors", luminanceMapLegend.GetColors());
        SetScale();
    }

    public void EnableLuminanceMap(bool enable)
    {
        luminanceMapPassAndVolume.SetActive(enable);
    }

    private void SetScale()
    {
        int numberOfColors = luminanceMapLegend.GetNumberOfColors();
        float[] values = new float[numberOfColors];

        float minValueLog = Mathf.Log10(DEFAULT_MINIMUM_VALUE);
        float maxValueLog = Mathf.Log10(DEFAULT_MAXIMUM_VALUE);

        for (int i=0; i<numberOfColors; ++i) {
            values[i] = Mathf.Pow(10, minValueLog + i * (maxValueLog - minValueLog) / (numberOfColors-1));
        }

        luminanceMapLegend.SetValues(values);

        luminanceMapPassMaterial.SetInt("numberOfColors", values.Length);
        luminanceMapPassMaterial.SetFloatArray("values", values);
    }
}