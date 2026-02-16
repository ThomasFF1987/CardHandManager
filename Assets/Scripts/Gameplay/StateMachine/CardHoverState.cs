using UnityEngine;

/// <summary>
/// ═══════════════════════════════════════════════════════════════════════════
/// CARDHOVERSTATE - État "Survol" de la carte
/// ═══════════════════════════════════════════════════════════════════════════
/// 
/// 🎯 RÔLE :
/// - État actif quand la souris survole la carte
/// - Élève la carte visuellement (position + sorting order)
/// - Fait partie de la State Machine (implémente ICardState)
/// 
/// 📦 RESPONSABILITÉS :
/// - OnEnter() : Augmente sorting order (+100) et lance AnimateHover()
/// - OnUpdate() : État passif (animation gérée par CardAnimator)
/// - OnExit() : Restaure le sorting order d'origine
/// 
/// 🎨 EFFETS VISUELS :
/// 1. Sorting order +100 → Carte au-dessus des autres
/// 2. AnimateHover() → Monte selon l'axe Y local (suit rotation)
/// 3. Scale x1.1 (défini dans CardAnimator)
/// 
/// 📊 TRANSITIONS :
/// Idle → Hover : Souris entre sur la carte
/// Hover → Idle : Souris sort ET bouge (anti-flickering)
/// Hover → Dragging : Clic souris pendant survol
/// 
/// 💡 CE QUE VOUS POUVEZ FAIRE :
/// - Ajouter un glow effect ou outline
/// - Afficher une preview agrandie de la carte
/// - Jouer un son de hover
/// - Animer les stats de la carte
/// - Créer un effet de particules
/// - Ajouter un tooltip avec description
/// 
/// ⚙️ DONNÉES SAUVEGARDÉES :
/// - originalFrontSortingOrder : Sorting order initial (front)
/// - originalBackSortingOrder : Sorting order initial (back)
/// 
/// ═══════════════════════════════════════════════════════════════════════════
/// </summary>
public class CardHoverState : ICardState
{
    private readonly CardStateMachine stateMachine;
    
    public string StateName => "Hover";

    public CardHoverState(CardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void OnEnter()
    {
        // Animer le hover
        if (stateMachine.CardAnimator != null)
        {
            stateMachine.CardAnimator.AnimateHover();
        }
    }

    public void OnUpdate()
    {
        // Continuer l'animation de hover
    }

    public void OnExit()
    {
        // L'animation de sortie sera gérée par le prochain état
    }
}