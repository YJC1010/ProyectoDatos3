using UnityEngine;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    public GameObject airportPrefab;
    public GameObject carrierPrefab;
    public int airportCount = 5;
    public int carrierCount = 3;
    public float mapWidth = 100f;
    public float mapHeight = 100f;

    private List<Vector2> positions = new List<Vector2>();
    private RouteGraph routeGraph;

    void Start()
    {
        routeGraph = GetComponent<RouteGraph>();  // Obtener el componente RouteGraph
        GenerateAirports();
        GenerateCarriers();
        routeGraph.locations = positions;  // Pasar las posiciones generadas a RouteGraph
        routeGraph.GenerateRoutes();       // Generar las rutas en el grafo
    }

    void GenerateAirports()
    {
        for (int i = 0; i < airportCount; i++)
        {
            Vector2 position = GenerateRandomPosition();
            Instantiate(airportPrefab, position, Quaternion.identity);
            positions.Add(position);
        }
    }

    void GenerateCarriers()
    {
        for (int i = 0; i < carrierCount; i++)
        {
            Vector2 position = GenerateRandomPosition();
            Instantiate(carrierPrefab, position, Quaternion.identity);
            positions.Add(position);
        }
    }

    Vector2 GenerateRandomPosition()
    {
        float x = Random.Range(-5f, 5f); // Limites en el eje X
        float y = Random.Range(-3f, 3f); // Limites en el eje Y
        return new Vector2(x, y);
    }

}

