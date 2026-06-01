using UnityEngine;
using System.Collections;

public class GestorInventarioCompleto : MonoBehaviour
{
    [Header("Configuración de Inventario")]
    public int[] slots = new int[3]; // 0 = vacío, 1-7 = IDs
    public PlayerController movimientoJugador; 
    
    [Header("Referencias")]
    public GameObject panelInventario;
    public Animator animadorBrazo;

    // --- LÓGICA DE RECOGIDA ---
    public void RecogerObjeto(int id, int peso)
    {
        if (peso == 2 && ContarSlotsVacios() >= 2)
        {
            OcuparSlots(id, 2);
            // Penalización al recoger carne
            if (movimientoJugador != null) movimientoJugador.multiplicadorVelocidad = 0.7f;
            Debug.Log("Carne recogida: Velocidad reducida.");
        }
        else if (peso == 1 && ContarSlotsVacios() >= 1)
        {
            OcuparSlots(id, 1);
        }
        else { Debug.Log("No hay suficiente espacio."); return; }

        MostrarUI();
    }

    // --- LÓGICA DE SELECCIÓN (La que mueve el brazo) ---
    public void SeleccionarObjeto(int indice)
    {
        if (indice < 0 || indice >= 3 || slots[indice] == 0) return;

        // Enviamos el ID al Animator
        if (animadorBrazo != null)
        {
            animadorBrazo.SetInteger("IDObjeto", slots[indice]);
            // Opcional: si tu Animator usa un trigger, descomenta la siguiente línea
            // animadorBrazo.SetTrigger("Seleccionar"); 
        }

        MostrarUI();
    }

    // --- LÓGICA DE USO ---
    public void UsarObjeto(int indice)
    {
        int id = slots[indice];
        if (id == 0) return;

        // Si es carne, quitamos la penalización al usarla
        if (id == 2 && movimientoJugador != null) 
            movimientoJugador.multiplicadorVelocidad = 1.0f;

        // Aquí irían los efectos de Cahuama, Coca, etc.
        Debug.Log("Usando objeto ID: " + id);

        slots[indice] = 0; // Vaciamos el slot
        MostrarUI();
    }

    // --- MÉTODOS AUXILIARES ---
    private int ContarSlotsVacios()
    {
        int c = 0;
        foreach (int s in slots) if (s == 0) c++;
        return c;
    }

    private void OcuparSlots(int id, int cantidad)
    {
        int ocupados = 0;
        for (int i = 0; i < slots.Length && ocupados < cantidad; i++)
        {
            if (slots[i] == 0) { slots[i] = id; ocupados++; }
        }
    }

    private void MostrarUI()
    {
        if (panelInventario != null)
        {
            panelInventario.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(OcultarTrasEspera());
        }
    }

    IEnumerator OcultarTrasEspera()
    {
        yield return new WaitForSeconds(3f);
        panelInventario.SetActive(false);
    }
}