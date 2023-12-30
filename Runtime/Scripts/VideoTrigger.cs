using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public TextMeshPro currentTimeDisplay;
    public void Start() {
        if(videoPlayer == null) {
            videoPlayer = GetComponent<VideoPlayer>();
            try{
                audioSource = videoPlayer.GetTargetAudioSource(0);
            }catch{}
        }
    }

    bool IsMuted() {
        var mute = false;
        if(videoPlayer != null) {
            if(audioSource != null) {
                mute = audioSource.mute;
            }else{
                mute = videoPlayer.GetDirectAudioMute(0);
            }
        }
        return mute;
    }

    void Mute(bool mute) {
        if(videoPlayer != null) {
            if(audioSource != null) {
                audioSource.mute = mute;
            }else{
                videoPlayer.SetDirectAudioMute(0, mute);
            }
        }
    }

    void Play(Action callback = null) {
        if(videoPlayer != null && !videoPlayer.isPlaying) {
            if(videoPlayer.isPrepared) {
                videoPlayer.Play();
                callback?.Invoke();
            }else{
                UnityEngine.Video.VideoPlayer.EventHandler prepared = null;
                prepared = (source) => {
                    videoPlayer.Play();
                    callback?.Invoke();
                    videoPlayer.prepareCompleted -= prepared;
                };
                videoPlayer.prepareCompleted += prepared;
                videoPlayer.Prepare();
            }
        }
    }
    public void PlayPause() {
        if(videoPlayer != null) {
            if(videoPlayer.isPlaying) {
                videoPlayer.Pause();
            } else {
                Play();
            }
        }
    }
    public void PlayUrl(string url) {
        if(videoPlayer != null) {
            videoPlayer.url = url;
            Play(() => {
                videoPlayer.time = 0;
            });
        }
    }
    public void Mute() {
       Mute(true);
    }

    public void UnMute() {
       Mute(false);
    }

    public void MuteToggle() {
        Mute(!IsMuted());
    }

    void SetVolume(float amount) {
         if(videoPlayer != null) {
            if(audioSource != null) {
                audioSource.volume += amount;
            }else{
                videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) + amount);
            }
        }
    }
    public void VolumeUp() {
        if(IsMuted()) {
            UnMute();
        }
        SetVolume(0.1f);
    }

    public void VolumeDown() {
        SetVolume(-0.1f);
    }
    public void SeekForward10Sec() {
        if(videoPlayer != null && videoPlayer.time < videoPlayer.length) {
            videoPlayer.time += 10;
        }
    }

    public void SeekBackward10Sec() {
        if(videoPlayer != null && videoPlayer.time > 0) {
            videoPlayer.time -= 10;
        }
    }
    public void SeekForward1Min() {
        if(videoPlayer != null && videoPlayer.time < videoPlayer.length) {
            videoPlayer.time += 60;
        }
    }
    public void SeekBack1Min() {
        if(videoPlayer != null && videoPlayer.time > 0) {
            videoPlayer.time -= 60;
        }
    }
    public void SeekForward10Min() {
        if(videoPlayer != null && videoPlayer.time < videoPlayer.length) {
            videoPlayer.time += 600;
        }
    }
    public void SeekBack10Min() {
        if(videoPlayer != null && videoPlayer.time > 0) {
            videoPlayer.time -= 600;
        }
    }

    void Update() {
        if(videoPlayer != null && currentTimeDisplay != null && videoPlayer.isPlaying) {
            currentTimeDisplay.text = TimeSpan.FromSeconds(videoPlayer.time).ToString(@"hh\:mm\:ss") + " / " + TimeSpan.FromSeconds(videoPlayer.length).ToString(@"hh\:mm\:ss");
        }
    }
}
