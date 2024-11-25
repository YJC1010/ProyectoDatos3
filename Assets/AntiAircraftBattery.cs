using UnityEngine;

public class AntiAircraftBattery : MonoBehaviour
{
    public float speed = 2f;           // Velocidad de movimiento de la bater�a
    public float bulletSpeed = 5f;     // Velocidad inicial de la bala
    public GameObject bulletPrefab;    // Prefab de la bala
    public float maxHoldTime = 2f;     // Tiempo m�ximo que se puede mantener presionado para aumentar la velocidad de la bala

    private float holdTime = 0f;       // Tiempo que el jugador ha mantenido el click presionado
    private bool isMovingRight = true; // Direcci�n del movimiento (derecha o izquierda)
    private bool receivedButtonPressed = false; // Variable para rastrear el comando del bot�n
    


    void Update()
    {
        MoveBattery();
        HandleShooting();
    }

    void MoveBattery()
    {
        // Movimiento horizontal
        if (isMovingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= 5f) // L�mite derecho
                isMovingRight = false;
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= -5f) // L�mite izquierdo
                isMovingRight = true;
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0)) // Mantener presionado el bot�n izquierdo del mouse
        {
            holdTime += Time.deltaTime;
            holdTime = Mathf.Clamp(holdTime, 0f, maxHoldTime); // Limita el tiempo de espera
        }

        if (Input.GetMouseButtonUp(0)) // Soltar el bot�n izquierdo del mouse
        {

            ShootBullet();
            holdTime = 0f;
        }

        // Detectar el comando del Arduino
        if (receivedButtonPressed) // Variable que se activa cuando Arduino env�a "BUTTON_PRESSED"
        {
            ShootBullet(); // Disparar la bala
            holdTime = 0f;
            receivedButtonPressed = false; // Restablecer la variable para evitar disparos repetidos
        }
    }

    public void ShootBullet()
    {
        // Instanciar la bala en la posici�n de la bater�a
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Calcular la velocidad de la bala en funci�n del tiempo de espera
        float calculatedSpeed = bulletSpeed + (bulletSpeed * (holdTime / maxHoldTime));
        bullet.GetComponent<Rigidbody2D>().velocity = Vector2.up * calculatedSpeed;
    }
}
