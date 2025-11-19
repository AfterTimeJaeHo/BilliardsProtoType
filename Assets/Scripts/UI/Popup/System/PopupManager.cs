using System;
using System.Collections.Generic;
using System.Linq;
using Aftertime.SecretSome.UI.Layout;
using Aftertime.SecretSome.UI.Page;
using Aftertime.StorylineEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Waving.MyTinyStreamer.UI;

namespace Aftertime.SecretSome.UI.Popup
{
    public enum PopupType
    {
        NameCreate
    }

    public class PopupManager : SingletonMonoBehaviour<PopupManager>
    {
        public PageStackMap PageStackMap { get; private set; }

        [SerializeField] private List<PopupBase> _popups;

        [Header("[Debug]")] [SerializeField] private List<PopupBase> _debugPopups;

        private List<PopupBase> _activePopups;
        private PopupBase _prePopup;

        // Awake entry point
        public override void Initialize()
        {
            _activePopups = new List<PopupBase>();
            PageStackMap = new PageStackMap();
        }

#if UNITY_EDITOR
        private void Update()
        {
            _debugPopups = _activePopups;
        }
#endif

        public PopupBase Push<T>(Selectable firstSelected = null) where T : PopupBase
        {
            PopupBase popup = _popups.FirstOrDefault(ui => ui.GetType() == typeof(T));

            if (popup == null || _activePopups.Contains(popup))
                return null;

            _activePopups.Add(popup);

            if (firstSelected != null)
                EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);

            popup.Show();

            return popup;
        }

        public NoticePopup PushNoticePopup(string message,Action onClickAction = null, Selectable firstSelected = null)
        {
            Push<NoticePopup>();
            
            NoticePopup noticePopup = GetPopup<NoticePopup>();
            noticePopup.ShowMessage(message,onClickAction);
            
            return noticePopup;
        }

        public PopupBase Pop()
        {
            PopupBase curPopup = _activePopups.LastOrDefault();
            if (curPopup == null)
                return null;

            if (PageStackMap.Has(curPopup))
            {
                PopPage();
                return curPopup;
            }
            else
            {
                PopupBase prevPopup = PopPopup();

                return prevPopup;
            }
        }

        public PopupBase PopPopup()
        {
            PopupBase curPopup = _activePopups.LastOrDefault();
            List<PopupBase> activePopup = null;
            if (curPopup.OwnerLayout == null)
            {
                activePopup = _activePopups;
            }
            else
            {
                activePopup = _activePopups.Where(popup => popup.OwnerLayout == curPopup.OwnerLayout).ToList();
            }

            _prePopup = curPopup.gameObject.activeSelf ? curPopup : _prePopup;
            _activePopups.Remove(curPopup);
            activePopup.Remove(curPopup);
            curPopup.Hide();
            PageStackMap.Clear(curPopup);

            if (activePopup.Any())
            {
                PopupBase prePopup = activePopup.Last();

                return prePopup;
            }
            else
            {
                if (_activePopups.Any())
                {
                    PopupBase prePopup = _activePopups.LastOrDefault();
                    return prePopup;
                }
                else
                {
                    return null;
                }
            }
        }

        public PageBase PopPage()
        {
            PopupBase curPopup = _activePopups.LastOrDefault();
            int pageCount = PageStackMap.Count(curPopup);
            if (pageCount <= 0)
            {
                return null;
            }
            else if (pageCount > 1)
            {
                PageBase curPage = PageStackMap.Peek(curPopup);
                curPage.Hide(false);
                PageBase prevPage = PageStackMap.Pop(curPopup);
                prevPage.Show(true);

                return prevPage;
            }
            else
            {
                PageBase curPage = PageStackMap.Peek(curPopup);
                PopPopup();

                return curPage;
            }
        }

        public PopupBase Hide<T>(bool isHideImmediately = false) where T : PopupBase
        {
            PopupBase popup = _popups.FirstOrDefault(ui => ui.GetType() == typeof(T));
            if (popup == null || popup.gameObject == null)
                return null;

            _prePopup = popup;
            popup.Hide(isHideImmediately);
            _activePopups.Remove(popup);

            return popup;
        }

        public void HideAllInLayout(LayoutBase ownerLayout)
        {
            List<PopupBase> popups = _activePopups.Where(popup => popup.OwnerLayout == ownerLayout).ToList();
            foreach (PopupBase popup in popups)
            {
                if (popup == null || popup.gameObject == null)
                    continue;

                popup.Hide();
                _activePopups.Remove(popup);
            }

            _prePopup = null;
        }

        public void HideAll(bool isHideImmediately = false)
        {
            foreach (PopupBase popup in _activePopups)
            {
                if (popup == null)
                    continue;

                popup.Hide(isHideImmediately);
            }

            _prePopup = null;
            _activePopups.Clear();
        }

        public T GetPopup<T>() where T : PopupBase
        {
            PopupBase popup = _popups.FirstOrDefault(ui => ui.GetType() == typeof(T));
            if (popup != null && popup.gameObject != null)
            {
                return (T)popup;
            }
            else
            {
                return null;
            }
        }

        public PopupBase GetCurrentPopup()
        {
            if (_activePopups.Count == 0)
                return null;

            return _activePopups.Last();
        }
    }
}
