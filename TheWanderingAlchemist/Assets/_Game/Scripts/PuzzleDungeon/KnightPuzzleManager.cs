using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class KnightPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    public string puzzleID = "KnightPuzzle_01"; 
    public List<KnightColor> correctSequence;

    private KnightPuzzleAudio puzzleAudio;
    private int currentIndex = 0; 
    private bool isSolved = false;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved; 
    public UnityEvent OnPuzzleFailed;
    public UnityEvent OnPuzzleAlreadySolved; 

    private void Awake()
    {
        puzzleAudio = GetComponent<KnightPuzzleAudio>();
    }

    private void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.solvedPuzzles.Contains(puzzleID))
        {
            ForceSolve();
        }
    }

    public void OnKnightActivated(PuzzleKnight knight)
    {
        if (isSolved) return;

        if (knight.myColor == correctSequence[currentIndex])
        {
            puzzleAudio?.PlayNote(currentIndex); 
            currentIndex++; 
            if (currentIndex >= correctSequence.Count) SolvePuzzle();
        }
        else
        {
            FailPuzzle();
        }
    }

    private void FailPuzzle()
    {
        currentIndex = 0;
        puzzleAudio?.PlayFail(); 
        OnPuzzleFailed?.Invoke();
    }

    private void SolvePuzzle()
    {
        isSolved = true;
        puzzleAudio?.PlaySuccess(); 
        
        if (SaveManager.Instance != null && !SaveManager.Instance.solvedPuzzles.Contains(puzzleID))
        {
            SaveManager.Instance.solvedPuzzles.Add(puzzleID);
        }
        OnPuzzleSolved?.Invoke();
    }

    private void ForceSolve()
    {
        isSolved = true;
        OnPuzzleAlreadySolved?.Invoke(); 
    }
}