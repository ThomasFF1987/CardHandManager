using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// INPUTHANDLER - Gestion des inputs clavier pour la main
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - Écoute les inputs clavier (G, H, etc.)
/// - Émet des événements vers les contrôleurs
/// - Pattern Observer pour découpler input et logique
/// 
/// 📦 RESPONSABILITÉS :
/// - Détecter la touche G → DrawHand event
/// - Futures : H pour shuffle, D pour discard, etc.
/// 
/// 💡 AVANTAGES :
/// - Respect du SRP (Single Responsibility Principle)
/// - Facile de changer les keybindings
/// - Testable sans MonoBehaviour
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class InputHandler : MonoBehaviour
{
    public System.Action OnDrawHandRequested;
    public System.Action OnShuffleHandRequested;
    
    private void Update()
    {
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            OnDrawHandRequested?.Invoke();
        }
        
        // Futures inputs
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            OnShuffleHandRequested?.Invoke();
        }
    }
}