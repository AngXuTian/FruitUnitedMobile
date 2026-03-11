using System.Web.UI;

public class BasePage : Page
{
    protected override PageStatePersister PageStatePersister
    {
        get { return new SessionPageStatePersister(this); }
    }
}