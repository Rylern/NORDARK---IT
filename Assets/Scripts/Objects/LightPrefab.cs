using UnityEngine;
using UnityEngine.Assertions;

public class LightPrefab : MonoBehaviour
{
    private const float LIGHT_TEMPERATURE = 6500;
    [SerializeField] private Light unityLight;
    private UnityEngine.Rendering.HighDefinition.HDAdditionalLightData hdAdditionalLightData;
    private Coordinate coordinate;

    void Awake()
    {
        Assert.IsNotNull(unityLight);

        hdAdditionalLightData = unityLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
    }

    public void Create(IESLight iesLight, Coordinate coordinate)
    {
        hdAdditionalLightData.SetColor(hdAdditionalLightData.color, LIGHT_TEMPERATURE);
        hdAdditionalLightData.SetCookie(iesLight.Cookie);
        hdAdditionalLightData.SetIntensity(iesLight.Intensity.Value, iesLight.Intensity.Unit);
        this.coordinate = coordinate;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}