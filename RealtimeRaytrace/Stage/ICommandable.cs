
namespace RealtimeRaytrace
{
    public interface ICommandable
    {
        void MoveDepth(float forwardAmount, float elapsedTotalSeconds);
        void MoveHeight(float heightAmount, float elapsedTotalSeconds);
        void MoveSide(float sideAmount, float elapsedTotalSeconds);
        void RotateYaw(float yawAmount, float elapsedTotalSeconds);
        void RotatePitch(float pitchAmount, float elapsedTotalSeconds);
        void DoQuit();
    }
}
