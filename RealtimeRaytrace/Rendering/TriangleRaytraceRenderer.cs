using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using System.Threading;

namespace RealtimeRaytrace
{
    public class TriangleRaytraceRenderer : IRenderer
    {
        const int RENDER_DISTANCE = 200;
        const float LIGHTSOURCE_INTENSITY = 1000;
        const float AMBIENT_INTENSITY = 0.25f;
        const int SHAKE_DIST = 1;

        GraphicsDeviceManager _graphicsDeviceManager;
        float _cycleRadians = 0;
        VertexPositionColor[] _vertices;
        //Vector2[] _orgVertices;
        int[] _indices;
        int _taskNumbers = Environment.ProcessorCount;
        Vector2 _minPos, _maxPos;

        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        Random _rnd = new Random();
        BasicEffect _basicEffect;

        Camera _camera = new Camera(0, new Vector3(0, 0, 3),0f,0f,0f);
        WorldGrid _grid;
        ISkyMap _skyMap;
        
        public Camera MainCamera
        {
            get { return _camera;}
        }

        public TriangleRaytraceRenderer(GraphicsDeviceManager graphicsDeviceManager, WorldGrid grid, int width, int height, ISkyMap skyMap)
        {
            _grid = grid;
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
            ////test av hur det kan se ut med punkter som flyttar sig
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

            _graphicsDeviceManager.GraphicsDevice.Indices = _indexBuffer;//Samma varje gång, anropa en gång! (fast det går inte om textures ska ritas ut också)

        }

        private bool isPositionOutsideBoundaries(float x, float y)
        {
            return x < _minPos.X || x > _maxPos.X || y < _minPos.Y || y > _maxPos.Y;
        }

