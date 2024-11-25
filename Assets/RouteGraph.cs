using System.Collections.Generic;
using UnityEngine;

public class RouteGraph : MonoBehaviour
{
    public GameObject airportPrefab;
    public GameObject carrierPrefab;
    public int airportCount = 5;
    public int carrierCount = 3;

    public List<Vector2> locations = new List<Vector2>();  // Almacena las ubicaciones de aeropuertos y portaaviones
    public List<Route> routes = new List<Route>();        // Almacena las rutas entre ubicaciones

    public List<Route> GetRoutes()
    {
        return routes;
    }

    void Start()
    {
        GenerateAirportsAndCarriers();
        GenerateRoutes();  // Generar rutas entre las ubicaciones en `locations`
    }

    void GenerateAirportsAndCarriers()
    {
        // Generar aeropuertos en los rangos especificados
        GenerateAirportsInRange(new Vector2(2.5f, -3f), new Vector2(1.5f, 3f));
        GenerateAirportsInRange(new Vector2(2f, -1.5f), new Vector2(1f, -0.5f));

        // Generar portaaviones en los rangos especificados
        GenerateCarriersInRange(new Vector2(5f, 4f), new Vector2(3f, -3f));
        GenerateCarriersInRange(new Vector2(-2f, -5f), new Vector2(0.9f, -3f));
        GenerateCarriersInRange(new Vector2(1.5f, -1.5f), new Vector2(-2f, -3f));
    }

    void GenerateAirportsInRange(Vector2 xRange, Vector2 yRange)
    {
        for (int i = 0; i < airportCount / 2; i++)  // Dividimos la cantidad entre los rangos
        {
            Vector2 position = new Vector2(
                Random.Range(xRange.x, xRange.y),
                Random.Range(yRange.x, yRange.y)
            );
            Instantiate(airportPrefab, position, Quaternion.identity);
            locations.Add(position);
            Debug.Log("Aeropuerto generado en posición: " + position);
        }
    }

    void GenerateCarriersInRange(Vector2 xRange, Vector2 yRange)
    {
        for (int i = 0; i < carrierCount / 3; i++)  // Dividimos la cantidad entre los rangos
        {
            Vector2 position = new Vector2(
                Random.Range(xRange.x, xRange.y),
                Random.Range(yRange.x, yRange.y)
            );
            Instantiate(carrierPrefab, position, Quaternion.identity);
            locations.Add(position);
            Debug.Log("Portaaviones generado en posición: " + position);
        }
    }

    Vector2 GenerateRandomPosition()
    {
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(-3f, 3f);
        return new Vector2(x, y);
    }

    public void GenerateRoutes()
    {
        for (int i = 0; i < locations.Count; i++)
        {
            for (int j = i + 1; j < locations.Count; j++)
            {
                Vector2 start = locations[i];
                Vector2 end = locations[j];
                float distance = Vector2.Distance(start, end);
                routes.Add(new Route(start, end, distance));
            }
        }
    }

    public List<Vector2> FindShortestPath(Vector2 start, Vector2 destination)
    {
        // Implementación simplificada de Dijkstra o cualquier otro algoritmo de búsqueda
        // Asumiendo que las rutas son directas en este ejemplo
        return new List<Vector2> { start, destination };
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (var route in routes)
        {
            Gizmos.DrawLine(route.start, route.end); // Dibujar la línea de la ruta

            // Mostrar el peso en el centro de la línea
            Vector3 midpoint = (route.start + route.end) / 2;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            UnityEditor.Handles.Label(midpoint, $"Peso: {route.distance}", style);
        }
    }

    
}

public class Route
{
    public Vector2 start;
    public Vector2 end;
    public float distance;

    public Route(Vector2 start, Vector2 end, float distance)
    {
        this.start = start;
        this.end = end;
        this.distance = distance;
    }
}
