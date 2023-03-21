using Model;
using View;

namespace Presenter
{
    public class CardPresenter
    {
        public CardModel Model;
        public CardView View;

        public CardPresenter(CardModel model, CardView view)
        {
            Model = model;
            View = view;
        }
    }
}
