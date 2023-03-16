using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SkyManager : MonoBehaviour
{
    private static readonly List<System.DateTime> DATES = new List<System.DateTime> {
        new System.DateTime(2023, 3, 15, 12, 0, 0),
        new System.DateTime(2023, 3, 15, 22, 0, 0)
    };
    private const float MOON_MAX_INTENSITY = 0.5f;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private Transform sun;
    [SerializeField] private Transform moon;
    private System.DateTime dateTime;
    private UnityEngine.Rendering.HighDefinition.HDAdditionalLightData moonLight;

    void Awake()
    {
        Assert.IsNotNull(mapManager);
        Assert.IsNotNull(sun);
        Assert.IsNotNull(moon);

        moonLight = moon.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
    }

    public void SetDate(int dateIndex)
    {
        dateTime = DATES[dateIndex];
        UpdateSunAndMoon();
    }

    public void OnLocationChanged()
    {
        UpdateSunAndMoon();
    }

    private void UpdateSunAndMoon()
    {
        SetSunAndMoonPosition();
        SetMoonIntensity();
    }

    private void SetSunAndMoonPosition()
    {
        Sampa.Sampa sampa = new Sampa.Sampa();
        sampa.Calculate(dateTime, mapManager.GetMapCoordinate());
        
        sun.localEulerAngles = new Vector3((float) sampa.spa.e, (float) sampa.spa.azimuth, 0);
        moon.localEulerAngles = new Vector3((float) sampa.mpa.e, (float) sampa.mpa.azimuth, 0);
    }

    private void SetMoonIntensity()
    {
        moonLight.SetIntensity(Mathf.Lerp(0, MOON_MAX_INTENSITY, (float) MoonPhaseConsole.Moon.Calculate(dateTime).Visibility/100));
    }
}
