# Statistics Screen Design

**Date:** 2026-02-24
**Feature:** Word mastery statistics screen
**Branch:** feature/statistics-screen

## Summary

Add a statistics screen accessible from the results screen that shows word mastery progress for all vocabulary words. Users can see how well they know each word and reset their statistics.

## Navigation

- Results screen gets a "Statistik anzeigen" button
- Tapping it switches to the new `Statistics` view state in `Home.razor`
- A "Zurück zum Start" button returns to the start screen
- No routing changes required

## Architecture

A new `Statistics` value is added to the existing view state enum in `Home.razor`. Statistics data is read directly from `GameService` (already loaded in memory). A new `ResetStatisticsAsync()` method is added to `GameService` that clears all `CorrectCount`/`IncorrectCount` values and persists to localStorage.

## Statistics Screen Layout

**Header:** Summary line showing total words played and overall accuracy.

**Word list:** Scrollable list of all 20 words, sorted by ascending accuracy (worst first). Words never played appear at the bottom.

Each row contains:
- Image thumbnail (~80px SVG)
- Word name (German)
- Accuracy percentage ("75% richtig") or "—" if never played
- Progress bar (fills green based on accuracy)

**Footer:**
- "Zurücksetzen" button (with confirmation) — resets all statistics
- "Zurück zum Start" button

## Mastery / Progress Bar Logic

- Accuracy = `CorrectCount / (CorrectCount + IncorrectCount)`
- Never played: shown last, bar at 0%, label "—"
- Bar color:
  - Green: ≥ 70% accuracy
  - Orange: 40–69% accuracy
  - Red: < 40% accuracy

## Sorting

Words sorted ascending by accuracy so the words needing most practice appear at the top. Never-played words are treated as 0% accuracy but sorted after all played words.

## Reset Flow

Tapping "Zurücksetzen" shows a browser `confirm()` dialog (or inline confirmation). On confirmation, `GameService.ResetStatisticsAsync()` zeros all counts and saves to localStorage. The list refreshes to show all words at 0 / "—".

## Files to Change

- `MemoryGame/Pages/Home.razor` — add Statistics view state and UI
- `MemoryGame/Services/GameService.cs` — add `ResetStatisticsAsync()` method
- `MemoryGame/wwwroot/css/app.css` — add statistics screen styles (including tablet breakpoint)
