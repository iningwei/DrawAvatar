using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using ZGame.Event;
using ZGame.Ress.AB;
using Object = UnityEngine.Object;

namespace ZGame.Ress
{
    public class AudioManager : Singleton<AudioManager>
    {
        GameObject gameObj;
        AudioSource bgmAudioSource;


        Dictionary<int, AudioSource> soundAudioSourceDic = new Dictionary<int, AudioSource>();

        Dictionary<string, AudioClip> soundClipDic = new Dictionary<string, AudioClip>();

        bool isSoundLimitPlaying(string name, int limitCount = 1)
        {
            int count = 0;
            foreach (var item in soundAudioSourceDic)
            {
                if (item.Value.isPlaying && item.Value.clip.name == name)
                {
                    count++;
                }
            }
            if (count >= limitCount)
            {
                return true;
            }
            return false;
        }
        int getFreeSoundAudioSourceID()
        {
            int target = 0;
            int count = soundAudioSourceDic.Count;

            foreach (var item in soundAudioSourceDic)
            {
                if (item.Value.isPlaying == false)
                {
                    target = item.Key;
                    break;
                }
            }


            if (target == 0)
            {
                target = count + 1;
                soundAudioSourceDic[target] = gameObj.AddComponent<AudioSource>();
                soundAudioSourceDic[target].playOnAwake = false;
                soundAudioSourceDic[target].volume = Storage.GetAudioValue();
            }

            return target;
        }

        public AudioManager()
        {
            gameObj = new GameObject();
            gameObj.name = "AudioRoot";
            Object.DontDestroyOnLoad(gameObj);

            gameObj.GetOrAddComponent<AudioListener>();
            bgmAudioSource = gameObj.GetOrAddComponent<AudioSource>();
            bgmAudioSource.playOnAwake = false;
            bgmAudioSource.volume = Storage.GetMusicValue();

            EventDispatcher.Instance.AddListener(EventID.OnSoundSliderValueChange, onSoundChanged);
            EventDispatcher.Instance.AddListener(EventID.OnBgmSliderValueChange, onBGMSoundChanged);
            EventDispatcher.Instance.AddListener(EventID.OnAudioSliderValueChange, onAudioSoundChanged);
        }

        private void onAudioSoundChanged(string evtId, object[] paras)
        {
            float soundValue = float.Parse(paras[0].ToString());

            foreach (var item in soundAudioSourceDic)
            {
                item.Value.volume = soundValue;
            }
        }

        private void onBGMSoundChanged(string evtId, object[] paras)
        {
            float soundValue = float.Parse(paras[0].ToString());
            bgmAudioSource.volume = soundValue;

        }

        private void onSoundChanged(string evtId, object[] paras)
        {
            float soundValue = float.Parse(paras[0].ToString());
            bgmAudioSource.volume = soundValue;

            foreach (var item in soundAudioSourceDic)
            {
                item.Value.volume = soundValue;
            }
        }

        public bool IsBGMEnabled
        {
            get
            {
                return Storage.GetMusicStatus();
            }
        }



        public void ClearBGM()
        {
            if (bgmAudioSource.clip != null)
            {
                if (bgmAudioSource.isPlaying)
                {
                    bgmAudioSource.Stop();
                }
                var res = ABManager.Instance.GetCachedRes<AudioRes>(bgmAudioSource.clip.name);
                if (res != null)
                {
                    res.Destroy();
                }
                bgmAudioSource.clip = null;
            }
        }

        public float GetCurBGMClipLength()
        {
            float length = 0f;
            if (bgmAudioSource.clip != null)
            {
                length = bgmAudioSource.clip.length;
            }

            return length;
        }
        public bool BGMStateIsPlaying()
        {
            return bgmAudioSource.isPlaying;
        }
        public void ResumeBGM()
        {
            if (IsBGMEnabled == false)
            {
                return;
            }
            if (bgmAudioSource.clip == null || (bgmAudioSource.isPlaying == false && bgmAudioSource.loop == false))
            {
                return;
            }
            Debug.LogError("resumeBGM");
            bgmAudioSource.Play();
        }

