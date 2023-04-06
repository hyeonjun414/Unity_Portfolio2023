using System.Collections.Generic;

namespace Model
{
    public class StageInfo
    {
        public string Type;
        public string Icon;
    }

    public class BattleStageInfo : StageInfo
    {
        public List<string> Enemies;
    } 
}
