Protocol Curator - Code Documentation

Project Overview
Protocol Curator is a first-person narrative exploration game built in Unity 6.0. The game features an artifact inspection system where players discover hidden truths by rotating museum artifacts to reveal their true nature beneath propaganda descriptions.

Technical Specifications
	•	Engine: Unity 6.0
	•	Programming Language: C#
	•	Input System: Unity New Input System
	•	Rendering Pipeline: Universal Render Pipeline (URP)
	•	Platform: PC (Windows/Mac/Linux)

Key Features
	•	First-person movement and camera control
	•	Interactive artifact pickup and inspection system
	•	Dynamic UI with propaganda/truth revelation mechanics
	•	Quest progression tracking
	•	Save/load system with PlayerPrefs
	•	Atmospheric audio management
	•	Main menu with continue/new game functionality
	•	End game sequence with completion tracking

System Architecture
Core Systems
Game Flow
├── Main Menu (MainMenuManager)
├── Intro Sequence (IntroSequenceManager)
├── Gameplay Loop
│   ├── Player Movement (PlayerMovement)
│   ├── Artifact Interaction (PickupSystem)
│   ├── Quest Management (QuestManager)
│   └── Pause System (PauseManager)
└── End Game (EndGameTrigger)
Data Flow
Player Input → PickupSystem → ArtifactData → QuestManager → UI Update
                    ↓
            ArtifactLevitation

Core Scripts
Player Systems
PlayerMovement.cs
Handles player locomotion, sprinting, jumping, and camera control.
Key Features:
	•	WASD movement with sprint toggle
	•	Mouse look with sensitivity control
	•	Jump mechanics with gravity
	•	Movement state management (can move, can look)

	•	Public Methods:

void OnMove(InputValue value)      // Movement input handler
void OnLook(InputValue value)      // Camera input handler
void OnJump(InputValue value)      // Jump input handler
void OnSprint(InputValue value)    // Sprint toggle handler

Dependencies:

	•	Unity's CharacterController
	•	Unity's New Input System

PickupSystem.cs
Core artifact interaction system with truth discovery mechanics.
Key Features:
	•	Raycast-based artifact detection
	•	Object pickup and rotation during inspection
	•	Zoom functionality (scroll wheel)
	•	Truth revelation through rotation alignment
	•	Quest integration
	•	Depth of field effects during inspection

Public Methods:
void OnInteract(InputValue value)         // E key handler
void OnLook(InputValue value)             // Rotation input during inspection
void OnZoom(InputValue value)             // Zoom input handler
void ForceSwapObject(ArtifactItem item)   // Programmatic object swap
GameObject GetHeldObject()                // Returns currently held object
ArtifactItem GetHeldArtifact()           // Returns held artifact component
void ResetPickupSystem()                  // Reset state (for new game)

Private Methods:
void TryPickUp()                    // Attempt to pick up artifact
void DropObject()                   // Release held object
void PrepareHeldObject(GameObject)  // Setup object for inspection
void RotateObject()                 // Handle rotation during inspection
void CheckForTruth()               // Verify if truth should be revealed
void RevealTruth(ArtifactData)     // Trigger glitch effect and reveal
void DisplayPropaganda(ArtifactData) // Show official description
void DisplayTruth(ArtifactData)    // Show truth (for solved artifacts)

Dependencies:
	•	ArtifactItem, ArtifactData, ArtifactLevitation
	•	QuestManager
	•	PlayerMovement
	•	URP Volume (Depth of Field)

Quest & UI Systems
QuestManager.cs
Singleton manager for artifact discovery tracking and finale triggering.
Key Features:
	•	Tracks 8 artifacts to discover
	•	Manages quest UI entries
	•	Triggers finale when all artifacts decoded
	•	Save/load integration
	•	Alarm system activation

Public Methods:
void ArtifactSolved(string artifactName)     // Mark artifact as solved
bool IsArtifactSolved(string artifactName)   // Check if solved
bool IsQuestComplete()                        // Returns true when all found
List<string> GetSolvedArtifactsList()        // For save system
void LoadProgress(List<string> artifacts)    // For load system

