using Presenter;

namespace Model
{
    public class Condition
    {
        public string Type;
        public virtual bool Check(object target)
        {
            return false;
        }
    }

    public class Cdt_EnemyActionReady : Condition
    {
        public override bool Check(object target)
        {
            if (target is Enemy enemy)
            {
                return enemy.IsExecutable();
            }
            else
            {
                return base.Check(target);
            }
        }
    }
}
