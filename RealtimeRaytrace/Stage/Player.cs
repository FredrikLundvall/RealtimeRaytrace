using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Player : ICommandable, IMessageReceiver
    {
        readonly Camera _camera;
        bool _hasQuit = false;
        bool _hasFullscreen = true;
        string _id;

        public bool IdMatchesReceiver(string receiver)
        {
            return (_id == receiver);
        }

        public void ReceiveMessage(EventMessage eventMessage, GameTime gameTime)
        {
            eventMessage.Execute(this, (float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        public Player(Camera camera, string id, bool hasFullscreen = true, bool hasQuit = false)
        {
            _camera = camera;
            _hasQuit = hasQuit;
            _hasFullscreen = hasFullscreen;
            _id = id;
        }

        public void DoQuit()
        {
            _hasQuit = true;
        }

        public bool HasQuit()
        {
            return _hasQuit;
        }

        public void ToggleFullscreen()
        {
            _hasFullscreen = !_hasFullscreen;
        }

        public bool HasFullscreen()
        {
            return _hasFullscreen;
        }
        
        public void MoveDepth(float forwardAmount,float elapsedTotalSeconds)
        {
            _camera.MoveDepth(forwardAmount * elapsedTotalSeconds);
        }

        public void MoveHeight(float heightAmount, float elapsedTotalSeconds)
        {
            _camera.MoveHeight(heightAmount * elapsedTotalSeconds);
        }

        public void MoveSide(float sideAmount, float elapsedTotalSeconds)
        {
            _camera.MoveSide(sideAmount * elapsedTotalSeconds);
        }

        public void RotateYaw(float yawAmount, float elapsedTotalSeconds)
        {
            _camera.RotateYaw(yawAmount * elapsedTotalSeconds);
        }

        public void RotatePitch(float pitchAmount, float elapsedTotalSeconds)
        {
            _camera.RotatePitch(pitchAmount * elapsedTotalSeconds);
        }
    }
}
