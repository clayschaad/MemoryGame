# Statistics Screen Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add a statistics screen accessible from the results screen showing word mastery progress with accuracy percentage, progress bars, and a reset option.

**Architecture:** Add a `_showStatistics` boolean flag to `Home.razor` that toggles a new view state within the existing `if/else` chain. Expose words from `GameService` via a new public method. Add `ResetStatisticsAsync()` to zero all counts.

**Tech Stack:** Blazor WebAssembly (.NET 10), C#, CSS (no additional dependencies)

---

### Task 1: Expose statistics words from GameService

**Files:**
- Modify: `MemoryGame/Services/GameService.cs`

**Step 1: Add public `GetWords()` method to GameService**

Add this method after `UpdateStatisticsAsync`:

```csharp
public IReadOnlyList<GameWord> GetWords()
{
    return _statistics?.Words ?? [];
}
```

**Step 2: Verify it compiles**

```bash
cd MemoryGame && dotnet build
```
Expected: Build succeeded, 0 errors.

**Step 3: Commit**

```bash
git add MemoryGame/Services/GameService.cs
git commit -m "feat: expose GetWords() on GameService for statistics screen"
```

---

### Task 2: Add ResetStatisticsAsync to GameService

**Files:**
- Modify: `MemoryGame/Services/GameService.cs`

**Step 1: Add `ResetStatisticsAsync()` method**

Add this method after `GetWords()`:

```csharp
public async Task ResetStatisticsAsync()
{
    if (_statistics == null)
        return;

    foreach (var word in _statistics.Words)
    {
        word.CorrectCount = 0;
        word.IncorrectCount = 0;
    }

    await _localStorage.SaveStatisticsAsync(_statistics);
}
```

**Step 2: Verify it compiles**

```bash
cd MemoryGame && dotnet build
```
Expected: Build succeeded, 0 errors.

**Step 3: Commit**

```bash
git add MemoryGame/Services/GameService.cs
git commit -m "feat: add ResetStatisticsAsync to GameService"
```

---

### Task 3: Add statistics view state and UI to Home.razor

**Files:**
- Modify: `MemoryGame/Pages/Home.razor`

**Step 1: Add `_showStatistics` field**

In the `@code` block, add after `_feedbackClass`:

```csharp
private bool _showStatistics;
```

**Step 2: Add "Statistik anzeigen" button to the results screen**

Find the results screen section (ends with `<button class="btn-start" @onclick="StartNewGame">Nochmal spielen</button>`).

Replace:
```razor
<button class="btn-start" @onclick="StartNewGame">Nochmal spielen</button>
```
With:
```razor
<button class="btn-start" @onclick="StartNewGame">Nochmal spielen</button>
<button class="btn-stats" @onclick="ShowStatistics">Statistik anzeigen</button>
```

**Step 3: Change results screen condition**

Find:
```razor
else if (_currentSession.IsComplete)
```
Replace with:
```razor
else if (_currentSession.IsComplete && !_showStatistics)
```

**Step 4: Add statistics screen section**

Add this new block after the results screen closing `}` and before the final `else`:

