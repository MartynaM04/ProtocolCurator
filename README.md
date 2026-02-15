Protocol Curator
A first-person narrative exploration game about uncovering hidden truths beneath propaganda. ￼

About The Game
Protocol Curator is an atmospheric first-person exploration game where you play as a visitor in a mysterious museum. Your mission: examine artifacts to discover the hidden truths concealed beneath official propaganda descriptions.

Game Features
• Interactive Artifact System: Pick up and rotate museum pieces to discover their true nature
• Dynamic Narrative: Watch as propaganda descriptions glitch and reveal hidden memories
• Quest Progression: Track your discoveries as you uncover 8 forbidden artifacts
• Save System: Continue your exploration from where you left off
• Immersive Controls: Smooth first-person movement with inspection mechanics
• Atmospheric Audio: Ambient soundscapes that enhance the mysterious atmosphere

How to Play
Move Forward/Back: W / S
Move Left/Right: A / D
Look Around: Mouse Movement
Jump: Space
Sprint: Left Shift (toggle)
Interact: E
Zoom (during inspection):Mouse Scroll
Pause Menu: Escape

Gameplay Loop
1. Explore the museum and locate artifacts marked with pedestals
2. Examine artifacts by pressing E when prompted
3. Rotate artifacts using your mouse to find the hidden angle
4. Discover the truth when you align the artifact correctly
5. Progress through the story as you uncover all 8 artifacts
6. Escape when you've learned the complete truth

Tips
• Look for glowing or floating artifacts - they can be inspected
• The "truth" is revealed at a specific rotation angle for each artifact
• Your progress is automatically saved after each discovery
• You can zoom in/out during inspection for a better view
• Check the quest panel (top corner) to track your progress

Objectives
Main Quest: "Decode the Truth"
Examine and decode all 8 forbidden artifacts:
• Old Doll
• Hourglass
• Mining Helmet
• Synth-Insulin Vial
• Hand of Tyrant
• Propaganda Book
• Copper Tree
• Access Card

Once all artifacts are decoded, find the terminal to enter the password and escape.

System Requirements
Minimum Requirements
• OS: Windows 7/8/10/11, macOS 10.13+, or Ubuntu 20.04+
• Processor: Intel Core i3 or equivalent
• Memory: 4 GB RAM
• Graphics: DirectX 11 compatible GPU
• Storage: 500 MB available space

Recommended Requirements
• OS: Windows 10/11, macOS 11+, or Ubuntu 22.04+
• Processor: Intel Core i5 or equivalent
• Memory: 8 GB RAM
• Graphics: NVIDIA GTX 1050 or AMD equivalent
• Storage: 1 GB available space

Installation
Option 1: Download Release Build
1. Download the latest release from the Releases page
2. Extract the ZIP file to your desired location
3. Run the executable file
4. Enjoy the game!

Option 2: Build from Source
1. Clone this repository
2. Open the project in Unity 6.0 or later
3. Open the main game scene (Assets/Scenes/MuseumScene.unity)
4. Click Play in the Unity Editor, or build for your platform (File → Build Settings)

Note: Building from source requires Unity 6.0 and the following packages:
• Universal Render Pipeline (URP)
• TextMeshPro
• Input System

Project Structure
Museum Project/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── MuseumScene.unity
│   │   └── ...
│   ├── Scripts/
│   │   ├── Player/
│   │   │   ├── PlayerMovement.cs
│   │   │   └── PickupSystem.cs
│   │   ├── Managers/
│   │   │   ├── QuestManager.cs
│   │   │   ├── PauseManager.cs
│   │   │   └── MainMenuManager.cs
│   │   ├── Artifacts/
│   │   │   ├── ArtifactItem.cs
│   │   │   ├── ArtifactData.cs
│   │   │   └── ArtifactLevitation.cs
│   │   └── UI/
│   │       ├── QuestEntry.cs
│   │       ├── FinaleUIEffect.cs
│   │       └── EndGameTrigger.cs
│   ├── Prefabs/
│   ├── Materials/
│   ├── Audio/
│   └── UI/
├── CODE_README.md          ← Technical documentation for developers
├── README.md               ← This file
└── LICENSE

Troubleshooting

Common Issues
Q: The game freezes when I press E
A: Make sure you're targeting an artifact (you should see "PRESS [E] TO EXAMINE" prompt). Clicking E in empty space should not freeze the game.

Q: I can't rotate the camera while inspecting.
A: This was fixed in v2.1. Make sure you're using the latest version of PickupSystem.cs.

Q: The description panel doesn't appear.
A: Ensure the Description UI field in the PickupSystem component (on Player) is properly assigned in the Inspector.

Q: Save game doesn't work.
A: The game uses Unity's PlayerPrefs system. Check that you have write permissions in your user directory. Progress is auto-saved after each artifact discovery.

Q: The game won't start.
A: Ensure you have the required packages installed (URP, TextMeshPro, Input System) and are using Unity 6.0 or later.

Getting Help
• Check the documentation files in the project folder
• Review CODE_README.md for technical details
• Submit an issue on GitHub (if applicable)

Credits

Development
• Lead Developer & Designer: Martyna Milic. All game code, mechanics, and design.
• AI Development Assistant: Claude (Anthropic). Code architecture, bug fixing, optimization, and technical guidance.

Tools & Technologies
• Unity 6.0 - Game engine
• C# - Programming language
• Universal Render Pipeline - Rendering system
• TextMeshPro - UI text rendering
• Unity Input System - Player input handling

Special Thanks
• Unity Technologies for the game engine and packages
• Anthropic for Claude AI assistant
• The Unity community for tutorials and resources

Copyright Notice
Protocol Curator
Copyright © 2025-2026 Martyna Milic
All rights reserved.

License
This project is released under the MIT License (see below).
You are free to:
• Use the code for learning purposes
• Modify and adapt the code for your own projects
• Share and distribute the code
• Use in commercial projects

Requirements:
• Include this copyright notice and license in any copies or substantial portions of the code
• Provide attribution to the original author (Martyna) and mention AI assistance by Claude

Disclaimer: THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

AI Assistance Disclosure
This project was developed with assistance from Claude, an AI assistant created by Anthropic. Claude provided:
• Code Architecture: Helped design and structure the game's code systems
• Implementation: Assisted in writing C# scripts and Unity components
• Debugging: Identified and fixed bugs throughout development
• Optimization: Suggested improvements and best practices
• Human Contribution: All creative decisions, game design, narrative content, and final implementation choices were made by Martyna. The AI assistant served as a coding partner and technical advisor.

Contact
Developer: Martyna Milic
Project Repository: https://github.com/MartynaM04/ProtocolCurator.git 
Bug Reports: martynamilic08@gmail.com

Acknowledgments
Special recognition to:
• Anthropic for creating Claude, the AI assistant that accelerated development
• Unity Technologies for the powerful and accessible game engine
• Everyone who playtests and provides feedback

Play & Enjoy!

Thank you for playing Protocol Curator.

Uncover the truth. Discover what's hidden. Question everything.

"The truth is not what they want you to believe..."

Made with Unity, C#, and AI-assisted development
Developed by Martyna Milic with Claude (Anthropic) | 2025-2026
￼ ￼ ￼
