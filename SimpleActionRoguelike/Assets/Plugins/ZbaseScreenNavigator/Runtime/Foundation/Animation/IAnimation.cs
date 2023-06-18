namespace ZBase.UnityScreenNavigator.Foundation.Animation
{
    public interface IAnimation
    {
        float Duration { get; }

        void SetTime(float time);
    }
}