graph TB

    subgraph "🎮 Core Layer - Modèles & Logique Métier"
        Card["<b>Card</b><br/>Modèle de carte<br/>(données)"]
        Hand["<b>Hand</b><br/>Collection de cartes<br/>Gestion main"]
        Deck["<b>Deck</b><br/>Pile de cartes<br/>(future utilisation)"]
        CardConfig["<b>CardConfiguration</b><br/>Template de carte"]
    end
    
    subgraph "⚙️ Commands Pattern"
        ICommand["<b>ICommand</b><br/>Interface Command"]
        DrawHandCommand["<b>DrawHandCommand</b><br/>Piocher des cartes"]
    end
    
    subgraph "🎬 Controllers - Coordination"
        HandController["<b>HandController</b><br/>Orchestrateur principal<br/>Gestion input G"]
    end
    
    subgraph "👁️ Views - Affichage"
        HandView["<b>HandView</b><br/>Layout en éventail<br/>Instanciation cartes"]
    end
    
    subgraph "🎭 GameObject Components"
        CardData["<b>CardData</b><br/>Données runtime<br/>Sprites, Collider"]
        CardInteraction["<b>CardInteraction</b><br/>Input souris<br/>Hover, Click, Drag"]
        CardStateMachine["<b>CardStateMachine</b><br/>Gestion états carte"]
        CardAnimator["<b>CardAnimator</b><br/>Animations<br/>Position, Scale"]
    end
    
    subgraph "🔄 State Machine"
        IdleState["<b>CardIdleState</b><br/>Repos"]
        HoverState["<b>CardHoverState</b><br/>Survol<br/>+Sorting Order"]
        SelectedState["<b>CardSelectedState</b><br/>Sélection"]
        DraggingState["<b>CardDraggingState</b><br/>Drag & Drop<br/>Tilt rotation"]
        CardTiltSettings["<b>CardTiltSettings</b><br/>Config rotation"]
    end
    
    subgraph "📡 Event System"
        CardEventBus["<b>CardEventBus</b><br/>Médiateur événements<br/>Découplage"]
    end
    
    %% Flux principal
    HandController -->|"1. Execute()"| DrawHandCommand
    DrawHandCommand -->|"2. AddCard()"| Hand
    DrawHandCommand -->|"3. UpdateDisplay()"| HandView
    HandView -->|"4. Instantiate prefab"| CardData
    
    %% Interactions
    CardInteraction -->|"Détecte input"| CardStateMachine
    CardStateMachine -->|"Change état"| IdleState
    CardStateMachine -->|"Change état"| HoverState
    CardStateMachine -->|"Change état"| SelectedState
    CardStateMachine -->|"Change état"| DraggingState
    
    %% Animations
    HoverState -->|"AnimateHover()"| CardAnimator
    DraggingState -->|"Position + Rotation"| CardAnimator
    DraggingState -.->|"Lit config"| CardTiltSettings
    
    %% Event Bus
    CardInteraction -->|"RaiseRemoveCard()"| CardEventBus
    DraggingState -->|"RaiseUpdateCardIndex()"| CardEventBus
    CardEventBus -->|"Events"| HandController
    HandController -->|"RemoveCard()"| Hand
    
    %% Data flow
    CardConfig -.->|"Template"| Card
    Card -.->|"Référence"| CardData
    HandController -->|"Commande clavier G"| DrawHandCommand
    
    style Card fill:#e1f5ff
    style Hand fill:#e1f5ff
    style Deck fill:#e1f5ff
    style CardConfig fill:#e1f5ff
    
    style ICommand fill:#fff4e1
    style DrawHandCommand fill:#fff4e1
    
    style HandController fill:#e8f5e9
    
    style HandView fill:#f3e5f5
    
    style CardEventBus fill:#ffebee
    
    style CardData fill:#fce4ec
    style CardInteraction fill:#fce4ec
    style CardStateMachine fill:#fce4ec
    style CardAnimator fill:#fce4ec
    
    style IdleState fill:#e0f2f1
    style HoverState fill:#e0f2f1
    style SelectedState fill:#e0f2f1
    style DraggingState fill:#e0f2f1
    style CardTiltSettings fill:#e0f2f1