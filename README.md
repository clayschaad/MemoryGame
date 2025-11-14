# Memory Game

A Blazor WebAssembly application designed to help people with dementia practice image-to-word associations.

## Features

- **10-Round Game Sessions**: Each game consists of 10 rounds to provide a focused practice session
- **Dual Game Modes**:
  - Show an image with 4 word choices
  - Show a word with 4 image choices
- **Adaptive Learning**: 
  - Words answered correctly become less frequent in future games
  - Words answered incorrectly appear more often for additional practice
- **Progress Tracking**: Statistics are stored in browser local storage and persist across sessions
- **Tablet-Optimized UI**: Designed for 11" tablets with large, easy-to-tap buttons
- **Simple Vocabulary**: Uses everyday objects (fork, knife, door, ball, house, etc.)
- **Immediate Feedback**: Shows whether the answer was correct or incorrect after each round
- **Game Results**: Displays total correct and incorrect answers at the end of each game

## Technology Stack

- **.NET 10.0**: Using the latest .NET framework
- **Blazor WebAssembly**: Client-side web application that runs entirely in the browser
- **Local Storage**: Browser-based persistence for tracking progress across sessions
- **SVG Images**: Scalable vector graphics for clear display on all screen sizes

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Running the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/clayschaad/MemoryGame.git
   cd MemoryGame
   ```

2. Navigate to the project directory:
   ```bash
   cd MemoryGame
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to the URL shown in the console (typically `http://localhost:5000`)

### Building for Production

To build the application for deployment:

```bash
dotnet publish -c Release
```

The published files will be in `bin/Release/net10.0/publish/wwwroot/` and can be hosted on any static web server.

## How to Use

1. **Start Screen**: Click "Start New Game" to begin a new game session
2. **Game Rounds**: 
   - Read the question (image or word)
   - Select the correct answer from 4 options
   - Receive immediate feedback
   - Wait 2 seconds before automatically moving to the next round
3. **Results**: After 10 rounds, view your score and click "Play Again" to start a new game

## Game Statistics

The application tracks performance for each word/image pair:
- **Correct Count**: Number of times the word was answered correctly
- **Incorrect Count**: Number of times the word was answered incorrectly
- **Priority**: Calculated based on performance (higher priority = more frequent appearance)

This data is stored in browser local storage and persists across sessions, enabling the adaptive learning feature.

## Project Structure

```
MemoryGame/
├── Models/              # Data models for game state
│   ├── GameWord.cs
│   ├── GameSession.cs
│   ├── GameStatistics.cs
│   ├── Round.cs
│   └── RoundType.cs
├── Services/            # Business logic and storage
│   ├── GameService.cs
│   └── LocalStorageService.cs
├── Pages/               # Blazor pages
│   └── Home.razor
├── Layout/              # Layout components
│   └── MainLayout.razor
└── wwwroot/
    ├── images/          # SVG image files
    └── css/             # Stylesheets
```

## Adding New Words/Images

To add new words and images:

1. Add an SVG image to `wwwroot/images/` (name it `[word].svg`)
2. Update the `_defaultWords` list in `Services/GameService.cs`:
   ```csharp
   new() { Word = "newword", ImagePath = "images/newword.svg", CorrectCount = 0, IncorrectCount = 0 }
   ```

## Browser Compatibility

The application works in all modern browsers that support WebAssembly:
- Chrome/Edge (recommended)
- Firefox
- Safari
- Opera

## License

This project is open source and available under the MIT License.

## Acknowledgments

Built with Blazor WebAssembly for optimal performance and offline capability.
