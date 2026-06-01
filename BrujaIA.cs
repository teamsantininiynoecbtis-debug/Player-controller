using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class BrujaIA : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public List<Transform> puntosPatrulla; 
    public float velocidadPatrulla = 1.8f;
    public float velocidadPersecucion = 3.5f;
    public float distanciaDeteccion = 10f; // Si estas mas cerca de esto, te sigue
    
    [Header("Configuracion de Efectos")]
    public float tiempoMiradaNecesario = 3f;
    public float anguloVision = 35f;

    [Header("Referencias")]
    public GameObject jugador;
    public Camera camaraJugador;
    public GameObject panelCeguera;

    private NavMeshAgent agente;
    private int indicePatrulla = 0;
    private bool estaPersiguiendo = false;
    private float cronometroEfecto = 0f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (camaraJugador == null) camaraJugador = Camera.main;
        
        // Empezar patrullando
        IrAlSiguientePunto();
    }

    void Update()
    {
        if (jugador == null || agente == null) return;

        // Calculamos la distancia real entre la Bruja y el Jugador
        float distanciaActual = Vector3.Distance(transform.position, jugador.transform.position);

        if (distanciaActual <= distanciaDeteccion)
        {
            // --- ESTADO: PERSECUCION ---
            if (!estaPersiguiendo)
            {
                Debug.Log("<color=red>¡BRUJA: Te vi! Iniciando persecución.</color>");
                estaPersiguiendo = true;
            }

            agente.speed = velocidadPersecucion;
            agente.SetDestination(jugador.transform.position);
            
            // Lógica de ataque (si la miras mientras te sigue)
            ManejarAtaquePorMirada();
        }
        else
        {
            // --- ESTADO: PATRULLA ---
            if (estaPersiguiendo)
            {
                Debug.Log("<color=green>¡BRUJA: Te perdiste. Regresando a patrulla.</color>");
                estaPersiguiendo = false;
                cronometroEfecto = 0f; // Resetea el ataque
                IrAlSiguientePunto();
            }

            agente.speed = velocidadPatrulla;

            // Si llega al punto de patrulla, va al siguiente
            if (!agente.pathPending && agente.remainingDistance < 0.6f)
            {
                IrAlSiguientePunto();
            }
        }
    }

    void IrAlSiguientePunto()
    {
        if (puntosPatrulla.Count == 0) return;
        
        agente.SetDestination(puntosPatrulla[indicePatrulla].position);
        indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Count;
    }

    void ManejarAtaquePorMirada()
    {
        if (EstaSiendoMirada())
        {
            cronometroEfecto += Time.deltaTime;
            if (cronometroEfecto >= tiempoMiradaNecesario)
            {
                LanzarEfectoAleatorio();
                cronometroEfecto = 0f;
            }
        }
        else
        {
            cronometroEfecto = Mathf.Max(0, cronometroEfecto - Time.deltaTime);
        }
    }

    bool EstaSiendoMirada()
    {
        Vector3 direccion = transform.position - camaraJugador.transform.position;
        float angulo = Vector3.Angle(camaraJugador.transform.forward, direccion);

        if (angulo < anguloVision)
        {
            RaycastHit hit;
            if (Physics.Raycast(camaraJugador.transform.position, direccion, out hit, 50f))
            {
                if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void LanzarEfectoAleatorio()
    {
        int suerte = Random.Range(1, 101);
        if (suerte <= 40) StartCoroutine(EfectoLentitud());
        else if (suerte <= 80) StartCoroutine(EfectoCeguera());
        else 
        {
            VidaJugador vida = jugador.GetComponent<VidaJugador>();
            if (vida != null) vida.RecibirDanio(75f);
        }
    }

    IEnumerator EfectoLentitud()
    {
        Debug.Log("Lentitud aplicada");
        yield return new WaitForSeconds(60f);
    }

    IEnumerator EfectoCeguera()
    {
        if (panelCeguera != null)
        {
            panelCeguera.SetActive(true);
            yield return new WaitForSeconds(20f);
            panelCeguera.SetActive(false);
        }
    }
}