using Aftertime.StorylineEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Waving.Directing
{
    public class TimelineDirector : SingletonMonoBehaviour<TimelineDirector>
    {
        [SerializeField] private PlayableDirector _playableDirector;
        [SerializeField] private TimelineAsset _spankingTimeline;
        [SerializeField] private TimelineAsset _assaultFailTimeline;

        public async UniTask PlaySpanking()
        {
            _playableDirector.Play(_spankingTimeline);
            double duration = _playableDirector.duration;
            await UniTask.Delay((int)(duration * 1000));
        }

        public async UniTask PlayAssaultFail()
        {
            _playableDirector.Play(_assaultFailTimeline);
            double duration = _playableDirector.duration;
            await UniTask.Delay((int)(duration * 1000));
        }
    }
   
}