        public void PlayBGM(string name, string extension, float volume, bool isLoop)
        {
            volume = Storage.GetMusicStatus() == true ? volume : 0;
            volume = volume * Storage.GetMusicValue();//整体乘以全局的音效大小控制值
            Debug.LogError("play bgm:" + name + ", volume:" + volume);
            //clip里面正是目标音乐
            if (bgmAudioSource.clip != null && bgmAudioSource.clip.name == name)
            {
                if (bgmAudioSource.isPlaying == false)
                {
                    bgmAudioSource.volume = volume;
                    bgmAudioSource.Play();
                }

                if (bgmAudioSource.loop != isLoop)
                {
                    bgmAudioSource.loop = isLoop;
                }
                return;
            }

            //clip里不是目标背景音乐，先卸载之
            if (bgmAudioSource.clip != null && bgmAudioSource.clip.name != name)
            {
                if (bgmAudioSource.isPlaying)
                {
                    bgmAudioSource.Stop();
                }
                if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
                {
                    var res = ABManager.Instance.GetCachedRes<AudioRes>(bgmAudioSource.clip.name);
                    if (res != null)
                    {
                        res.Destroy();
                    }
                }
                else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
                {
                    //卸载
                    Resources.UnloadAsset(bgmAudioSource.clip);
                }
                else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
                {
                    AAManager.Instance.DestroyRes<AudioClip>(bgmAudioSource.clip.name);

                    //////Resources.UnloadAsset(bgmAudioSource.clip);
                }

                bgmAudioSource.clip = null;
            }


            if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
            {
                ABManager.Instance.LoadAudioClip(name, (clip) =>
                {
                    bgmAudioSource.clip = clip;
                    bgmAudioSource.volume = volume;
                    bgmAudioSource.Play();
                    bgmAudioSource.loop = isLoop;
                }, true);
            }
            else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
            {
                var clip = Resources.Load("Audio/" + name, typeof(AudioClip)) as AudioClip;
                bgmAudioSource.clip = clip;
                bgmAudioSource.volume = volume;
                bgmAudioSource.Play();
                bgmAudioSource.loop = isLoop;
            }
            else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
            {
                //addressable下webgl在真实模拟或者打包后报错
                //Error: Cannot create FMOD::Sound instance for clip "" (FMOD error: Unsupported file or audio format. )
                //暂时没找到可行解决方案
                AAManager.Instance.LoadAudioClip(name, extension, (clip) =>
                {
                    bgmAudioSource.clip = clip;
                    bgmAudioSource.volume = volume;
                    bgmAudioSource.Play();
                    bgmAudioSource.loop = isLoop;
                });


                //////var clip = Resources.Load("Audio/" + name, typeof(AudioClip)) as AudioClip;
                //////bgmAudioSource.clip = clip;
                //////bgmAudioSource.volume = volume;
                //////bgmAudioSource.Play();
                //////bgmAudioSource.loop = isLoop;
            }

        }



        public void StopBGM()
        {
            if (bgmAudioSource.clip != null && bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop();
            }
        }

        public async Task<AudioSource> PlaySound(string name, string extension, float volume, int limitCount, bool isLoop = false)
        {
            volume = Storage.GetAudioStatus() == true ? volume : 0;
            volume = volume * Storage.GetAudioValue();//整体乘以全局的音效大小控制值
            if (limitCount < 1)
            {
                Debug.LogError($"error, audio {name} limitCount<1");
            }
            //限制同时一种声音播放个数
            if (isSoundLimitPlaying(name, limitCount))
            {
                return null;
            }

            if (soundClipDic.ContainsKey(name))
            {
                return this.playSound(name, volume, isLoop);
            }
            else
            {
                if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
                {
                    var clip = Resources.Load("Audio/" + name, typeof(AudioClip)) as AudioClip;
                    soundClipDic.Add(name, clip);
                    this.playSound(name, volume, isLoop);
                }
                else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
                {

                    var clip = await AAManager.Instance.LoadAudioClip(name, extension);
                    if (!soundClipDic.ContainsKey(name))
                    {
                        soundClipDic.Add(name, clip);
                    }
                    return this.playSound(name, volume, isLoop);

                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        AudioSource playSound( string name, float volume, bool isLoop = false)
        {
            var audioSourceId = getFreeSoundAudioSourceID();
            var audioSource = soundAudioSourceDic[audioSourceId];
            audioSource.loop = isLoop;
            audioSource.clip = soundClipDic[name];
            audioSource.volume = volume;
            audioSource.Play();

            return audioSource;
        }
        AudioSource playSound(AudioSource audioSource, string name, float volume, bool isLoop = false)
        {
            audioSource.loop = isLoop;
            audioSource.clip = soundClipDic[name];
            audioSource.volume = volume;
            audioSource.Play();

            return audioSource;
        }


        public async Task<AudioSource> PlaySound(AudioSource targetAudioSource, string name, string extention, float volume, bool isLoop = false)
        {
            targetAudioSource.Stop();
            volume = Storage.GetAudioStatus() == true ? volume : 0;
            volume = volume * Storage.GetAudioValue();//整体乘以全局的音效大小控制值
            if (soundClipDic.ContainsKey(name))
            {
                return this.playSound(targetAudioSource, name, volume, isLoop);
            }
            else
            {
                if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
                {
                    var clip = await AAManager.Instance.LoadAudioClip(name, extention);
                    if (!soundClipDic.ContainsKey(name))
                    {
                        soundClipDic.Add(name, clip);
                    }
                    return this.playSound(targetAudioSource, name, volume, isLoop);
                }
            }
            return targetAudioSource;
        }
        public void StopSound(int id)
        {
            if (soundAudioSourceDic.ContainsKey(id))
            {
                soundAudioSourceDic[id].Stop();
            }
        }

    }
}