using UnityEngine;

[CreateAssetMenu(fileName = "CardTiltSettings", menuName = "Card/Tilt Settings")]
public class CardTiltSettings : ScriptableObject
{
    [Header("Tilt Intensity")]
    [Range(0f, 50f)]
    [Tooltip("Intensité de la rotation sur l'axe X (mouvement vertical)")]
    public float tiltIntensityX = 15f;
    
    [Range(0f, 50f)]
    [Tooltip("Intensité de la rotation sur l'axe Y (mouvement horizontal)")]
    public float tiltIntensityY = 15f;
    
    [Range(0f, 50f)]
    [Tooltip("Intensité de l'inclinaison sur l'axe Z (mouvement horizontal)")]
    public float tiltIntensityZ = 20f;
    
    [Header("Tilt Behavior")]
    [Range(1f, 20f)]
    [Tooltip("Vitesse de lissage des rotations")]
    public float tiltSmoothSpeed = 8f;
    
    [Range(10f, 60f)]
    [Tooltip("Angle maximum de rotation pour X et Y")]
    public float maxTiltAngleXY = 30f;
    
    [Range(10f, 90f)]
    [Tooltip("Angle maximum d'inclinaison pour Z")]
    public float maxTiltAngleZ = 45f;
}