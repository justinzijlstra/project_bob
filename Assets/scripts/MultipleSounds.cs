
using UnityEngine;

public class MultipleSounds : MonoBehaviour
{
    public AudioClip[] audioClips_Pop;
    public AudioClip[] audioClips_Eten;
    public AudioClip[] audioClips_Annoyed;
    public AudioClip[] audioClips_Angry;
    public AudioClip[] audioClips_Happy;
    public AudioClip[] audioClips_Wasp;
    private AudioSource audioSource0;
    public AudioSource audioSourceWasp;

    void Start()
    {
        audioSource0 = GetComponent<AudioSource>();
        if (audioSource0 == null)
        {
            Debug.LogError("No AudioSource found on this GameObject.");
        }
    }

    public void PlayRandomSound(AudioClip[] audioClips, float pitch = 1.0f, int audioSourceID = 0)
    {
        AudioSource audioSource = audioSource0;
        if (audioSourceID == 1 && audioSourceWasp != null)
            audioSource = audioSourceWasp;

        if (audioClips.Length > 0 && audioSource != null)
        {
            // Choose a random AudioClip from the array
            int randomIndex = Random.Range(0, audioClips.Length);
            AudioClip clipToPlay = audioClips[randomIndex];

            // Assign the chosen AudioClip to the AudioSource and play it
            audioSource.clip = clipToPlay;
            audioSource.Play();
            audioSource.pitch = pitch;
        }
    }
}
