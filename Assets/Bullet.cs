using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 5f;
    public GameObject explosionEffect;

    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        transform.Translate(Vector2.up * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Airplane"))
        {
            Airplane airplane = collision.GetComponent<Airplane>();
            if (airplane != null && !airplane.IsImmune())
            {
                if (gameController != null)
                {
                    gameController.DecreaseAirplaneCount(airplane); // Pasar el avión destruido al GameController
                }

                Destroy(collision.gameObject);

                if (explosionEffect != null)
                {
                    Instantiate(explosionEffect, transform.position, Quaternion.identity);
                }
            }

            Destroy(gameObject); // Destruir la bala en cualquier caso
        }
        else if (collision.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }

        else if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject); // Esto llamará automáticamente a OnDestroy()
        }
    }
}
