using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using System.Threading;

namespace RealtimeRaytrace
{
    public class TriangleRaytraceRenderer : IRenderer, IDisposable
    {
        const int RENDER_DISTANCE = 280;
        const float LIGHTSOURCE_INTENSITY = 5000000;
        const float AMBIENT_INTENSITY = 0.15f;
        const int SHAKE_DIST = 1;

        GraphicsDeviceManager _graphicsDeviceManager;
        float _cycleRadians = (float)Math.PI;
        VertexPositionColor[] _vertices;
        ////test of moving polygon net
        //Vector2[] _orgVertices;
        int[] _indices;
        int _taskNumbers = Environment.ProcessorCount;
        Vector2 _minPos, _maxPos;

        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        Random _rnd = new Random();
        BasicEffect _basicEffect;
        Camera _camera;

        IWorld _world;
        ISkyMap _skyMap;

        public TriangleRaytraceRenderer(GraphicsDeviceManager graphicsDeviceManager, IWorld world, Camera camera, int width, int height, ISkyMap skyMap)
        {
            _world = world;
            _camera = camera;
            _skyMap = skyMap;
            _basicEffect = new BasicEffect(graphicsDeviceManager.GraphicsDevice);
            _graphicsDeviceManager = graphicsDeviceManager;
            _graphicsDeviceManager.PreferredBackBufferHeight = height;
            _graphicsDeviceManager.PreferredBackBufferWidth = width;
#if DEBUG
            _graphicsDeviceManager.IsFullScreen = false;
#else
            _graphicsDeviceManager.IsFullScreen = true;
#endif
            _graphicsDeviceManager.ApplyChanges();
            _graphicsDeviceManager.GraphicsDevice.Textures[0] = null;

            Vector2 center;
            center.X = _graphicsDeviceManager.GraphicsDevice.Viewport.Width * 0.5f;
            center.Y = _graphicsDeviceManager.GraphicsDevice.Viewport.Height * 0.5f;
            _minPos = -center;
            _maxPos = center;

            //This will create the triangles used for drawing the screen
            TriangleProjectionGrid projGrid = new TriangleProjectionGrid(_minPos.X, _minPos.Y, _maxPos.X, _maxPos.Y);

            projGrid.CreateGrid();

            _vertices = projGrid.GetTriangleIndex().GetVerticesPositionColor();
            ////test of moving polygon net
            //_orgVertices = new Vector2[_vertices.Length];
            //for (int i = 0; i < _vertices.Length; i++)
            //    _orgVertices[i] = new Vector2(_vertices[i].Position.X, _vertices[i].Position.Y);

            _vertexBuffer = new VertexBuffer(_graphicsDeviceManager.GraphicsDevice, typeof(VertexPositionColor), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(_vertices);

            _indices = projGrid.GetTriangleIndex().GetIndices();
            _indexBuffer = new IndexBuffer(_graphicsDeviceManager.GraphicsDevice, typeof(int), _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);

            _basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, -1), new Vector3(0, 0, 0), Vector3.Up);
            _basicEffect.Projection = Matrix.CreateOrthographic(center.X * 2, center.Y * 2, 1, 2);
            _basicEffect.VertexColorEnabled = true;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            _graphicsDeviceManager.GraphicsDevice.RasterizerState = rasterizerState;

