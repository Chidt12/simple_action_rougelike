using Pathfinding;
using Pathfinding.RVO;
namespace Runtime.Gameplay.EntitySystem
{
    public class CustomRVOController : RVOController
    {
        // IAStarAI now dont have to be Monobehavior
        protected override IAstarAI ai
        {
            get { return aiBackingField; }
            set { aiBackingField = value; }
        }
    }

}