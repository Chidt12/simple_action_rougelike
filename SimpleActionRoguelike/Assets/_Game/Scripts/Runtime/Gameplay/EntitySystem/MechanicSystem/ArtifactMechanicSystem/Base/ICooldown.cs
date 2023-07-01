using System;
namespace Runtime.Gameplay.EntitySystem
{
    public interface ICooldown
    {
        public float CurrentCountTime { get; }
        public float CountTime { get; }
        public Action<bool> OnCountTimeChanged { get; set; }
    }
}