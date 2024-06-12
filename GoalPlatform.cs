using UnityEngine;

public class GoalPlatform : MonoBehaviour
{
    public MusicController musicController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            musicController.PlayGoalMusic();
        }
    }
}
