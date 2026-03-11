using System.Web.UI;

namespace FruitUnitedMobile
{
    public class BasePage : Page
    {
        protected override PageStatePersister PageStatePersister
        {
            get { return new SessionPageStatePersister(this); }
        }
    }
}