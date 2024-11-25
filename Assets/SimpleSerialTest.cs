using UnityEngine;
using System.IO.Ports;

public class SerialTest : MonoBehaviour
{
    private SerialPort serialPort; // Comunicación con el Arduino
    private string portName = "COM3"; // Cambia al puerto correcto
    private int baudRate = 9600;

    public GameObject bulletPrefab; // Prefab del proyectil (usa el existente en tu proyecto)
    public Transform firePoint; // Punto desde donde se disparan los proyectiles
    public Transform servoObject; // Objeto que representa el movimiento del servo
    public float bulletSpeed = 10f; // Velocidad del proyectil
    private bool receivedButtonPressed = false; // Variable para rastrear el comando del botón
    public AntiAircraftBattery antiAircraftBattery;

    void Start()
    {
        Debug.LogError("Inicio del script SerialTest.");

        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            Debug.LogError("Puerto serial abierto correctamente.");

            // Enviar el comando para activar el movimiento del servo al iniciar el juego
            SendCommand("START_SERVO");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error crítico al abrir el puerto: {e.Message}");
        }
    }

    void Update()
    {
        Debug.LogError("El método Update se está ejecutando."); // Confirmación de que Update corre

        if (serialPort != null && serialPort.IsOpen)
        {
            // Leer datos desde el Arduino
            if (serialPort.BytesToRead > 0)
            {
                try
                {
                    string data = serialPort.ReadLine().Trim(); // Leer la respuesta
                    Debug.LogError($"Recibido: {data}");

                    // Manejar comandos del Arduino
                    if (data == "BUTTON_PRESSED")
                    {
                        Debug.Log("El botón físico fue presionado en el Arduino.");
                        SimulateLeftClick(); // Simula un click izquierdo en el juego
                        receivedButtonPressed = true;

                        // Notificar al AntiAircraftBattery que dispare
                        if (antiAircraftBattery != null)
                        {
                            antiAircraftBattery.ShootBullet(); // Llama directamente a ShootBullet
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error al leer datos: {e.Message}");
                }
            }
            

            // Enviar comandos manualmente desde Unity
            if (Input.GetMouseButtonDown(0)) // Detectar click izquierdo del mouse
            {
                SendCommand("FIRE");
            }
            if (Input.GetKeyDown(KeyCode.M)) // Comando para mover el servo (opcional)
            {
                SendCommand("MOVE:90");
            }
        }
    }

    void SimulateLeftClick()
    {
        Debug.Log("¡Disparo Simulado por el Botón Físico!");
        // Aquí puedes reutilizar el código que usas para el disparo, por ejemplo:
        SendCommand("FIRE"); // Enviar "FIRE" como si fuera el click izquierdo
    }

    void SendCommand(string command)
    {
        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.WriteLine(command);
                Debug.Log($"Comando enviado: {command}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al enviar el comando '{command}': {e.Message}");
        }
    }

    // Simula el disparo usando tu prefab de proyectil existente
    void SimulateFire()
    {
        Debug.Log("¡Disparo Real!"); // Confirmación de disparo

        if (bulletPrefab != null && firePoint != null)
        {
            // Instanciar el proyectil
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Darle velocidad al proyectil
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.right * bulletSpeed; // Ajusta según la dirección en tu juego
            }

            // Destruir el proyectil después de un tiempo
            Destroy(bullet, 5f); // Elimina el proyectil tras 5 segundos
        }
        else
        {
            Debug.LogWarning("Falta el prefab del proyectil o el punto de disparo.");
        }
    }

    // Simula el movimiento del objeto servo
    void SimulateMoveServo(int angle)
    {
        if (servoObject != null)
        {
            // Rotar el objeto para simular el movimiento del servo
            servoObject.rotation = Quaternion.Euler(0, 0, angle);
            Debug.Log($"Servo movido visualmente a {angle} grados.");
        }
        else
        {
            Debug.LogWarning("Falta el objeto del servo.");
        }
    }

    private void OnApplicationQuit()
    {
        // Enviar el comando para detener el movimiento del servo al cerrar el juego
        SendCommand("STOP_SERVO");

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.LogError("Puerto serial cerrado.");
        }
    }
}
