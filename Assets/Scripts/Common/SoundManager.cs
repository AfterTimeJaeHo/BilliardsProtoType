using DG.Tweening;
using UnityEngine;

namespace Aftertime.StorylineEngine
{
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        private const string ResourcesBGMPrefix = "Audio/BGM/"; 
        private const string ResourcesSFXPrefix = "Audio/SFX/";
        private const string ResourcesAmbientPrefix = "Audio/Ambient/";
            
        [Header("[Audio]")] [SerializeField] private AudioSource _bgmAudio_0;
        [SerializeField] private AudioSource _bgmAudio_1;
        [SerializeField] private AudioSource _sfxAudio;
        [SerializeField] private AudioSource _AmbientAudio;
        [SerializeField] private AudioSource _voiceAudio;

        [Header("[Setting]")] public float bgmVolume;
        public float sfxVolume;
        public float ambientVolume;
        public float voiceVolume;

        private float _preBgmVolume;
        private float _preSfxVolume;
        private float _preAmbientVolume;
        private float _preVoiceVolume;

        private bool isFirstBGMPlaying;
        private AudioSource _curBGMAudioSource;

        public override void Initialize()
        {
            _preBgmVolume = bgmVolume;
            _preSfxVolume = sfxVolume;
            _preAmbientVolume = ambientVolume;
            _preVoiceVolume = voiceVolume;
        }

        private void Update()
        {
            // TODO: Replace with UniRx ReactiveProperty to drop Update polling
            if (_preBgmVolume != bgmVolume)
            {
                _preBgmVolume = bgmVolume;
                _bgmAudio_0.volume = bgmVolume;
                _bgmAudio_1.volume = bgmVolume;
            }

            if (_preSfxVolume != sfxVolume)
            {
                _preSfxVolume = sfxVolume;
                _sfxAudio.volume = sfxVolume;
            }

            if (_preAmbientVolume != ambientVolume)
            {
                _preAmbientVolume = ambientVolume;
                _AmbientAudio.volume = ambientVolume;
            }

            if (_preVoiceVolume != voiceVolume)
            {
                _preVoiceVolume = voiceVolume;
                _voiceAudio.volume = voiceVolume;
            }
        }

        public void PlayBGM(string bgmName, float duration = 0, float volume = 0)
        {
            string bgmPath = ResourcesBGMPrefix + bgmName;
            AudioClip clip = Resources.Load<AudioClip>(bgmPath);
            PlayBGM(clip, duration, volume);
        }
        
        public void PlayBGM(AudioClip audioClip, float duration = 0, float volume = 0)
        {
            volume = volume == 0 ? bgmVolume : volume;
            bool isFade = duration != 0;

            AudioSource nextAudioSource = (_curBGMAudioSource == _bgmAudio_0) ? _bgmAudio_1 : _bgmAudio_0;

            if (isFade)
                PlayFade(nextAudioSource, audioClip, duration, volume, true);
            else
                Play(nextAudioSource, audioClip, volume, true);

            _curBGMAudioSource = nextAudioSource;
        }
        
        public void PlaySFX(string sfxName, float duration = 0, float volume = 0)
        {
            string sfxPath = ResourcesSFXPrefix + sfxName;
            AudioClip clip = Resources.Load<AudioClip>(sfxPath);
            PlaySFX(clip, duration, volume);
        }

        public void PlaySFX(AudioClip audioClip, float duration = 0, float volume = 0)
        {
            volume = volume == 0 ? sfxVolume : volume;
            bool isFade = duration != 0;

            if (isFade)
                PlayFade(_sfxAudio, audioClip, duration, volume, false);
            else
                Play(_sfxAudio, audioClip, volume, false);
        }
        
        public void PlayAmbient(string ambientName, float duration = 0, float volume = 0)
        {
            string ambientPath = ResourcesAmbientPrefix + ambientName;
            AudioClip clip = Resources.Load<AudioClip>(ambientPath);
            PlayAmbient(clip, duration, volume);
        }

