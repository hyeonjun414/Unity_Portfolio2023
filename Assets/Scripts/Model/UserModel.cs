using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public class UserModel : SceneModel
    {
        public HeroModel Hero;
        public List<CardModel> Cards = new();
        public List<ArtifactModel> Artifacts = new();
        public int MaxEnergy;
        public int CurEnergy;

        public int DrawCardCount;
        public int Gold;

        public void Init(MasterUser mu, MasterTable mt)
        {
            var hero = mt.MasterHeroes.First(target => target.Name == mu.Hero);
            Hero = new HeroModel(hero);

            MaxEnergy = mu.Energy;
            DrawCardCount = mu.DrawCardCount;
            Gold = mu.InitGold;
            foreach (var cardId in mu.Cards)
            {
                var mc = mt.MasterCards.First(target => target.Id == cardId);
                Cards.Add(new CardModel(mc));
            }

            foreach (var artifactId in mu.Artifacts)
            {
                var ma = mt.MasterArtifacts.First(target => target.Id == artifactId);
                Artifacts.Add(new ArtifactModel(ma));
            }
        }
    }
}
