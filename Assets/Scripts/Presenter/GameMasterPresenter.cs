using Model;
using View;

namespace Presenter
{
    public class GameMasterPresenter
    {
        public GameMasterModel Model;
        public UserModel userModel;
        public GameMasterView View;

        public GameMasterPresenter(GameMasterModel model, GameMasterView view)
        {
            this.Model = model;
            this.View = view;
            userModel = new UserModel(GetMasterTable().MasterHeroes[0]);
        }

        public MasterTable GetMasterTable()
        {
            return Model.masterTable;
        }

        public UserModel GetUser()
        {
            return userModel;
        }
    }
}