Singleton Access:
QuestManager.Instance.ArtifactSolved("Old Doll");

Dependencies:
	•	QuestEntry (UI component)
	•	SimpleNeonLogic (frame effect)
	•	DoorController, AlarmSystem (finale events)
	•	FinaleUIEffect

QuestEntry.cs
Individual UI element representing one artifact in the quest list.
Key Features:
	•	Displays "??????????" for undiscovered artifacts
	•	Animated decryption effect when discovered
	•	Color coding (gray → cyan)

Public Methods:
void InitializeEntry(string name)  // Setup with artifact name
void StartDecryption()             // Trigger reveal animation
Internal:
IEnumerator DecryptRoutine()       // Glitch animation coroutine
void ApplyTruthVisuals()           // Final appearance after decrypt

Scene Management
MainMenuManager.cs
Handles main menu UI, scene transitions, and save file management.
Key Features:
	•	New Game / Continue functionality
	•	Credits screen transition
	•	Game completion detection
	•	Disables Continue button when game completed
	•	Audio cleanup on menu return

Public Methods:
void StartNewGame()        // Deletes save, starts from beginning
void ContinueGame()        // Loads saved progress
void OpenCredits()         // Fade to credits screen
void CloseCredits()        // Return to main menu
void ExitGame()           // Quit application
void ResetToMenu()        // Return to menu from gameplay
Private Methods:
void UpdateContinueButton()                           // Checks save state
IEnumerator TransitionToGame(bool showIntro)         // Fade and load
IEnumerator FadeBetweenPanels(from, to)              // UI transitions

Dependencies:
	•	PauseManager, PlayerMovement
	•	PickupSystem (for reset)

IntroSequenceManager.cs
Manages the opening narrative sequence with controls display and story text.
Key Features:
	•	Controls panel display (4 seconds)
	•	Typewriter effect for narrative text
	•	Ambient sound management
	•	Smooth fade transitions

Configuration:
public float controlsDuration = 4f;              // How long controls show
public float narrativeTypingSpeed = 0.04f;       // Speed of typing effect
public float waitAfterTyping = 3f;               // Pause after text complete
public float fadeSpeed = 2f;                     // Fade transition speed

Internal Coroutine Flow:
FadeIn(Controls) → Wait → FadeOut(Controls) 
    → FadeIn(Narrative) → TypeText → Wait 
    → FadeOut(Narrative) → FadeOut(Black) 
    → Enable Player → Start Ambient Audio

Dependencies:
	•	PlayerInput, PlayerMovement
	•	PauseManager (enables at end)

PauseManager.cs
Handles pause menu, save/load functionality, and game state management.
Key Features:
	•	ESC key pause/resume
	•	Save game with visual feedback (pulsing icon)
	•	Load game with position and quest restoration
	•	Alarm system pause/resume integration

Public Methods:
void PauseGame()              // Freeze game, show menu
void ResumeGame()             // Resume gameplay
void SaveGame()               // Save position, rotation, quests
void LoadGame()               // Restore from PlayerPrefs
void ReturnToMainMenu()       // Return to menu, cleanup
void ExitGame()              // Save and quit
void OnPause(InputValue)      // Input handler
Save Data Format (PlayerPrefs):
"PlayerX", "PlayerY", "PlayerZ"    // Vector3 position
"PlayerRotY"                        // Y rotation
"SavedProgress"                     // Comma-separated artifact names
"AlarmActive"                       // 0 or 1

Private Methods:
IEnumerator FlashSaveIcon()   // Animated save feedback
void StartAmbientSounds()     // Resume audio after load

Dependencies:
	•	PlayerMovement, PickupSystem
	•	QuestManager
	•	AlarmSystem (optional)


Effects & Feedback

FinaleUIEffect.cs
Dramatic glitch effect when all artifacts are decoded.
Key Features:
	•	7-second glitch animation
	•	Screen shake and text scrambling
	•	Audio integration
	•	Smooth fade in/out

