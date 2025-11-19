using UnityEngine.UI;

namespace Aftertime.SecretSome.UI
{
    public interface INavigationGroup
    {
        public void ActiveNavigation(Navigation activeNaviation);
        public void InActiveNavigation(Navigation inActiveNavigation);
    }
   
}