        public void PlayAmbient(AudioClip audioClip, float duration = 0, float volume = 0)
        {
            volume = volume == 0 ? ambientVolume : volume;
            bool isFade = duration != 0;

            if (isFade)
                PlayFade(_AmbientAudio, audioClip, duration, volume, true);
            else
                Play(_AmbientAudio, audioClip, volume, true);
        }
        
        // TODO: AudioSource 4개로 추가
        public void PlayVoice(string[] voicePath, float duration = 0, float volume = 0)
        {
            AudioClip clip = Resources.Load<AudioClip>(voicePath[0]);
            PlayVoice(clip, duration, volume);
        }

        public void PlayVoice(AudioClip clip, float duration = 0, float volume = 0)
        {
            volume = volume == 0 ? voiceVolume : volume;
            bool isFade = duration != 0;

            if (isFade)
                PlayFade(_voiceAudio, clip, duration, volume, false);
            else
                Play(_voiceAudio, clip, volume, false);
        }


        public void StopBGM(float duration = 0)
        {
            bool isFade = duration != 0;
            AudioSource audioSource = (_curBGMAudioSource == _bgmAudio_0) ? _bgmAudio_1 : _bgmAudio_0;

            if (isFade)
                StopFade(audioSource, duration);
            else
                Stop(audioSource);
        }

        public void StopAllBGM(float duration = 0)
        {
            bool isFade = duration != 0;

            if (isFade)
            {
                StopFade(_bgmAudio_0, duration);
                StopFade(_bgmAudio_1, duration);
            }
            else
            {
                Stop(_bgmAudio_0);
                Stop(_bgmAudio_1);
            }
        }

        public void StopSFX(float duration = 0)
        {
            bool isFade = duration != 0;

            if (isFade)
                StopFade(_sfxAudio, duration);
            else
                Stop(_sfxAudio);
        }

        public void StopAmbient(float duration = 0)
        {
            bool isFade = duration != 0;

            if (isFade)
                StopFade(_AmbientAudio, duration);
            else
                Stop(_AmbientAudio);
        }

        public void StopVoice(float duration = 0)
        {
            bool isFade = duration != 0;

            if (isFade)
                StopFade(_voiceAudio, duration);
            else
                Stop(_voiceAudio);
        }

        protected virtual void Play(AudioSource audioSource, string clipName, float volume, bool isLoop)
        {
            AudioClip clip = Resources.Load<AudioClip>(clipName);
            Play(audioSource, clip, volume, isLoop);
        }
        
        protected virtual void Play(AudioSource audioSource, AudioClip clip, float volume, bool isLoop)
        {
            if (audioSource == null)
                return;
            
            audioSource.DOKill();
            audioSource.volume = volume;
            audioSource.clip = clip;
            audioSource.loop = isLoop;
            audioSource.Play();
        }

        protected virtual void PlayFade(AudioSource audioSource, string clipName, float duration, float volume, bool isLoop)
        {
            AudioClip clip = Resources.Load<AudioClip>(clipName);
            PlayFade(audioSource, clip, duration, volume, isLoop);
        }
        
        protected virtual void PlayFade(AudioSource audioSource, AudioClip clip, float duration, float volume, bool isLoop)
        {
            if (audioSource == null)
                return;
            
            audioSource.DOKill();
            audioSource.clip = clip;
            audioSource.volume = 0;
            audioSource.loop = isLoop;
            audioSource.Play();
            audioSource.DOFade(volume, duration);
        }

        protected virtual void Stop(AudioSource audioSource)
        {
            if (audioSource == null)
                return;
            
            audioSource.Stop();
            audioSource.clip = null;
        }

        protected virtual void StopFade(AudioSource audioSource, float duration)
        {
            if (audioSource == null)
                return;
            
            audioSource.DOFade(0, duration).onComplete += () => audioSource.clip = null;
        }
    }
}