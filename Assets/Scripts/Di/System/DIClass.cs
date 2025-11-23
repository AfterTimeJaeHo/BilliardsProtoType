namespace Waving.Di
{
    public abstract class DIClass
    {
        protected DIClass()
        {
            DIContainerBase.TryInjectAll(this);
        }
    }   
}