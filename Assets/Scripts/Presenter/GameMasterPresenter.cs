using Model;
using View;

namespace Presenter
{
    public class GameMasterPresenter
    {
        private GameMasterModel model;
        private GameMasterView view;

        public GameMasterPresenter(GameMasterModel model, GameMasterView view)
        {
            this.model = model;
            this.view = view;
        }

        public MasterTable GetMasterTable()
        {
            return model.masterTable;
        }

        public User GetUser()
        {
            return model.user;
        }
    }
}
