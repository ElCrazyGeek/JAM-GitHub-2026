using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum LightState { Green, Yellow, Red }
    public LightState currentState = LightState.Red;

    [Header("Luces Visuales (Opcional)")]
    public Renderer greenBulb;
    public Renderer yellowBulb;
    public Renderer redBulb;

    private void Start()
    {
        UpdateVisuals();
    }

    // Cambia de estado al hacer clic sobre el semáforo (requiere un Collider)
    private void OnMouseDown()
    {
        CycleState();
    }

    public void CycleState()
    {
        currentState = (LightState)(((int)currentState + 1) % 3);
        UpdateVisuals();
    }

    public bool CanPass(bool ignoreYellow)
    {
        if (currentState == LightState.Green) return true;
        if (currentState == LightState.Yellow && ignoreYellow) return true;
        return false;
    }

    private void UpdateVisuals()
    {
        if (greenBulb != null) greenBulb.material.color = currentState == LightState.Green ? Color.green : Color.gray;
        if (yellowBulb != null) yellowBulb.material.color = currentState == LightState.Yellow ? Color.yellow : Color.gray;
        if (redBulb != null) redBulb.material.color = currentState == LightState.Red ? Color.red : Color.gray;
    }
}