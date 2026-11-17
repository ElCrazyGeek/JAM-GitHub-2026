using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cámaras de la Escena")]
    [Tooltip("Arrastra aquí todas las cámaras entre las que quieres alternar")]
    public List<Camera> cameras = new List<Camera>();

    [Header("Control de Teclado")]
    public KeyCode switchKey = KeyCode.LeftShift;

    [Header("Rotación en Eje Y con el Mouse")]
    [Tooltip("Grados máximos que puede girar hacia izquierda o derecha desde su posición original")]
    public float maxYawAngle = 35f;
    public float rotationSmoothness = 8f;
    [Tooltip("Si está activo, gira según la posición del cursor en pantalla sin necesidad de hacer clic")]
    public bool followCursorPosition = true;
    public float dragSensitivity = 0.2f;

    private int currentCameraIndex = 0;
    private float[] initialYRotations;
    private float targetYawOffset = 0f;
    private float currentYawOffset = 0f;

    private void Start()
    {
        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogWarning("CameraSwitcher: No hay cámaras asignadas en la lista.");
            return;
        }

        // Guardar la rotación inicial en Y de cada cámara para respetar su ángulo original
        initialYRotations = new float[cameras.Count];
        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i] != null)
            {
                initialYRotations[i] = cameras[i].transform.eulerAngles.y;
            }
        }

        ActivateCamera(0);
    }

    private void Update()
    {
        if (cameras.Count == 0) return;

        // 1. Cambiar de cámara con Shift
        if (Input.GetKeyDown(switchKey))
        {
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;
            ActivateCamera(currentCameraIndex);
        }

        // 2. Calcular rotación en Y según el mouse
        HandleMouseYaw();
    }

    private void HandleMouseYaw()
    {
        Camera activeCam = cameras[currentCameraIndex];
        if (activeCam == null) return;

        if (followCursorPosition)
        {
            // Normaliza la posición X del mouse de -1 (borde izquierdo) a +1 (borde derecho)
            float normalizedMouseX = Mathf.Clamp01(Input.mousePosition.x / Screen.width) * 2f - 1f;
            targetYawOffset = normalizedMouseX * maxYawAngle;
        }
        else if (Input.GetMouseButton(1)) // Clic derecho arrastrando (alternativa)
        {
            targetYawOffset += Input.GetAxis("Mouse X") * dragSensitivity * 10f;
            targetYawOffset = Mathf.Clamp(targetYawOffset, -maxYawAngle, maxYawAngle);
        }

        // Suavizado del giro
        currentYawOffset = Mathf.Lerp(currentYawOffset, targetYawOffset, Time.deltaTime * rotationSmoothness);

        // Aplicar solo en Y manteniendo intactos X (inclinación) y Z
        Vector3 currentEuler = activeCam.transform.eulerAngles;
        activeCam.transform.eulerAngles = new Vector3(
            currentEuler.x,
            initialYRotations[currentCameraIndex] + currentYawOffset,
            currentEuler.z
        );
    }

    private void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i] == null) continue;

            bool isSelected = (i == index);
            cameras[i].gameObject.SetActive(isSelected);

            // Asegurar que solo la cámara activa tenga AudioListener (para el audio 3D de los carros)
            AudioListener listener = cameras[i].GetComponent<AudioListener>();
            if (listener != null)
            {
                listener.enabled = isSelected;
            }
        }

        // Reiniciar el desplazamiento al cambiar de cámara
        currentYawOffset = 0f;
        targetYawOffset = 0f;
    }
}