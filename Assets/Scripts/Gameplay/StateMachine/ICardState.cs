using UnityEngine;

    /// <summary>
    /// Interface pour les états d'une carte
    /// </summary>
    public interface ICardState
    {
        /// <summary>
        /// Appelé lors de l'entrée dans cet état
        /// </summary>
        void OnEnter();
        
        /// <summary>
        /// Appelé à chaque frame pendant que cet état est actif
        /// </summary>
        void OnUpdate();
        
        /// <summary>
        /// Appelé lors de la sortie de cet état
        /// </summary>
        void OnExit();
        
        /// <summary>
        /// Nom de l'état pour le debug
        /// </summary>
        string StateName { get; }
    }
