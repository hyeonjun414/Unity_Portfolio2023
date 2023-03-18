using Model;
using View;

namespace Presenter
{
    public class GameMasterPresenter
    {
        public GameMasterModel Model;
        public GameMasterView View;

        public GameMasterPresenter(GameMasterModel model, GameMasterView view)
        {
            this.Model = model;
            this.View = view;
        }

        public MasterTable GetMasterTable()
        {
            return Model.masterTable;
        }

        public User GetUser()
        {
            return Model.user;
        }
    }
}
