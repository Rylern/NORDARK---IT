using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class ParametersControl : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private LightPolesManager lightPolesManager;
    [SerializeField] private SkyManager skyManager;
    [SerializeField] private LuminanceMapManager luminanceMapManager;
    [SerializeField] private TMP_Dropdown location;
    [SerializeField] private TMP_Dropdown lightConfiguration;
    [SerializeField] private TMP_Dropdown date;
    [SerializeField] private Toggle luminanceMap;

    void Awake()
    {
        Assert.IsNotNull(mapManager);
        Assert.IsNotNull(lightPolesManager);
        Assert.IsNotNull(skyManager);
        Assert.IsNotNull(luminanceMapManager);
        Assert.IsNotNull(location);
        Assert.IsNotNull(lightConfiguration);
        Assert.IsNotNull(date);
        Assert.IsNotNull(luminanceMap);
    }

    void Start()
    {
        location.onValueChanged.AddListener(mapManager.ChangeLocation);
        lightConfiguration.onValueChanged.AddListener(lightPolesManager.SetConfiguration);
        date.onValueChanged.AddListener(skyManager.SetDate);
        luminanceMap.onValueChanged.AddListener(luminanceMapManager.EnableLuminanceMap);
    }
}
