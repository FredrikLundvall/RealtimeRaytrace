using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Actor
    {
        protected AnimationArchive _animationArchive;
        protected Vector3 _position;
        Vector3 _direction; //Quaternion? Matrix?

        public Actor(AnimationArchive animationArchive)
        {
            _animationArchive = animationArchive;
        }

        public void UpdateGameTime(GameTime gameTime)
        {

        }

        public AnimationArchive GetAnimationArchive()
        {
            return _animationArchive;
        } 
    }
}
