using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;

public class MapManager : MonoBehaviour
{
    private static readonly List<Coordinate> LOCATION_COORDINATES = new List<Coordinate> {
        new Coordinate(62.46767, 6.301557),
        new Coordinate(59.807076, 17.703123)
    };
    [SerializeField] private LightPolesManager lightPolesManager;
    [SerializeField] private SkyManager skyManager;
    private AbstractMap map;
    private bool isMapInitialized;
    private int numberOfTiles;
    private int numberOfTilesInitialized;

    void Awake()
    {
        Assert.IsNotNull(lightPolesManager);
        Assert.IsNotNull(skyManager);

        map = GetComponent<AbstractMap>();
        map.OnTileFinished += TileFinished;

        Mapbox.Unity.Map.RangeTileProviderOptions extentOptions = (Mapbox.Unity.Map.RangeTileProviderOptions) map.Options.extentOptions.GetTileProviderOptions();
        numberOfTiles = (extentOptions.north + 1 + extentOptions.south) * (extentOptions.west + 1 + extentOptions.east);

        numberOfTilesInitialized = 0;
        isMapInitialized = false;
    }

    public void Initialize()
    {
        map.Initialize(new Mapbox.Utils.Vector2d(
            LOCATION_COORDINATES[0].latitude,
            LOCATION_COORDINATES[0].longitude
        ), (int) map.Zoom);
    }

    public bool IsMapInitialized()
    {
        return isMapInitialized;
    }

    public void ChangeLocation(int coordinateIndex)
    {
        numberOfTilesInitialized = 0;

        map.SetCenterLatitudeLongitude(new Mapbox.Utils.Vector2d(
            LOCATION_COORDINATES[coordinateIndex].latitude,
            LOCATION_COORDINATES[coordinateIndex].longitude
        ));
        map.UpdateMap();

        // There is a Mapbox bug with the roads, buildings and elevation, so we reset them
        map.VectorData.GetFeatureSubLayerAtIndex(0).SetActive(false);
        map.VectorData.GetFeatureSubLayerAtIndex(0).SetActive(true);
        map.Terrain.SetElevationType(ElevationLayerType.FlatTerrain);
        map.Terrain.SetElevationType(ElevationLayerType.TerrainWithElevation);
    }

    public Vector3 GetUnityPositionFromCoordinates(Coordinate coordinates)
    {
        Vector3 position = Conversions.GeoToWorldPosition(coordinates.latitude, coordinates.longitude, map.CenterMercator, map.WorldRelativeScale).ToVector3xz();
        position.y = map.QueryElevationInUnityUnitsAt(new Mapbox.Utils.Vector2d(coordinates.latitude, coordinates.longitude));
        return position;
    }

    public Coordinate GetMapCoordinate()
    {
        return new Coordinate(map.CenterLatitudeLongitude);
    }

    private void TileFinished(Mapbox.Unity.MeshGeneration.Data.UnityTile tile)
    {
        if (!isMapInitialized) {
            isMapInitialized = true;
        }

        numberOfTilesInitialized++;

        if (numberOfTilesInitialized == numberOfTiles) {
            lightPolesManager.OnLocationChanged();
            skyManager.OnLocationChanged();
        }
    }
}