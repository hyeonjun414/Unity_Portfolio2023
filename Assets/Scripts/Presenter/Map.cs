using Model;
using View;

namespace Presenter
{
    public class Map
    {
        public MapModel Model;
        public MapView View;
        
        public Map(MapModel model, MapView view)
        {
            Model = model;
            View = view;
        }
    }
}
