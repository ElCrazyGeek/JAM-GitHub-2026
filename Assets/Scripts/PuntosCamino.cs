using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public enum TurnDirection { Straight, Right, Left }

    [Header("Tipo de Maniobra")]
    public TurnDirection direction = TurnDirection.Straight;

    [Header("Conexiones de Ruta")]
    public List<Waypoint> nextWaypoints = new List<Waypoint>();

    [Header("Control de Semáforo (Opcional)")]
    public TrafficLight trafficLight;

    public Waypoint GetNextWaypoint(bool allowTurns = true)
    {
        if (nextWaypoints == null || nextWaypoints.Count == 0)
            return null;

        // Si el vehículo no tiene permitido girar, filtra solo los nodos rectos
        if (!allowTurns)
        {
            List<Waypoint> straightOptions = nextWaypoints.FindAll(wp => wp != null && wp.direction == TurnDirection.Straight);
            if (straightOptions.Count > 0)
                return straightOptions[Random.Range(0, straightOptions.Count)];
        }

        return nextWaypoints[Random.Range(0, nextWaypoints.Count)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = trafficLight != null ? Color.red : Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.35f);

        foreach (var next in nextWaypoints)
        {
            if (next != null)
            {
                // Colorea cada ruta distinto en el editor para no confundirlas
                Gizmos.color = next.direction == TurnDirection.Straight ? Color.green :
                               next.direction == TurnDirection.Right ? Color.yellow : Color.magenta;
                Gizmos.DrawLine(transform.position, next.transform.position);
            }
        }
    }
}