using UnityEngine;

public class VehicleAI : MonoBehaviour
{
    [Header("Ruta")]
    public Waypoint currentWaypoint;
    public float waypointReachDistance = 0.6f;

    [Header("Parámetros Base (Conectar luego a DriverProfile)")]
    public float maxSpeed = 8f;
    public float turnSpeed = 6f;
    public float acceleration = 10f;
    public float brakeDeceleration = 20f;

    [Header("Sensores Raycast")]
    public Transform rayOrigin; // Punto vacío al frente del cofre
    public float detectionDistance = 4.5f;
    public LayerMask vehicleLayer; // Capa asignada a todos los vehículos

    [Header("Modificadores de Estado (Mood / Profile)")]
    public bool ignoreRedLight = false; // Activar cuando el conductor esté Enojado
    public bool ignoreYellowLight = true;

    private float currentSpeed = 0f;

    private void Update()
    {
        if (currentWaypoint == null)
        {
            // Llegó al final del carril: destruir o devolver al pool
            Destroy(gameObject);
            return;
        }

        bool mustStop = CheckTrafficLight() || CheckVehicleAhead();

        // Ajustar velocidad suavemente
        float targetSpeed = mustStop ? 0f : maxSpeed;
        float rate = mustStop ? brakeDeceleration : acceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

        MoveTowardsWaypoint();
    }

    private void MoveTowardsWaypoint()
    {
        Vector3 targetPos = currentWaypoint.transform.position;
        targetPos.y = transform.position.y; // Mantener altura constante

        Vector3 direction = (targetPos - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) <= waypointReachDistance)
        {
            currentWaypoint = currentWaypoint.GetNextWaypoint();
        }
    }

    private bool CheckTrafficLight()
    {
        if (ignoreRedLight || currentWaypoint.trafficLight == null)
            return false;

        // Solo frena si está cerca de la línea de alto y el semáforo no permite pasar
        float distToLine = Vector3.Distance(transform.position, currentWaypoint.transform.position);
        if (distToLine <= 6.5f && !currentWaypoint.trafficLight.CanPass(ignoreYellowLight))
        {
            return true;
        }

        return false;
    }

    private bool CheckVehicleAhead()
    {
        Vector3 origin = rayOrigin != null ? rayOrigin.position : transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, detectionDistance, vehicleLayer))
        {
            return true;
        }

        return false;
    }

    // Dibuja el láser de detección en la vista de Escena
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = rayOrigin != null ? rayOrigin.position : transform.position + Vector3.up * 0.5f;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(origin, origin + transform.forward * detectionDistance);
    }
}