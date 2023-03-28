using System.Collections.Generic;

namespace Model
{
    public class StageInfo
    {
        public string Type;
    }

    public class BattleStageInfo : StageInfo
    {
        public List<string> Enemies;
    } 
}
