using System;
using Aftertime.SecretSome.UI;

public interface IChoicePopup
{
    public void ShowSelect(string selectTitle);
    public void RegisterSelectEvent(string selectTitle, Action onClick);
    public ExtendedButton GetSelectButton(string selectTitle);
}
