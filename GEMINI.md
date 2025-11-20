### Project Overview

This is a Unity-based Blackjack game. The project is structured into three main parts:

*   **UI:** The `UIManager.cs` script manages all UI elements and user interactions. It's responsible for handling button clicks, updating the display, and managing the overall UI flow.
*   **Game Logic:** The `BJController.cs` script contains the core game logic for Blackjack. It manages the game state, including the deck, player and dealer hands, betting, and game rules.
*   **API (Placeholder):** The `SocketIOManager.cs` script is currently empty, suggesting that the networking functionality is not yet implemented.

The game uses the DOTween library for animations.

### Building and Running

As a Unity project, you can build and run it using the Unity Editor.

1.  Open the project in the Unity Editor.
2.  Open the `MainScene` file from the `Assets/Scenes` directory.
3.  Press the "Play" button in the Unity Editor to run the game.

### Development Conventions

*   The code is written in C#.
*   The project follows a clear separation of concerns between UI, game logic, and API layers.
*   The code makes extensive use of the `SerializeField` attribute to expose variables to the Unity Editor, which is a standard practice in Unity development.
*   The project uses the DOTween library for animations.
*   The `SocketIOManager.cs` file is a placeholder, indicating that the networking functionality is not yet implemented.
