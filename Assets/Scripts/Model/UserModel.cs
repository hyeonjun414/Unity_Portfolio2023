using UnityEngine;

namespace Model
{
    public class UserModel
    {
        public EntityModel Hero;

        public UserModel(MasterEntity hero)
        {
            Hero = new EntityModel(hero);
        }
    }
}
