using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using ZGame.Ress;
namespace ZGame.Ress
{
    public class VideoManager : Singleton<VideoManager>
    {
        Dictionary<VideoPlayer, RawImage> videoMap = new Dictionary<VideoPlayer, RawImage>();
        private Dictionary<VideoPlayer, UnityEngine.Video.VideoPlayer.EventHandler> handlers
        = new Dictionary<VideoPlayer, UnityEngine.Video.VideoPlayer.EventHandler>();


        //onFinished：播放失败会执行；播放视频完成后也会执行
        public async void PlayVideo(VideoPlayer player, RawImage targetImg, string name, string extension, float volume, Action onFinished)
        {
            volume = Storage.GetAudioStatus() == true ? volume : 0;
            volume = volume * Storage.GetAudioValue();//整体乘以全局的音效大小控制值

            if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
            {
                Debug.LogError("TODO");
            }
            else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
            {
                var clip = await AAManager.Instance.LoadVideoClip(name, extension);
                this.playVideo(player, targetImg, clip, volume, onFinished);
            }
            else
            {
                Debug.LogError("TODO");
            }
        }



        private void playVideo(VideoPlayer player, RawImage targetImg, VideoClip clip, float volume, Action onFinished)
        {
            videoMap[player] = targetImg;
            removePreviousHandler(player);

            targetImg.color = Color.white;//设置非透明
            player.clip = clip;
            player.SetDirectAudioVolume(0, volume);
            player.isLooping = false;

            UnityEngine.Video.VideoPlayer.EventHandler newHandler = null;
            newHandler = (p) =>
            {
                try
                {
                    if (this.videoMap.TryGetValue(p, out RawImage targetImg) && targetImg != null)
                    {
                        targetImg.color = new Color(1, 1, 1, 0);//播放完，透明
                    }
                    this.videoMap.Remove(p);

                    onFinished?.Invoke();
                }
                finally
                {
                    // 播放完成后**立即移除自己**，防止内存泄漏
                    if (p != null)
                    {
                        p.loopPointReached -= newHandler;
                        handlers.Remove(p);
                    }
                }
            };
            handlers[player] = newHandler;//绑定
            player.loopPointReached += newHandler;
            player.Play();
        }
        void removePreviousHandler(VideoPlayer player)
        {
            if (player == null) return;

            if (handlers.TryGetValue(player, out var handler))
            {
                player.loopPointReached -= handler;
                handlers.Remove(player);
            }
        }
    }
}