            _graphicsDeviceManager.GraphicsDevice.Indices = _indexBuffer;//Same every time, only call once! (but it's not possible when 2Dtextures are drawn too)

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_vertexBuffer != null)
                {
                    _vertexBuffer.Dispose();
                    _vertexBuffer = null;
                }
                if (_basicEffect != null)
                {
                    _basicEffect.Dispose();
                    _basicEffect = null;
                }
                if (_indexBuffer != null)
                {
                    _indexBuffer.Dispose();
                    _indexBuffer = null;
                }
            }
        }

        public Camera MainCamera
        {
            get { return _camera; }
        }

        private bool isPositionOutsideBoundaries(float x, float y)
        {
            return x < _minPos.X || x > _maxPos.X || y < _minPos.Y || y > _maxPos.Y;
        }

        public void Render(GameTime gameTime)
        {
            _cycleRadians += (float)gameTime.ElapsedGameTime.TotalSeconds / 15;

            double maxDistance = Math.Sqrt(_maxPos.X * _maxPos.X + _maxPos.Y * _maxPos.Y);

            //Vector3 newPosition = _grid._testSphereMove.GetPosition();
            //newPosition.X += (float)gameTime.ElapsedGameTime.TotalSeconds * 5;           
            //_grid.ReIndexByVoxelPosition(_grid._testSphereMove, newPosition);

            //Use threading or not
            //for (int t = 0; t < _vertices.Length; t++)
            Parallel.For(0, _vertices.Length, (t) =>
            {
                ////test of moving polygon net
                //_vertices[t].Position.X = _orgVertices[t].X + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST);
                //_vertices[t].Position.Y = _orgVertices[t].Y + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST);
                //_vertices[t].Color = RenderPosition(_vertices[t].Position.X, _vertices[t].Position.Y);
                //Rays with moving points
                //_vertices[t].Color = RenderPosition(_vertices[t].Position.X + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST), _vertices[t].Position.Y + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST), maxDistance);

                _vertices[t].Color = RenderPosition(_vertices[t].Position.X, _vertices[t].Position.Y, maxDistance);
            }
            );


            //Added because of possible use of spritebatch
            _graphicsDeviceManager.GraphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDeviceManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            _vertexBuffer.SetData<VertexPositionColor>(_vertices);
            _indexBuffer.SetData(_indices);////Same every time, only call once! (but it's not possible when 2Dtextures are drawn too)
            _graphicsDeviceManager.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDeviceManager.GraphicsDevice.Indices = _indexBuffer;//Same every time, only call once! (but it's not possible when 2Dtextures are drawn too)
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDeviceManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indices.Length / 3);
            }

            //Test of rotation
            _world.GetEntity(0).RotateYaw(-0.0008f);
        }

        private Color RenderPosition(float x, float y, double maxDistance)
        {
            Vector3 lightsourcePos = new Vector3(2000 * (float)Math.Cos(_cycleRadians) , 0, 2000 * (float)Math.Sin(_cycleRadians));
            Ray ray = _camera.SpawnRay(x, y, maxDistance);
            Color pixel = Color.Black;
            //Use voxel traverse or complete brute force raytrace
            //Intersection closestIntersection = getClosestIntersectionUsingVoxelTraverse(ray, RENDER_DISTANCE);
            Intersection closestIntersection = _world.GetClosestIntersection(ray, RENDER_DISTANCE);

            if (closestIntersection.IsHit())
            {
                Vector3 direction = lightsourcePos - closestIntersection.GetPositionFirstHit();
                float distance = direction.Length();
                float factor = Vector3.Dot(closestIntersection.GetNormalFirstHit(), Vector3.Normalize(direction));
                //Use distance to lower intensity of the light
                factor *= (float)(LIGHTSOURCE_INTENSITY / Math.Pow(distance, 2));
                factor += AMBIENT_INTENSITY;
                //limit the factor to between 0 and 1
                factor = (factor < 0) ? 0 : ((factor > 1) ? 1 : factor);

                //Use textures or color
                ////Check if the sphere is rotated
                //if (closestIntersection.GetSphere().IsRotated())
                //{
                pixel = Color.Multiply(closestIntersection.GetSphere().GetTextureMap().GetColorFromDirection(Vector3.Transform(closestIntersection.GetNormalFirstHitTexture(), closestIntersection.GetSphere().GetTextureRotation())), factor);
                //}
                //else
                //{
                //    pixel = Color.Multiply(closestIntersection.GetSphere().GetTextureMap().GetColorFromDirection(closestIntersection.GetNormalFirstHitTexture()), factor);
                //}
                //pixel = Color.Multiply(Color.Lerp(closestIntersection.GetSphere().GetColor(), closestIntersection.GetSphere().GetTextureMap().GetColorFromDirection(closestIntersection.GetNormalFirstHitTexture()), 0.95f), factor);
                //pixel = Color.Multiply(closestIntersection.GetSphere().GetColor(), factor);
            }
            else
                pixel = _skyMap.GetColorInSky(ray.GetDirection());
            return pixel;
        }
    }
}
