using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Conexiones de Ruta")]
    [Tooltip("Siguiente(s) punto(s) en el carril. Si hay más de uno, el vehículo elige al azar.")]
    public List<Waypoint> nextWaypoints = new List<Waypoint>();

    [Header("Control de Semáforo (Opcional)")]
    [Tooltip("Asignar solo en el waypoint que está justo en la línea de alto antes del cruce.")]
    public TrafficLight trafficLight;

    public Waypoint GetNextWaypoint()
    {
        if (nextWaypoints == null || nextWaypoints.Count == 0)
            return null;

        int randomIndex = Random.Range(0, nextWaypoints.Count);
        return nextWaypoints[randomIndex];
    }

    // Dibuja la ruta en la vista de Escena para armar los carriles fácil
    private void OnDrawGizmos()
    {
        Gizmos.color = trafficLight != null ? Color.red : Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.35f);

        Gizmos.color = Color.yellow;
        foreach (var next in nextWaypoints)
        {
            if (next != null)
            {
                Gizmos.DrawLine(transform.position, next.transform.position);
            }
        }
    }
}