Public Methods:
void TriggerEffect(Color truthColor)  // Start the sequence

Configuration:
public float glitchDuration = 7f;      // Length of glitch effect
public float displayDuration = 4f;      // How long message stays
public float shakeIntensity = 12f;      // Screen shake strength
Internal Sequence:
FadeIn → Glitch Loop (7s) → Display Final Text → FadeOut

EndGameTrigger.cs
Handles end game sequence when player reaches escape zone.
Key Features:
	•	Trigger collider detection
	•	Typewriter effect for ending text
	•	Audio feedback
	•	Smooth transition to main menu
	•	Sets "GameCompleted" flag

Configuration:
public float typewriterSpeed = 0.05f;     // Letter typing speed
public float delayBeforeTyping = 1f;      // Pause before text
public float fadeSpeed = 0.8f;            // Fade transition speed
public float waitAfterText = 2f;          // Pause after completion
Flow:
OnTriggerEnter → Fade to Black → Freeze Player 
    → Stop Audio → Type Ending Text → Wait 
    → Fade Out → Set Completed Flag → Return to Menu

PlayerPrefs Written:
PlayerPrefs.SetInt("GameCompleted", 1);

ArtifactLevitation.cs
Creates floating and rotating animation for artifacts on pedestals.
Key Features:
	•	Sine wave vertical motion
	•	Constant Y-axis rotation
	•	Disables during player inspection
	•	Returns to original state on drop

Public Methods:
void ResetToPedestal()  // Called by PickupSystem when dropping

Public Variables:
bool isBeingHeld  // Set by PickupSystem to pause animation

Configuration:
public float amplitude = 0.05f;     // Vertical float distance
public float frequency = 1f;         // Float speed
public float rotationSpeed = 20f;    // Degrees per second


Script Dependencies
Dependency Graph
MainMenuManager
    ├── PauseManager
    ├── PlayerMovement
    └── PickupSystem

PauseManager
    ├── PlayerMovement
    ├── PickupSystem
    ├── QuestManager
    └── AlarmSystem (optional)

PickupSystem
    ├── PlayerMovement
    ├── ArtifactItem / ArtifactData
    ├── ArtifactLevitation
    ├── QuestManager
    ├── FinaleUIEffect
    └── DoorController

QuestManager
    ├── QuestEntry
    ├── SimpleNeonLogic
    ├── DoorController
    ├── AlarmSystem
    ├── FinaleUIEffect
    └── PauseManager

IntroSequenceManager
    ├── PlayerInput
    ├── PlayerMovement
    └── PauseManager

EndGameTrigger
    ├── MainMenuManager
    ├── PlayerMovement
    └── AudioSource

Required Components (on GameObjects)
Player:
	•	CharacterController
	•	PlayerMovement
	•	PickupSystem
	•	PlayerInput (New Input System)
	•	Camera (child object)

QuestManager (singleton):
	•	QuestManager
	•	Canvas with quest UI elements

Intro Manager:
	•	IntroSequenceManager
	•	Canvas with narrative panels

Main Menu:
	•	MainMenuManager
	•	Canvas with menu UI


Setup Instructions
1. Import Scripts
Place all .cs files in Assets/Scripts/

2. Configure Player
	1	Create Player GameObject with CharacterController
	2	Add PlayerMovement.cs
	3	Add PickupSystem.cs
	4	Create child Camera object
	5	Add PlayerInput component
	6	Assign Input Actions Asset

3. Setup Input Actions
Create Input Actions Asset with these actions:
	•	Move (Vector2) → WASD keys
	•	Look (Vector2) → Mouse Delta
	•	Jump (Button) → Space
	•	Sprint (Button) → Left Shift
	•	Interact (Button) → E key
	•	Pause (Button) → Escape
	•	Zoom (Vector2) → Mouse Scroll

