using System.Collections.Generic;
using System.Linq;
using Aftertime.StorylineEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Aftertime.SecretSome.UI.Layout
{
    public class LayoutManager : SingletonMonoBehaviour<LayoutManager>
    {
        [SerializeField] private List<LayoutBase> layouts;

        public override void Initialize()
        {
        }

        public void Show<T>(bool isShowImmediately = false) where T : LayoutBase
        {
            LayoutBase layout = layouts.FirstOrDefault(layout => layout.GetType() == typeof(T));
            if (layout == null)
                return;

            layout.Show(isShowImmediately);
        }

        public void Show<T>(float duration) where T : LayoutBase
        {
            LayoutBase layout = layouts.FirstOrDefault(layout => layout.GetType() == typeof(T));
            if (layout == null)
                return;

            layout.Show(duration);
        }

        public void Hide<T>(bool isHideImmediately = false) where T : LayoutBase
        {
            LayoutBase layout = layouts.FirstOrDefault(layout => layout.GetType() == typeof(T));
            if (layout != null)
                layout.Hide(isHideImmediately);
        }

        public void Hide<T>(float duration) where T : LayoutBase
        {
            LayoutBase layout = layouts.FirstOrDefault(layout => layout.GetType() == typeof(T));
            if (layout != null)
                layout.Hide(duration);
        }

        public void Hide(LayoutBase hideLayout, float duration)
        {
            LayoutBase layout = layouts.FirstOrDefault(layout => layout == hideLayout);
            if (layout != null)
                layout.Hide(duration);
        }

        public async UniTask HideAsync<T>(bool isHideImmediately = false) where T : LayoutBase
        {
            LayoutBase layout = layouts.FirstOrDefault(layout => layout.GetType() == typeof(T));
            if (layout != null)
                await layout.HideAsync(isHideImmediately);
        }


        public T GetLayout<T>() where T : LayoutBase
        {
            LayoutBase layout = layouts.FirstOrDefault(layout => layout.GetType() == typeof(T));
            if (layout != null)
                return (T)layout;
            else
                return null;
        }
    }
}