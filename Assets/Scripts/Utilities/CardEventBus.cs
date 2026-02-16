using System;
using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDEVENTBUS - Médiateur centralisé pour les événements de cartes
/// ═══════════════════════════════════════════════════════════════════════════
//
// 🎯 RÔLE :
// - Bus d'événements global (Singleton) pour la communication découplée
// - Permet aux composants de communiquer sans références directes
// - Pattern Observer/Mediator pour réduire le couplage
//
// 📦 RESPONSABILITÉS :
// - RaiseRemoveCard() : Notifie la suppression d'une carte
// - RaiseHandLayoutUpdate() : Demande un refresh du layout
// - RaiseUpdateCardIndex() : Notifie le changement de position (drag)
// - RaiseCardHovered/Unhovered() : Événements de survol (futurs)
// - RaiseCardSelected/Deselected() : Événements de sélection (futurs)
//
// 🔗 ÉMETTEURS (Publishers) :
// - CardInteraction : Envoie RemoveCard, HandLayoutUpdate
// - CardDraggingState : Envoie UpdateCardIndex pendant le drag
//
// 🔗 RÉCEPTEURS (Subscribers) :
// - HandController : S'abonne à tous les événements pour gérer la main
//
// 📊 FLUX D'ÉVÉNEMENT :
// CardInteraction.HandleMouseUp() → RaiseRemoveCard() → HandController.OnRemoveCardRequested()
// CardDraggingState.OnUpdate() → RaiseUpdateCardIndex() → HandController.OnUpdateCardIndexRequested()
//
// 💡 CE QUE VOUS POUVEZ FAIRE :
// - Ajouter des événements : CardPlayed, CardDiscarded, CardDrawn
// - Implémenter un système de log/replay des événements
// - Créer des listeners pour l'UI (score, mana, etc.)
// - Ajouter des événements pour le son/musique
// - Implémenter un système de networking (multiplayer)
// - Logger les événements pour analytics/debug
//
// ⚠️ AVANTAGES :
// - ✅ Découplage fort : Les composants ne se connaissent pas
// - ✅ Extensibilité : Facile d'ajouter de nouveaux listeners
// - ✅ Testabilité : Mock le bus pour les tests unitaires
// - ✅ Protection mémoire : Gestion du isQuitting pour éviter les leaks
//
// 🏗️ PATTERN :
// Event Bus / Mediator Pattern - Centralise la communication entre
// composants indépendants via un bus d'événements global.
// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class CardEventBus : Singleton<CardEventBus>
{
    // Événements
    public event Action HandLayoutToUpdate;
    public event Action<GameObject> RemoveCard;
    public event Action<GameObject, Vector3> UpdateCardIndex;
    public event Action<GameObject> CardSelected;
    public event Action<GameObject> CardDeselected;
    public event Action<GameObject> CardHovered;
    public event Action<GameObject> CardUnhovered;

    private bool isQuitting = false;

    // Méthodes pour déclencher les événements de manière sécurisée

    /// <summary>
    /// Demande la mise à jour du layout de la main.
    /// </summary>
    public void RaiseHandLayoutUpdate()
    {
        if (isQuitting) return;
        HandLayoutToUpdate?.Invoke();
    }

    /// <summary>
    /// Demande la suppression d'une carte de la main.
    /// </summary>
    /// <param name="card">GameObject de la carte à supprimer</param>
    public void RaiseRemoveCard(GameObject card)
    {
        if (isQuitting) return;
        
        if (card == null)
        {
            Debug.LogWarning("Tentative de supprimer une carte null.");
            return;
        }
        RemoveCard?.Invoke(card);
    }

    /// <summary>
    /// Notifie le changement de position d'une carte (pour réorganisation).
    /// </summary>
    /// <param name="card">GameObject de la carte</param>
    /// <param name="position">Nouvelle position</param>
    public void RaiseUpdateCardIndex(GameObject card, Vector3 position)
    {
        if (isQuitting) return;
        
        if (card == null)
        {
            Debug.LogWarning("Tentative de mettre à jour l'index d'une carte null.");
            return;
        }
        UpdateCardIndex?.Invoke(card, position);
    }

    /// <summary>
    /// Notifie qu'une carte a été sélectionnée.
    /// </summary>
    public void RaiseCardSelected(GameObject card)
    {
        if (isQuitting || card == null) return;
        CardSelected?.Invoke(card);
    }

    /// <summary>
    /// Notifie qu'une carte a été désélectionnée.
    /// </summary>
    public void RaiseCardDeselected(GameObject card)
    {
        if (isQuitting || card == null) return;
        CardDeselected?.Invoke(card);
    }

    /// <summary>
    /// Notifie qu'une carte est survolée par la souris.
    /// </summary>
    public void RaiseCardHovered(GameObject card)
    {
        if (isQuitting || card == null) return;
        CardHovered?.Invoke(card);
    }

    /// <summary>
    /// Notifie qu'une carte n'est plus survolée.
    /// </summary>
    public void RaiseCardUnhovered(GameObject card)
    {
        if (isQuitting || card == null) return;
        CardUnhovered?.Invoke(card);
    }

    /// <summary>
    /// Nettoie tous les abonnements (utile pour éviter les fuites mémoire).
    /// </summary>
    public void ClearAllSubscriptions()
    {
        HandLayoutToUpdate = null;
        RemoveCard = null;
        UpdateCardIndex = null;
        CardSelected = null;
        CardDeselected = null;
        CardHovered = null;
        CardUnhovered = null;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        isQuitting = true;
        ClearAllSubscriptions();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        isQuitting = true;
        ClearAllSubscriptions();
    }
}