4. Configure PickupSystem Inspector
References:
	•	Holding Point (empty Transform child of camera)
	•	Player Camera
	•	Global Volume (URP)
	•	Description UI (Canvas panel)
	•	Player Movement

UI Elements:
	•	Title Label (TextMeshProUGUI)
	•	Description Label (TextMeshProUGUI)
	•	Header Title (TextMeshProUGUI)
	•	Status Label (TextMeshProUGUI)
	•	Status Indicator (Image)
	•	Interaction Prompt (TextMeshProUGUI)

Audio:
	•	Glitch Audio Source
	•	Glitch Sound (AudioClip)
	•	UI Audio Source (optional)
	•	Hover Sound (optional)

Settings:
	•	Interaction Distance: 3
	•	Rotation Speed: 20
	•	Zoom Speed: 5
	•	Min Zoom Dist: 0.6
	•	Max Zoom Dist: 2.5

Colors:
	•	Propaganda Color: Red (#FF0000)
	•	Truth Color: Cyan (#00FFFF)

5. Create Artifact Prefabs
Each artifact needs:
	•	Collider
	•	Rigidbody (not kinematic)
	•	ArtifactItem.cs
	•	ArtifactData ScriptableObject assigned
	•	ArtifactLevitation.cs
	•	Layer: "Interactable"

6. Setup Quest Manager
	1	Create empty GameObject "QuestManager"
	2	Add QuestManager.cs
	3	Create Canvas with quest list UI
	4	Assign entry prefab with QuestEntry.cs

7. Configure Layers
Create these layers:
	•	Interactable (for artifacts)
	•	HeldObject (assigned during pickup)

8. Setup Scenes
Main Menu Scene:
	•	Canvas with MainMenuManager
	•	Continue/New Game buttons
Game Scene:
	•	Player
	•	QuestManager
	•	IntroSequenceManager (optional)
	•	Pause UI
	•	Artifacts in scene


Code Style & Best Practices
Naming Conventions
	•	Classes: PascalCase (PlayerMovement)
	•	Methods: PascalCase (CheckForTruth())
	•	Variables: camelCase (heldObject, isInspecting)
	•	Public Fields: camelCase (interactionDistance)
	•	Private Fields: camelCase (originalPos)
	•	Constants: UPPER_SNAKE_CASE (rare, prefer readonly)

Documentation

All major classes include:
	•	XML summary comments (/// <summary>)
	•	Method documentation where complex
	•	Header regions for organization

Unity-Specific Patterns
	•	Use [Header] attributes for Inspector organization
	•	Use [HideInInspector] for public fields not meant for editing
	•	Coroutines for timed sequences
	•	Singleton pattern for managers (QuestManager)
	•	FindFirstObjectByType for runtime references (Unity 6.0+)

Input System
	•	All input handled through New Input System
	•	OnAction(InputValue value) pattern
	•	InputValue.Get<T>() for reading values

Known Issues & Limitations
Current Limitations
	1	No networking support - single player only
	2	No gamepad support - keyboard/mouse only
	3	PlayerPrefs save system - not encrypted, easy to modify
	4	Hard-coded artifact names in QuestManager

Future Improvements
	•	Gamepad input support
	•	Encrypted save system (JSON or binary)
	•	Dynamic artifact list loading
	•	Mobile platform support
	•	Localization system

Additional Resources
External Dependencies
	•	Unity 6.0 - Game engine
	•	TextMeshPro - UI text rendering (Unity package)
	•	Universal Render Pipeline - Rendering (Unity package)
	•	Input System - Player input (Unity package)

Development Info
Developer: Martyna Milic (with AI assistance from Claude - Anthropic) 
Development Period: 2025-2026 
Engine Version: Unity 6.0 
Language: C# (.NET Standard 2.1)
AI-Assisted Development: All scripts were developed with assistance from Claude (Anthropic's AI assistant). Claude provided:
	•	Code architecture and design patterns
	•	Bug fixing and optimization
	•	Documentation and comments
	•	Unity best practices guidance

License
See main README.md for licensing information.
