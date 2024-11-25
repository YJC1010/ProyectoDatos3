using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Airplane : MonoBehaviour
{
    public float speed = 2f;
    public float waitTimeMin = 2f;
    public float waitTimeMax = 5f;
    public float fuelCapacity = 100f;
    public float fuelConsumptionRate = 1f;

    private float currentFuel;
    private Vector2 currentTarget;
    private List<Vector2> path;
    private int pathIndex = 0;
    private bool isWaiting = false;
    private bool isFalling = false;
    private bool isImmune = false;
    private string airplaneID;  // ID único del avión

    private RouteGraph routeGraph;

    public AIModule PilotModule { get; private set; }
    public AIModule CopilotModule { get; private set; }
    public AIModule MaintenanceModule { get; private set; }
    public AIModule SpaceAwarenessModule { get; private set; }

    // Método para obtener el nivel de combustible actual
    public float GetFuel()
    {
        return currentFuel;
    }
    // Método para obtener la ruta del avión
    public List<Vector2> GetPath()
    {
        return path;
    }

    public void Initialize(RouteGraph graph)
    {
        routeGraph = graph;
        currentFuel = fuelCapacity;  // Llenar el tanque de gasolina al inicializar
        SetRandomDestination();      // Asignar destino al generarse
    }

    public void InitializeWithID()
    {
        airplaneID = Guid.NewGuid().ToString();
        Debug.Log("Avión creado con ID: " + airplaneID);

        // Inicializar los módulos de AI con roles específicos
        PilotModule = new AIModule("Pilot");
        CopilotModule = new AIModule("Copilot");
        MaintenanceModule = new AIModule("Maintenance");
        SpaceAwarenessModule = new AIModule("Space Awareness");

        // Mostrar detalles de cada módulo AI en la consola
        Debug.Log($"Módulo Pilot - ID: {PilotModule.ID}, Rol: {PilotModule.Role}, Horas de vuelo: {PilotModule.FlightHours}");
        Debug.Log($"Módulo Copilot - ID: {CopilotModule.ID}, Rol: {CopilotModule.Role}, Horas de vuelo: {CopilotModule.FlightHours}");
        Debug.Log($"Módulo Maintenance - ID: {MaintenanceModule.ID}, Rol: {MaintenanceModule.Role}, Horas de vuelo: {MaintenanceModule.FlightHours}");
        Debug.Log($"Módulo Space Awareness - ID: {SpaceAwarenessModule.ID}, Rol: {SpaceAwarenessModule.Role}, Horas de vuelo: {SpaceAwarenessModule.FlightHours}");
    }

    public string GetID()
    {
        return airplaneID;
    }

    void Update()
    {
        if (!isWaiting && !isFalling)
        {
            MoveAlongPath();
            ConsumeFuel();

            // Añadir horas de vuelo a los módulos activos mientras el avión esté volando
            float hoursToAdd = Time.deltaTime / 3600; // Convertir deltaTime a horas
            PilotModule.AddFlightHours(hoursToAdd);
            SpaceAwarenessModule.AddFlightHours(hoursToAdd);

            // Mostrar horas de vuelo actualizadas en la consola
            Debug.Log($"Horas de vuelo - Pilot: {PilotModule.FlightHours:F2}, Space Awareness: {SpaceAwarenessModule.FlightHours:F2}");
        }
        else if (isFalling)
        {
            Fall();
        }

        if (currentFuel <= 0 && !isFalling)
        {
            StartFalling();
        }
    }
    public List<AIModule> GetCrew()
    {
        return new List<AIModule> { PilotModule, CopilotModule, MaintenanceModule, SpaceAwarenessModule };
    }


    void MoveAlongPath()
    {
        if (path == null || pathIndex >= path.Count) return;

        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            pathIndex++;
            if (pathIndex < path.Count)
            {
                currentTarget = path[pathIndex];
            }
            else
            {
                StartCoroutine(WaitAtDestination());
                Refuel();
            }
        }
    }

    void ConsumeFuel()
    {
        currentFuel -= fuelConsumptionRate * Time.deltaTime;
        currentFuel = Mathf.Max(currentFuel, 0); // Asegura que la gasolina no sea negativa
    }

    void StartFalling()
    {
        isFalling = true;
        Debug.Log("El avión se quedó sin gasolina y está cayendo.");
    }

    void Fall()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    IEnumerator WaitAtDestination()
    {
        isWaiting = true;
        isImmune = true;  // Activar inmunidad mientras espera en una estación de carga
        float waitTime = UnityEngine.Random.Range(waitTimeMin, waitTimeMax);
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        isImmune = false;
        SetRandomDestination(); // Asignar nuevo destino al salir de la estación
    }

    void Refuel()
    {
        float fuelAmount = UnityEngine.Random.Range(20f, 50f);
        currentFuel = Mathf.Min(currentFuel + fuelAmount, fuelCapacity);
        Debug.Log("Avión recargó " + fuelAmount + " unidades de gasolina. Gasolina actual: " + currentFuel);

        // Llamar a GameController para actualizar la UI
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.UpdateUI();
            gameController.UpdateAirplaneAttributes(gameController.GetActiveAirplanes());
        }
    }

    void OnDestroy()
    {
        // Notificar al GameController que este avión se está destruyendo
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.RemoveActiveAirplane(this);
            Debug.Log($"Avión ID {GetID()} removido de la lista de aviones activos.");
        }
    }



    public bool IsImmune()
    {
        return isImmune;
    }

    public void SetRandomDestination()
    {
        if (routeGraph != null && routeGraph.locations.Count > 1)
        {
            Vector2 destination;
            do
            {
                destination = routeGraph.locations[UnityEngine.Random.Range(0, routeGraph.locations.Count)];
            } while (destination == (Vector2)transform.position);

            path = routeGraph.FindShortestPath(transform.position, destination);
            pathIndex = 0;
            if (path != null && path.Count > 0)
            {
                currentTarget = path[pathIndex];

                // Llamar a GameController para actualizar la UI
                GameController gameController = FindObjectOfType<GameController>();
                if (gameController != null)
                {
                    gameController.UpdatePathInfo(this); // Pasar este avión al método
                }
            }
        }
    }
}
