namespace Waving.Di
{
    public class DIClass
    {
        public DIClass()
        {
            DIContainerBase.TryInjectAll(this);
        }
    }   
}