```razor
else if (_currentSession.IsComplete && _showStatistics)
{
    var words = GameService.GetWords()
        .OrderBy(w => w.CorrectCount + w.IncorrectCount == 0 ? 1 : 0)
        .ThenBy(w => w.CorrectCount + w.IncorrectCount == 0
            ? 0
            : (double)w.CorrectCount / (w.CorrectCount + w.IncorrectCount))
        .ToList();

    var playedWords = words.Where(w => w.CorrectCount + w.IncorrectCount > 0).ToList();
    var overallAccuracy = playedWords.Count == 0
        ? 0
        : (int)Math.Round(100.0 * playedWords.Sum(w => w.CorrectCount) / playedWords.Sum(w => w.CorrectCount + w.IncorrectCount));

    <div class="stats-screen">
        <h1>Statistik</h1>

        <div class="stats-summary">
            @playedWords.Count von @words.Count Wörtern geübt
            @if (playedWords.Count > 0)
            {
                <span> · @overallAccuracy% richtig</span>
            }
        </div>

        <div class="stats-list">
            @foreach (var word in words)
            {
                var total = word.CorrectCount + word.IncorrectCount;
                var accuracy = total == 0 ? 0 : (int)Math.Round(100.0 * word.CorrectCount / total);
                var barClass = total == 0 ? "bar-unplayed" : accuracy >= 70 ? "bar-green" : accuracy >= 40 ? "bar-orange" : "bar-red";

                <div class="stats-row">
                    <img src="@word.ImagePath" alt="@word.Word" class="stats-thumbnail" />
                    <div class="stats-word-info">
                        <span class="stats-word-name">@word.Word</span>
                        <div class="stats-bar-container">
                            <div class="stats-bar @barClass" style="width: @(total == 0 ? 0 : accuracy)%"></div>
                        </div>
                    </div>
                    <span class="stats-accuracy">@(total == 0 ? "—" : $"{accuracy}% richtig")</span>
                </div>
            }
        </div>

        @if (_confirmReset)
        {
            <div class="reset-confirm">
                <span>Alle Statistiken wirklich zurücksetzen?</span>
                <button class="btn-confirm-yes" @onclick="ConfirmReset">Ja, zurücksetzen</button>
                <button class="btn-confirm-no" @onclick="() => _confirmReset = false">Abbrechen</button>
            </div>
        }
        else
        {
            <button class="btn-reset" @onclick="() => _confirmReset = true">Zurücksetzen</button>
        }

        <button class="btn-start" @onclick="GoToStart">Zurück zum Start</button>
    </div>
}
```

**Step 5: Add `_confirmReset` field and helper methods**

In the `@code` block, add after `_showStatistics`:

```csharp
private bool _confirmReset;
```

Add these methods after `SelectAnswer`:

```csharp
private void ShowStatistics()
{
    _showStatistics = true;
}

private void GoToStart()
{
    _currentSession = null;
    _showStatistics = false;
    _confirmReset = false;
}

private async Task ConfirmReset()
{
    await GameService.ResetStatisticsAsync();
    _confirmReset = false;
    StateHasChanged();
}
```

**Step 6: Verify it compiles**

```bash
cd MemoryGame && dotnet build
```
Expected: Build succeeded, 0 errors.

**Step 7: Commit**

```bash
git add MemoryGame/Pages/Home.razor
git commit -m "feat: add statistics screen view state to Home.razor"
```

---

### Task 4: Add CSS styles for statistics screen

**Files:**
- Modify: `MemoryGame/wwwroot/css/app.css`

**Step 1: Add base statistics styles**

Append to the end of the base styles section (before the `@media (max-width: 768px)` block at line 335):

