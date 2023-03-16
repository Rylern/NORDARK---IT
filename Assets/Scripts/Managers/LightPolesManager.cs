using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft.Json;

public class LightPolesManager : MonoBehaviour
{
    private static readonly List<string> CONFIG_NAMES = new List<string> {
        "Config1",
        "Config2"
    };
    [SerializeField] private MapManager mapManager;
    [SerializeField] private IESManager iesManager;
    [SerializeField] private List<GameObject> lightPolePrefabs;
    private List<string> lightPrefabNames;
    private List<LightPrefab> lightPrefabs;

    void Awake()
    {
        Assert.IsNotNull(mapManager);
        Assert.IsNotNull(iesManager);
        Assert.IsNotNull(lightPolePrefabs);

        lightPrefabNames = new List<string>();
        foreach (GameObject lightPolePrefab in lightPolePrefabs) {
            lightPrefabNames.Add(lightPolePrefab.name);
        }
        lightPrefabs = new List<LightPrefab>();
    }

    public void SetConfiguration(int configurationIndex)
    {
        Clear();

        GeoJSON.Net.Feature.FeatureCollection featureCollection =
            JsonConvert.DeserializeObject<GeoJSON.Net.Feature.FeatureCollection>(Resources.Load<TextAsset>(CONFIG_NAMES[configurationIndex]).text);

        foreach (GeoJSON.Net.Feature.Feature feature in featureCollection.Features) {
            Create(feature);
        }
    }

    public void OnLocationChanged()
    {
        foreach (LightPrefab lightPrefab in lightPrefabs) {
            lightPrefab.SetPosition(mapManager.GetUnityPositionFromCoordinates(lightPrefab.GetCoordinate()));
        }
    }

    private void Clear()
    {
        foreach (LightPrefab lightPrefab in lightPrefabs) {
            lightPrefab.Destroy();
        }
        lightPrefabs.Clear();
    }

    private void Create(GeoJSON.Net.Feature.Feature feature)
    {
        GeoJSON.Net.Geometry.Point point = feature.Geometry as GeoJSON.Net.Geometry.Point;
        Coordinate coordinate = new Coordinate(point.Coordinates.Latitude, point.Coordinates.Longitude);

        string prefabName = feature.Properties["prefabName"] as string;        
        List<float> eulerAngles = (feature.Properties["eulerAngles"] as Newtonsoft.Json.Linq.JArray).ToObject<List<float>>();
        string IESName = feature.Properties["IESfileName"] as string;
        
        GameObject lightPrefab = null;
        foreach (GameObject lightPolePrefab in lightPolePrefabs) {
            if (lightPolePrefab.name == prefabName) {
                lightPrefab = lightPolePrefab;
            }
        }

        if (lightPrefab != null) {
            LightPrefab light = Instantiate(
                lightPrefab,
                mapManager.GetUnityPositionFromCoordinates(coordinate),
                Quaternion.Euler(new Vector3(eulerAngles[0], eulerAngles[1], eulerAngles[2])),
                transform
            ).GetComponent<LightPrefab>();
            light.Create(iesManager.GetIESLightFromName(IESName), coordinate);
            lightPrefabs.Add(light);
        }
    }
}