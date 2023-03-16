using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private LightPolesManager lightPolesManager;
    [SerializeField] private SkyManager skyManager;
    [SerializeField] private LuminanceMapManager luminanceMapManager;

    void Awake()
    {
        Assert.IsNotNull(mapManager);
        Assert.IsNotNull(lightPolesManager);
        Assert.IsNotNull(skyManager);
        Assert.IsNotNull(luminanceMapManager);
    }

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        skyManager.SetDate(0);
        luminanceMapManager.EnableLuminanceMap(false);

        mapManager.Initialize();
        yield return new WaitUntil(() => mapManager.IsMapInitialized());
        lightPolesManager.SetConfiguration(0);
    }
}
