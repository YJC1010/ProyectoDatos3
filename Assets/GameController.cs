using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI airplaneCounterText;
    public TextMeshProUGUI destroyedAirplanesText;
    public TextMeshProUGUI pathInfoText;          // Texto para caminos calculados
    public TextMeshProUGUI routeWeightsText;      // Texto para pesos de rutas
    public TextMeshProUGUI airplaneAttributesText; // Texto para atributos de aviones
    public GameObject gameOverPanel;

    public float gameDuration = 60f;

    private float timeRemaining;
    private int airplanesRemaining = 0;
    private int airplanesDestroyed = 0;
    private bool isGameOver = false;
    // Lista de aviones derribados
    private List<Airplane> destroyedAirplanes = new List<Airplane>();
    private List<Airplane> activeAirplanes = new List<Airplane>();

    private RouteGraph routeGraph;
    void Start()
    {
        timeRemaining = gameDuration;
        UpdateUI();

        RouteGraph routeGraph = FindObjectOfType<RouteGraph>();
        if (routeGraph != null)
        {
            UpdateRouteWeights(routeGraph); // Actualizar pesos de las rutas al inicio
        }
    }

     public void AddActiveAirplane(Airplane airplane)
    {
        activeAirplanes.Add(airplane);
        UpdateAirplaneAttributes(activeAirplanes);
    }

    public void RemoveActiveAirplane(Airplane airplane)
    {
        activeAirplanes.Remove(airplane);
        UpdateAirplaneAttributes(activeAirplanes);
    }



    public List<Airplane> GetActiveAirplanes()
    {
        return activeAirplanes;
    }


    void Update()
    {
        if (!isGameOver)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame();
            }
            UpdateUI();

            UpdateAirplaneAttributes(activeAirplanes);
        }
    }

    public void UpdatePathInfo(Airplane airplane)
    {
        if (pathInfoText != null && airplane != null)
        {
            string pathInfo = $"Camino Calculado de {airplane.GetID()}:\n";
            var path = airplane.GetPath();

            if (path != null && path.Count > 0)
            {
                foreach (var point in path)
                {
                    pathInfo += $"{point}\n";
                }
            }
            else
            {
                pathInfo += "No hay camino calculado.\n";
            }

            pathInfoText.text = pathInfo;
        }
    }



    public void UpdateRouteWeights(RouteGraph routeGraph)
    {
        if (routeWeightsText != null && routeGraph != null)
        {
            string weightsInfo = "Pesos de las Rutas:\n";
            var routes = routeGraph.GetRoutes(); // Usa GetRoutes o accede a routeGraph.routes directamente

            if (routes != null && routes.Count > 0)
            {
                foreach (var route in routes)
                {
                    weightsInfo += $"Ruta {route.start} -> {route.end}: Peso {route.distance}\n";
                }
            }
            else
            {
                weightsInfo += "No hay rutas disponibles.\n";
            }

            routeWeightsText.text = weightsInfo;
        }
    }


    public void UpdateAirplaneAttributes(List<Airplane> airplanes)
    {
        if (airplaneAttributesText != null && airplanes != null)
        {
            string attributesInfo = "Atributos de Aviones:\n";

            if (airplanes.Count > 0)
            {
                foreach (var airplane in airplanes)
                {
                    attributesInfo += $"ID: {airplane.GetID()}, Fuel: {airplane.GetFuel()}, Velocidad: {airplane.speed}\n";
                }
            }
            else
            {
                attributesInfo += "No hay aviones disponibles.\n";
            }

            airplaneAttributesText.text = attributesInfo;
        }
    }


    public void IncreaseAirplaneCount()
    {
        airplanesRemaining++;
        UpdateUI();
    }

    public void DecreaseAirplaneCount(Airplane airplane)
    {
        airplanesRemaining--;
        airplanesDestroyed++;

        // Añadir el avión derribado a la lista
        AddDestroyedAirplane(airplane);

        UpdateUI();
        if (airplanesRemaining <= 0)
        {
            EndGame();
        }
    }

    // Método para añadir el avión a la lista de derribados
    public void AddDestroyedAirplane(Airplane airplane)
    {
        destroyedAirplanes.Add(airplane);
    }

    // Método para ordenar los aviones derribados por ID utilizando Merge Sort
    public void SortDestroyedAirplanesByID()
    {
        destroyedAirplanes = MergeSort(destroyedAirplanes);
    }

    // Implementación del Merge Sort para ordenar la lista de aviones por ID
    private List<Airplane> MergeSort(List<Airplane> list)
    {
        if (list.Count <= 1)
            return list;

        int middle = list.Count / 2;
        List<Airplane> left = MergeSort(list.GetRange(0, middle));
        List<Airplane> right = MergeSort(list.GetRange(middle, list.Count - middle));

        return Merge(left, right);
    }

    private List<Airplane> Merge(List<Airplane> left, List<Airplane> right)
    {
        List<Airplane> result = new List<Airplane>();

        while (left.Count > 0 && right.Count > 0)
        {
            if (string.Compare(left[0].GetID(), right[0].GetID()) <= 0)
            {
                result.Add(left[0]);
                left.RemoveAt(0);
            }
            else
            {
                result.Add(right[0]);
                right.RemoveAt(0);
            }
        }

        result.AddRange(left);
        result.AddRange(right);

        return result;
    }

    void EndGame()
    {
        isGameOver = true;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        SortDestroyedAirplanesByID(); // Ordenar la lista al finalizar el juego
        ShowFinalScore();
        Debug.Log("Juego Terminado");
    }

    void ShowFinalScore()
    {
        if (destroyedAirplanesText != null)
        {
            destroyedAirplanesText.text = "Aviones Eliminados: " + airplanesDestroyed;

            foreach (Airplane airplane in destroyedAirplanes)
            {
                ShowSortedCrew(airplane, "id"); // Puedes cambiar "id", "rol" o "horas de vuelo"
            }
            // Mostrar los IDs ordenados de los aviones derribados
            string airplaneList = "Lista de Aviones Derribados (Ordenada por ID):\n";
            foreach (Airplane airplane in destroyedAirplanes)
            {
                airplaneList += $"{airplane.GetID()}\n";
            }
            Debug.Log(airplaneList);

            
        }
    }

    public void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "Tiempo: " + Mathf.Ceil(timeRemaining).ToString();

        if (airplaneCounterText != null)
            airplaneCounterText.text = "Aviones: " + airplanesRemaining.ToString();
    }
    public void SortCrew(List<AIModule> crew, string criterion)
    {
        // Ordenar la tripulación usando Selection Sort según el criterio especificado
        for (int i = 0; i < crew.Count - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < crew.Count; j++)
            {
                bool shouldSwap = false;

                switch (criterion.ToLower())
                {
                    case "id":
                        shouldSwap = string.Compare(crew[j].ID, crew[minIndex].ID) < 0;
                        break;
                    case "rol":
                        shouldSwap = string.Compare(crew[j].Role, crew[minIndex].Role) < 0;
                        break;
                    case "horas de vuelo":
                        shouldSwap = crew[j].FlightHours < crew[minIndex].FlightHours;
                        break;
                    default:
                        Debug.LogWarning("Criterio no válido: " + criterion);
                        return;
                }

                if (shouldSwap)
                {
                    minIndex = j;
                }
            }

            // Intercambiar elementos
            if (minIndex != i)
            {
                AIModule temp = crew[i];
                crew[i] = crew[minIndex];
                crew[minIndex] = temp;
            }
        }
    }

    // Método para mostrar la tripulación de cada avión derribado en la consola
    public void ShowSortedCrew(Airplane airplane, string criterion)
    {
        List<AIModule> crew = airplane.GetCrew();

        // Verificar que la tripulación no está vacía
        if (crew == null || crew.Count == 0)
        {
            Debug.LogWarning($"El avión con ID {airplane.GetID()} no tiene módulos de tripulación asignados.");
            return;
        }

        SortCrew(crew, criterion);

        Debug.Log($"Tripulación del avión ID {airplane.GetID()} ordenada por {criterion}:");
        foreach (var module in crew)
        {
            Debug.Log($"ID: {module.ID}, Rol: {module.Role}, Horas de vuelo: {module.FlightHours}");
        }
    }
}
