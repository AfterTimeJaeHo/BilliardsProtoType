using System.Collections.Generic;
using Aftertime.SecretSome.UI.Page;

namespace Aftertime.SecretSome.UI.Popup
{
    public class PageStackMap
    {
        private readonly Dictionary<PopupBase, Stack<PageBase>> _map = new();

        public void Push(PopupBase popup, PageBase page)
        {
            if (popup == null)
                return;
            
            if (_map.TryGetValue(popup, out var stack))
            {
                stack.Push(page);
            }
            else
            {
                Stack<PageBase> newStack = new();
                newStack.Push(page);
                _map.Add(popup,newStack);
            }
        }

        public PageBase Pop(PopupBase popup)
        {
            if (popup == null)
                return null;
            
            if (_map.TryGetValue(popup, out var stack) && stack.Count > 0)
            {
                return stack.Pop();
            }
            return null;
        }

        public PageBase Peek(PopupBase popup)
        {
            if (popup == null)
                return null;
            
            if (_map.TryGetValue(popup, out var stack) && stack.Count > 0)
            {
                return stack.Peek();
            }
            return null;
        }

        public bool Has(PopupBase popup)
        {
            if (popup == null)
                return false;
            
            return _map.ContainsKey(popup);
        }

        public int Count(PopupBase popup)
        {
            if (popup == null)
                return -1;
            
            if (_map.TryGetValue(popup, out var stack))
            {
                return stack.Count;
            }
            return 0;
        }

        public void Clear(PopupBase popup)
        {
            if (popup == null)
                return;
            
            if (_map.TryGetValue(popup, out var stack))
            {
                stack.Clear();
            }
        }

        public void ClearAll()
        {
            _map.Clear();
        }
    }   
}
