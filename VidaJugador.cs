using UnityEngine;
using UnityEngine.UI; // Para la interfaz
using UnityEngine.SceneManagement; // Para reiniciar si mueres

public class VidaJugador : MonoBehaviour
{
    [Header("Ajustes de Salud")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Interfaz (Opcional)")]
    public Slider barraVida; // Si tienes un Slider en el UI
    public GameObject pantallaMuerte; // Un panel que diga "Moriste"

    void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarUI();
    }

    // Esta es la función que llamará la Bruja
    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("<color=red>Jugador herido. Vida restante: </color>" + vidaActual);

        ActualizarUI();

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void ActualizarUI()
    {
        if (barraVida != null)
        {
            barraVida.value = vidaActual / vidaMaxima;
        }
    }

    void Morir()
    {
        Debug.Log("El jugador ha muerto en el mercado.");
        if (pantallaMuerte != null)
        {
            pantallaMuerte.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
        else
        {
            // Si no tienes pantalla de muerte, reinicia la escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Curar(float cantidad)
    {
        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMaxima);
        ActualizarUI();
    }
}