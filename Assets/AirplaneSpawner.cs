using UnityEngine;
using System.Collections;

public class AirplaneSpawner : MonoBehaviour
{
    public GameObject airplanePrefab;
    public int airplaneCount = 5;
    public float spawnInterval = 1f;

    private RouteGraph routeGraph;
    private GameController gameController;

    void Start()
    {
        routeGraph = FindObjectOfType<RouteGraph>();
        gameController = FindObjectOfType<GameController>();
        StartCoroutine(SpawnAirplanesWithInterval());
    }

    private IEnumerator SpawnAirplanesWithInterval()
    {
        for (int i = 0; i < airplaneCount; i++)
        {
            SpawnAirplane();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnAirplane()
    {
        if (routeGraph == null || routeGraph.locations.Count == 0)
        {
            Debug.LogWarning("No hay ubicaciones en el grafo para generar aviones.");
            return;
        }

        Vector2 spawnPosition = routeGraph.locations[Random.Range(0, routeGraph.locations.Count)];
        GameObject airplane = Instantiate(airplanePrefab, spawnPosition, Quaternion.identity);

        Airplane airplaneScript = airplane.GetComponent<Airplane>();
        if (airplaneScript != null)
        {
            airplaneScript.Initialize(routeGraph);
            airplaneScript.InitializeWithID();  // Asignar un ID único

            // Agregar el avión a la lista de aviones activos
            GameController gameController = FindObjectOfType<GameController>();
            if (gameController != null)
            {
                gameController.AddActiveAirplane(airplaneScript);
            }
        }

        if (gameController != null)
        {
            gameController.IncreaseAirplaneCount();
        }

        Debug.Log("Avión generado por el spawner en posición: " + spawnPosition);

        
    }
}

