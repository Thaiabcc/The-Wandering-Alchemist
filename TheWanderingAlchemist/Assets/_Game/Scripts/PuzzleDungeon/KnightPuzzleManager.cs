using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class KnightPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [Tooltip("Correct Sequence")]
    public List<KnightColor> correctSequence;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip activateClip; // Ting
    public AudioClip failClip;     // Buzz
    public AudioClip successClip;  // Win

    [Tooltip("Pitch")]
    public float pitchStep = 0.15f;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved; 
    public UnityEvent OnPuzzleFailed; 

    private int currentIndex = 0; 
    private bool isSolved = false;

    public void OnKnightActivated(PuzzleKnight knight)
    {
        if (isSolved) return;
        if (knight.myColor == correctSequence[currentIndex])
        {
            PlayNote(currentIndex);
            currentIndex++; 
            if (currentIndex >= correctSequence.Count)
            {
                SolvePuzzle();
            }
        }
        else
        {
            FailPuzzle();
        }
    }

    private void PlayNote(int index)
    {
        if (audioSource && activateClip)
        {
            audioSource.pitch = 1.0f + (index * pitchStep);
            audioSource.PlayOneShot(activateClip);
        }
    }

    private void FailPuzzle()
    {
        currentIndex = 0;
        if (audioSource && failClip)
        {
            audioSource.pitch = 0.8f;
            audioSource.PlayOneShot(failClip);
        }

        OnPuzzleFailed?.Invoke();
    }

    private void SolvePuzzle()
    {
        isSolved = true;
        if (audioSource && successClip)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(successClip);
        }
        OnPuzzleSolved?.Invoke();
    }
}