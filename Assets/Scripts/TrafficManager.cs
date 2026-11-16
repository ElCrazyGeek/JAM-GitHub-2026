using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Configuración de Vehículos")]
    [Tooltip("Arrastra aquí todos tus prefabs de carros (Mazda, Pickup, etc.)")]
    public List<GameObject> vehiclePrefabs = new List<GameObject>();

    [Header("Puntos de Aparición (Entradas)")]
    [Tooltip("Arrastra los 'Punto Inicio' de cada una de las 4 calles")]
    public List<Waypoint> spawnWaypoints = new List<Waypoint>();

    [Header("Tiempos de Generación")]
    public float minSpawnInterval = 2.0f;
    public float maxSpawnInterval = 4.5f;

    [Header("Seguridad Anti-Solapamiento")]
    [Tooltip("Radio para verificar si hay un auto estorbando en la entrada antes de crear uno nuevo")]
    public float spawnCheckRadius = 3.5f;
    public LayerMask vehicleLayer; // La misma capa 'Vehiculo' que usas en VehicleAI

    private void Start()
    {
        if (vehiclePrefabs.Count == 0 || spawnWaypoints.Count == 0)
        {
            Debug.LogWarning("TrafficSpawner: Faltan prefabs de vehículos o puntos de spawn asignados.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Espera un tiempo aleatorio para que el tráfico no sea robótico
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            TrySpawnVehicle();
        }
    }

    private void TrySpawnVehicle()
    {
        // 1. Elegir una calle de entrada al azar
        Waypoint chosenSpawn = spawnWaypoints[Random.Range(0, spawnWaypoints.Count)];

        // 2. Verificar que no haya un carro atascado justo en el punto de inicio
        bool isBlocked = Physics.CheckSphere(chosenSpawn.transform.position, spawnCheckRadius, vehicleLayer);
        if (isBlocked)
        {
            // Si la calle está llena hasta el tope, saltamos este turno para no encimar autos
            return;
        }

        // 3. Elegir un modelo de auto al azar
        GameObject prefabToSpawn = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)];

        // 4. Calcular la rotación inicial mirando hacia el siguiente punto del camino
        Quaternion spawnRotation = chosenSpawn.transform.rotation;
        Waypoint nextPoint = chosenSpawn.GetNextWaypoint();
        if (nextPoint != null)
        {
            Vector3 direction = (nextPoint.transform.position - chosenSpawn.transform.position).normalized;
            direction.y = 0; // Mantenerlo nivelado
            if (direction != Vector3.zero)
            {
                spawnRotation = Quaternion.LookRotation(direction);
            }
        }

        // 5. Instanciar el vehículo y asignarle su ruta
        GameObject newCar = Instantiate(prefabToSpawn, chosenSpawn.transform.position, spawnRotation);

        // Obtener el script de IA (usa el nombre de tu clase, ej. VehicleAI o IAVehiculos)
        VehicleAI carAI = newCar.GetComponent<VehicleAI>();
        if (carAI != null)
        {
            carAI.currentWaypoint = nextPoint != null ? nextPoint : chosenSpawn;
        }
    }

    // Dibuja en la escena el área de seguridad de cada punto de spawn
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        foreach (var wp in spawnWaypoints)
        {
            if (wp != null)
            {
                Gizmos.DrawSphere(wp.transform.position, spawnCheckRadius);
            }
        }
    }
}