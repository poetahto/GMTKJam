using System.Collections;
using FMODUnity;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter music;
    
    public IEnumerator FadeOut()
    {
        var fadingMusic = music.EventInstance;
        var fadeTime = 0.4f;
        var startTime = Time.time;
        
        fadingMusic.getVolume(out var startVolume);
        
        while (Time.time - startTime < fadeTime)
        {
            var percentComplete = (Time.time - startTime) / fadeTime;
            var targetVolume = Mathf.Lerp(startVolume, 0, percentComplete);
            
            fadingMusic.setVolume(targetVolume);
            yield return new WaitForEndOfFrame();
        }

        fadingMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fadingMusic.release();
    }
}