```css
/* Statistics Screen */
.stats-screen {
    width: 100%;
    max-width: 900px;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 20px;
    padding: 20px 0;
}

.stats-screen h1 {
    font-size: 2.2rem;
    color: #333;
    margin: 0;
}

.stats-summary {
    font-size: 1.2rem;
    color: #666;
}

.stats-list {
    width: 100%;
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.stats-row {
    display: flex;
    align-items: center;
    gap: 15px;
    padding: 12px;
    background-color: #f8f9fa;
    border: 2px solid #ddd;
    border-radius: 10px;
}

.stats-thumbnail {
    width: 80px;
    height: 80px;
    object-fit: contain;
    flex-shrink: 0;
}

.stats-word-info {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.stats-word-name {
    font-size: 1.3rem;
    font-weight: 600;
    color: #333;
    text-transform: capitalize;
}

.stats-bar-container {
    width: 100%;
    height: 14px;
    background-color: #e0e0e0;
    border-radius: 7px;
    overflow: hidden;
}

.stats-bar {
    height: 100%;
    border-radius: 7px;
    transition: width 0.4s ease;
    min-width: 0;
}

.bar-green  { background-color: #4CAF50; }
.bar-orange { background-color: #FF9800; }
.bar-red    { background-color: #f44336; }
.bar-unplayed { background-color: #e0e0e0; width: 0; }

.stats-accuracy {
    font-size: 1.1rem;
    font-weight: 600;
    color: #555;
    min-width: 110px;
    text-align: right;
    flex-shrink: 0;
}

.btn-stats {
    font-size: 1.2rem;
    padding: 12px 35px;
    background-color: #2196F3;
    color: white;
    border: none;
    border-radius: 10px;
    cursor: pointer;
    font-weight: bold;
    transition: background-color 0.3s;
    margin-top: 10px;
}

@media (hover: hover) and (pointer: fine) {
    .btn-stats:hover {
        background-color: #1976D2;
    }
}

.btn-stats:active {
    transform: scale(0.98);
}

.btn-reset {
    font-size: 1.1rem;
    padding: 10px 30px;
    background-color: #f44336;
    color: white;
    border: none;
    border-radius: 10px;
    cursor: pointer;
    font-weight: bold;
    transition: background-color 0.3s;
}

@media (hover: hover) and (pointer: fine) {
    .btn-reset:hover {
        background-color: #d32f2f;
    }
}

.reset-confirm {
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
    justify-content: center;
    font-size: 1.1rem;
    color: #333;
}

.btn-confirm-yes {
    padding: 10px 20px;
    background-color: #f44336;
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-weight: bold;
    font-size: 1rem;
}

.btn-confirm-no {
    padding: 10px 20px;
    background-color: #9e9e9e;
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-weight: bold;
    font-size: 1rem;
}
```

**Step 2: Add tablet-scale overrides for statistics screen**

Inside the existing `@media (min-width: 2500px) and (min-height: 1550px)` block, append:

```css
    .stats-screen {
        max-width: 2000px;
        gap: 35px;
        padding: 30px 0;
    }

    .stats-screen h1 {
        font-size: 4rem;
    }

    .stats-summary {
        font-size: 2rem;
    }

    .stats-list {
        gap: 20px;
    }

    .stats-row {
        gap: 25px;
        padding: 20px;
    }

    .stats-thumbnail {
        width: 140px;
        height: 140px;
    }

    .stats-word-name {
        font-size: 2.2rem;
    }

    .stats-bar-container {
        height: 22px;
        border-radius: 11px;
    }

    .stats-bar {
        border-radius: 11px;
    }

    .stats-accuracy {
        font-size: 2rem;
        min-width: 200px;
    }

    .btn-stats {
        font-size: 2.2rem;
        padding: 22px 65px;
    }

    .btn-reset {
        font-size: 2rem;
        padding: 18px 55px;
    }

    .reset-confirm {
        font-size: 2rem;
        gap: 20px;
    }

    .btn-confirm-yes, .btn-confirm-no {
        font-size: 1.8rem;
        padding: 18px 40px;
    }
```

**Step 3: Verify it builds**

```bash
cd MemoryGame && dotnet build
```
Expected: Build succeeded, 0 errors.

**Step 4: Commit**

```bash
git add MemoryGame/wwwroot/css/app.css
git commit -m "feat: add statistics screen CSS styles"
```

---

### Task 5: Manual smoke test

**Step 1: Run the app**

```bash
cd MemoryGame && dotnet run
```

Open browser at the URL shown (typically `http://localhost:5267`).

**Step 2: Verify the following flows**

- [ ] Play a full 10-round game
- [ ] Results screen shows "Statistik anzeigen" button
- [ ] Tapping it shows the statistics screen with all 20 words
- [ ] Played words show accuracy % and colored progress bar
- [ ] Unplayed words show "—" and are sorted to the bottom
- [ ] "Zurücksetzen" shows inline confirmation; cancelling hides it
- [ ] Confirming reset zeroes all counts and shows "—" for all words
- [ ] "Zurück zum Start" returns to the start screen
- [ ] Starting a new game and playing updates statistics correctly

**Step 3: Commit any fixes found during testing, then push**

```bash
git push -u origin feature/statistics-screen
```