        public void Render(GameTime gameTime)
        {
            _cycleRadians += (float)gameTime.ElapsedGameTime.TotalSeconds / 2;

            double maxDistance = Math.Sqrt(_maxPos.X * _maxPos.X + _maxPos.Y * _maxPos.Y);

            Parallel.For(0, _vertices.Length, (t) =>
            //for (int t = 0; t < _vertices.Length; t++)
            {
                //Punkter som flyttar sig
                //_vertices[t].Position.X = _orgVertices[t].X + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST);
                //_vertices[t].Position.Y = _orgVertices[t].Y + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST);
                //_vertices[t].Color = RenderPosition(_vertices[t].Position.X, _vertices[t].Position.Y);
                //Rays med punkter som flyttar sig
                //_vertices[t].Color = RenderPosition(_vertices[t].Position.X + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST), _vertices[t].Position.Y + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST), maxDistance);

                _vertices[t].Color = RenderPosition(_vertices[t].Position.X, _vertices[t].Position.Y, maxDistance);
            }
            );


            //Added because of possible use of spritebatch
            _graphicsDeviceManager.GraphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDeviceManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            _vertexBuffer.SetData<VertexPositionColor>(_vertices);
            _indexBuffer.SetData(_indices);//Samma varje gång, anropa en gång! (fast det går inte om textures ska ritas ut också)
            _graphicsDeviceManager.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDeviceManager.GraphicsDevice.Indices = _indexBuffer;//Samma varje gång, anropa en gång! (fast det går inte om textures ska ritas ut också)
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //_graphicsDeviceManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length);
                _graphicsDeviceManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indices.Length / 3);
            }
        }
        private Color RenderPosition(float x, float y, double maxDistance)
        {
            Vector3 lightsourcePos = new Vector3(40 * (float)Math.Cos(_cycleRadians), 40, 40 * (float)Math.Sin(_cycleRadians));
            Ray ray = _camera.SpawnRay(x, y, maxDistance);
            Color pixel = Color.Black;
            Intersection closestIntersection = getClosestIntersectionUsingVoxelTraverse(ray, RENDER_DISTANCE);
            //Intersection closestIntersection = getClosestIntersectionByCheckingAllEntities(ray, RENDER_DISTANCE);

            if (closestIntersection.IsHit())
            {
                Vector3 direction = lightsourcePos - closestIntersection.GetPositionFirstHit();
                float distance = direction.Length();
                float factor = Vector3.Dot(closestIntersection.GetNormalFirstHit(), Vector3.Normalize(direction));
                //Använd avstånd för att minska ljuset
                factor *= (float)(LIGHTSOURCE_INTENSITY / Math.Pow(distance, 2));
                factor += AMBIENT_INTENSITY;
                //sätt gränserna till mellan 0 och 1
                factor = (factor < 0) ? 0 : ((factor > 1) ? 1 : factor);

                //Test av texturer
                pixel = Color.Multiply(Color.Lerp(closestIntersection.GetSphere().GetColor(), closestIntersection.GetSphere().GetTextureMap().GetColorFromDirection(closestIntersection.GetNormalFirstHitTexture()),0.95f), factor);
                //pixel = Color.Multiply(closestIntersection.GetSphere().GetColor(), factor);
            }
            else
                pixel = _skyMap.GetColorInSky(ray.GetDirection());
            return pixel;
        }

        private Intersection getClosestIntersectionUsingVoxelTraverse(Ray ray, int distance)
        {
            //närmaste träff 
            Intersection closestIntersection = new Intersection(true);
            //TODO: Snabba på genom att bygga upp en struktur med grid hitboxar som är tomma?

            Vector3 uPosition = ray.GetStart();
            Vector3 vDirection = ray.GetDirection();
            float scaledX = uPosition.X + 0.5f;
            float scaledY = uPosition.Y + 0.5f;
            float scaledZ = uPosition.Z + 0.5f;

            int voxelX = (int)Math.Floor(scaledX);
            int voxelY = (int)Math.Floor(scaledY);
            int voxelZ = (int)Math.Floor(scaledZ);

            int stepX = Math.Sign(vDirection.X);
            int stepY = Math.Sign(vDirection.Y);
            int stepZ = Math.Sign(vDirection.Z);

            int cellBoundaryX = voxelX + (stepX > 0 ? 1 : 0);
            int cellBoundaryY = voxelY + (stepY > 0 ? 1 : 0);
            int cellBoundaryZ = voxelZ + (stepZ > 0 ? 1 : 0);

            float tMaxX = (cellBoundaryX - scaledX) / vDirection.X;
            float tMaxY = (cellBoundaryY - scaledY) / vDirection.Y;
            float tMaxZ = (cellBoundaryZ - scaledZ) / vDirection.Z;

            if (Single.IsNaN(tMaxX) || Single.IsNegativeInfinity(tMaxX)) tMaxX = Single.PositiveInfinity;
            if (Single.IsNaN(tMaxY) || Single.IsNegativeInfinity(tMaxY)) tMaxY = Single.PositiveInfinity;
            if (Single.IsNaN(tMaxZ) || Single.IsNegativeInfinity(tMaxZ)) tMaxZ = Single.PositiveInfinity;

            float tDeltaX = stepX / vDirection.X;
            float tDeltaY = stepY / vDirection.Y;
            float tDeltaZ = stepZ / vDirection.Z;

            if (Single.IsNaN(tDeltaX) || Single.IsNegativeInfinity(tDeltaX)) tDeltaX = Single.PositiveInfinity;
            if (Single.IsNaN(tDeltaY) || Single.IsNegativeInfinity(tDeltaY)) tDeltaY = Single.PositiveInfinity;
            if (Single.IsNaN(tDeltaZ) || Single.IsNegativeInfinity(tDeltaZ)) tDeltaZ = Single.PositiveInfinity;

            int step = 0;
            do
            {
                Entity entityInPosition = _grid.GetEntityByVoxelPosition(voxelX, voxelY, voxelZ);
                if (entityInPosition != null)
                {
                    Intersection intersection = entityInPosition.Intersect(ray);
                    if ((! intersection.IsNull()) && (intersection.GetTFirstHit() > 0) && intersection.GetTFirstHit() < closestIntersection.GetTFirstHit())
                    {
                        closestIntersection = intersection;
                        break;
                    }
                }

                if (tMaxX < tMaxY && tMaxX < tMaxZ)
                {
                    voxelX += stepX;
                    tMaxX += tDeltaX;
                }
                else if (tMaxY < tMaxZ)
                {
                    voxelY += stepY;
                    tMaxY += tDeltaY;
                }
                else
                {
                    voxelZ += stepZ;
                    tMaxZ += tDeltaZ;
                }

                step++;
            } while (step < distance);

            return closestIntersection;
        }

        ////Testa att raytracea alla spheres (för att jämföra vid felsökning)
        public Intersection getClosestIntersectionByCheckingAllEntities(Ray ray, float distance)
        {
            //närmaste träff 
            Intersection closestIntersection = new Intersection(true);

            foreach (Entity entityInPosition in _grid.GetEntityCollection())
            {
                Intersection intersection = ((Sphere)entityInPosition).Intersect(ray);
                if ((!intersection.IsNull()) && (intersection.GetTFirstHit() > 0) && intersection.GetTFirstHit() < closestIntersection.GetTFirstHit())
                {
                    closestIntersection = intersection;
                }
            }

            return closestIntersection;
        }

    }
}
