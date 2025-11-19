using Aftertime.SecretSome.UI.Page;

namespace Aftertime.SecretSome.UI
{
    public interface IPagable
    {
        public T GetPage<T>() where T : PageBase;
        public void ShowPage<T>(bool hasDuration) where T : PageBase;
        public void HidePage<T>(bool hasDuration) where T : PageBase;
        
    }
   
}
