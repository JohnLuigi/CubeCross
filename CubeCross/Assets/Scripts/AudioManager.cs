using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    // Reference to the audioSource attached to the object
    // this script is attached to (AudioManager).
    public AudioSource audioSource;
    
    // List of pop sfx audio clips that were dragged onto this script
    // via the inspector
    public List<AudioClip> popAudioClips;
    public List<float> popClipVolumes;

	// Use this for initialization
	void Start () {

        audioSource = gameObject.GetComponent<AudioSource>();

        // Add a default volume of 1 for each audio clip in popAudioClips
        while (popClipVolumes.Count < popAudioClips.Count)
            popClipVolumes.Add(1f);


	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Use this to play a specific popClip SFX from another script
    // Since its stored in a list, it will be indexed from 0.
    public void PlayPopClip(int clipIndex)
    {
        // Check if the inptu index is valid and if there are clips in the List.
        if(popAudioClips.Count > 0 && clipIndex >= 0 && clipIndex < popAudioClips.Count)
            audioSource.PlayOneShot(popAudioClips[clipIndex], popClipVolumes[clipIndex]);
    }

    // Use this to play a random popClip SFX from another script
    public void PlayRandomPopClip()
    {
        if(popAudioClips.Count > 0)
        {
            // Get a random index from the list containing popClips.
            int clipIndex = Random.Range(0, popAudioClips.Count - 1);

            // Play that index.
            audioSource.PlayOneShot(popAudioClips[clipIndex], popClipVolumes[clipIndex]);
        }
    }


}
