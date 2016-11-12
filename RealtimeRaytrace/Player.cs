using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{
    public class Player
    {
        readonly Camera _camera;
        bool _hasQuit = false;
        bool _hasFullscreen = true;

        public Player(Camera camera, bool hasQuit = false, bool hasFullscreen = true)
        {
            _camera = camera;
            _hasQuit = hasQuit;
            _hasFullscreen = hasFullscreen;
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
    }
}
