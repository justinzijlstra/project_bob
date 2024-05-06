
using System;
using UnityEngine;

[System.Serializable]
public class PlayAudio : MonoBehaviour
{
    private MultipleSounds soundManager;

    void Start()
    {
        // Get the AudioSource component attached to the GameObject
        if(transform.parent != null)
        {
            soundManager = transform.parent.GetComponent<MultipleSounds>();
        }
        else
        {
            soundManager = transform.parent.GetComponent<MultipleSounds>();
        }
    }

    // The method to be called from the Animation Event
    public void PlayPopSound()
    {
        if (!soundManager)
            return;
        // Play the audio clip
        soundManager.PlayRandomSound(soundManager.audioClips_Pop);
    }

    public void FoodParticles()
    {
        if (transform.parent == null)
            return;
        ParticleSystem particleSystem = transform.parent.GetComponent<ParticleSystem>();
        if (particleSystem == null)
            return;

        // Emit the specified number of particles
        particleSystem.Emit(10);

        if (!soundManager)
            return;
        // Play the audio clip
        soundManager.PlayRandomSound(soundManager.audioClips_Eten, 1.5f);
    }

    public void PlayAnnoyedSound()
    {
        if (!soundManager)
            return;
        // Play the audio clip
        soundManager.PlayRandomSound(soundManager.audioClips_Annoyed, 1.6f);
    }
    public void PlayAngrySound()
    {
        if (!soundManager)
            return;
        // Play the audio clip
        soundManager.PlayRandomSound(soundManager.audioClips_Angry, 2.0f);
    }

    public void PlayHappySound()
    {
        if (!soundManager)
            return;
        // Play the audio clip
        soundManager.PlayRandomSound(soundManager.audioClips_Happy, 1.3f);
    }

    public void PlayOccasionalHappySound()
    {
        if (!soundManager)
            return;
        // Play the audio clip
        float randomValue = UnityEngine.Random.Range(0.0f, 1.0f);
        // Check if the random value is less than the chance
        if (randomValue < 0.25f)
            soundManager.PlayRandomSound(soundManager.audioClips_Happy, 1.3f);
    }

    public void PlayWaspSound()
    {
        if (!soundManager)
            return;
        // Play the audio clip
        soundManager.PlayRandomSound(soundManager.audioClips_Wasp, 1.0f, 1);
    }
}
