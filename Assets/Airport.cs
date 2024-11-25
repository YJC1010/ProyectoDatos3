using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Airport : MonoBehaviour
{
    public GameObject airplanePrefab;
    public int hangarCapacity = 3;             // Capacidad m�xima del hangar en el aeropuerto
    public int totalCapacity = 15;             // Capacidad m�xima total de generaci�n de aviones
    public float constructionInterval = 10f;   // Intervalo de construcci�n de aviones

    private int totalPlanesGenerated = 0;      // Contador global de aviones generados por este aeropuerto
    private List<GameObject> airplanes = new List<GameObject>();
    private RouteGraph routeGraph;
    private GameController gameController;

    void Start()
    {
        routeGraph = FindObjectOfType<RouteGraph>();
        gameController = FindObjectOfType<GameController>();
        StartCoroutine(BuildAirplanes());
    }

    IEnumerator BuildAirplanes()
    {
        while (true)
        {
            yield return new WaitForSeconds(constructionInterval);

            // Verificar si el hangar est� lleno o si se ha alcanzado el l�mite total de generaci�n de aviones
            if (airplanes.Count < hangarCapacity && totalPlanesGenerated < totalCapacity)
            {
                BuildNewAirplane();
            }
            else
            {
                Debug.Log("El aeropuerto " + gameObject.name + " alcanz� su l�mite de capacidad total de aviones.");
            }
        }
    }

    void BuildNewAirplane()
    {
        // Generar una posici�n cercana al aeropuerto para el nuevo avi�n
        Vector2 spawnPosition = new Vector2(
            transform.position.x + Random.Range(-1f, 1f),
            transform.position.y + Random.Range(-1f, 1f)
        );

        GameObject newAirplane = Instantiate(airplanePrefab, spawnPosition, Quaternion.identity);
        Airplane airplaneScript = newAirplane.GetComponent<Airplane>();

        if (airplaneScript != null)
        {
            airplaneScript.Initialize(routeGraph);
            airplaneScript.InitializeWithID();

            // Asignar un ID �nico

            // Agregar el avi�n a la lista de aviones activos
            GameController gameController = FindObjectOfType<GameController>();
            if (gameController != null)
            {
                gameController.AddActiveAirplane(airplaneScript);
            }
        }

        airplanes.Add(newAirplane);    // A�adir a la lista del hangar
        totalPlanesGenerated++;        // Incrementar el contador de aviones generados

        // Aumentar el contador en GameController
        if (gameController != null)
        {
            gameController.IncreaseAirplaneCount();
        }

        Debug.Log("Nuevo avi�n construido en " + gameObject.name + " con ID: " + airplaneScript.GetID());
    }
}
