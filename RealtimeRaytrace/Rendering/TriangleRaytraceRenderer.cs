using System;
using System.Collections.Generic;
using System.Linq;
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

        GraphicsDeviceManager _graphicsDeviceManager;
        float _cycleRadians = 0;
        VertexPositionColor[] _vertices;
        Vector2[] _orgVertices;
        int[] _indices;
        int _taskNumbers = Environment.ProcessorCount;

        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        Random _rnd = new Random();
        BasicEffect _basicEffect;

        Camera _camera = new Camera(0, new Vector3(0, 0, -3), Vector3.Backward);
        WorldGrid _grid;

        public Camera MainCamera
        {
            get { return _camera;}
        }

        public TriangleRaytraceRenderer(WorldGrid grid, GraphicsDeviceManager graphicsDeviceManager, int width, int height)
        {
            _grid = grid;
            _basicEffect = new BasicEffect(graphicsDeviceManager.GraphicsDevice);
            _graphicsDeviceManager = graphicsDeviceManager;
            Vector2 center;
            _graphicsDeviceManager.PreferredBackBufferHeight = height;
            _graphicsDeviceManager.PreferredBackBufferWidth = width;
#if DEBUG
            _graphicsDeviceManager.IsFullScreen = false;
#else
            _graphicsDeviceManager.IsFullScreen = true;
#endif
            _graphicsDeviceManager.ApplyChanges();
            _graphicsDeviceManager.GraphicsDevice.Textures[0] = null;

            center.X = _graphicsDeviceManager.GraphicsDevice.Viewport.Width * 0.5f;
            center.Y = _graphicsDeviceManager.GraphicsDevice.Viewport.Height * 0.5f;

            //This will create the triangles used for drawing the screen
            TriangleProjectionGrid2 projGrid = new TriangleProjectionGrid2(-center.X, -center.Y, center.X, center.Y);
            projGrid.MakeTriangleHexagonRing(17, 1);

            //TODO: Check if multiples ar valid
            //TODO: The TriangleHexagonRings should be dynamic to the resolution
            //for (int i = 0; i < 160; i += 1)
            //    projGrid.MakeTriangleHexagonRing(i, 1);
            //for (int i = 160; i < 300; i += 2)
            //    projGrid.MakeTriangleHexagonRing(i, 2);


            //for (int i = 0; i < 900; i += 1)
            //    projGrid.MakeTriangleHexagonRing(i, 1);
            //for (int i = 0; i < 900; i += 2)
            //    projGrid.MakeTriangleHexagonRing(i, 2);
            //for (int i = 0; i < 900; i += 3)
            //    projGrid.MakeTriangleHexagonRing(i, 3);

            //for (int i = 40; i < 180; i += 2)
            //    projGrid.MakeTriangleHexagonRing(i, 2);

            //for (int i = 180; i < 120; i += 4)
            //    projGrid.MakeTriangleHexagonRing(i, 4);

            //for (int i = 120; i < 180; i += 6)
            //    projGrid.MakeTriangleHexagonRing(i, 6);

            //for (int i = 180; i < 260; i += 8)
            //    projGrid.MakeTriangleHexagonRing(i, 8);

            _vertices = projGrid.GetTriangleIndex().GetVerticesPositionColor();
            //test av hur det kan se ut
            _orgVertices = new Vector2[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
                _orgVertices[i] = new Vector2(_vertices[i].Position.X, _vertices[i].Position.Y);

            _vertexBuffer = new VertexBuffer(_graphicsDeviceManager.GraphicsDevice, typeof(VertexPositionColor), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(_vertices);

            _indices = projGrid.GetTriangleIndex().GetIndices();
            _indexBuffer = new IndexBuffer(_graphicsDeviceManager.GraphicsDevice, typeof(int), _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);

            _basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, -1), new Vector3(0, 0, 0), Vector3.Down);
            _basicEffect.Projection = Matrix.CreateOrthographic(center.X * 2, center.Y * 2, 1, 2);
            _basicEffect.VertexColorEnabled = true;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            _graphicsDeviceManager.GraphicsDevice.RasterizerState = rasterizerState;

        }

        public void Render(GameTime gameTime)
        {
            _cycleRadians += (float)gameTime.ElapsedGameTime.TotalSeconds / 4;

            Parallel.For(0, _vertices.Length, (t) =>
            //for (int t = 0; t < _vertices.Length; t++)
            {
                //_vertices[t].Position.X = _orgVertices[t].X + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST);
                //_vertices[t].Position.Y = _orgVertices[t].Y + ThreadSafeRandom.Next(-SHAKE_DIST, SHAKE_DIST);
                _vertices[t].Color = RenderPosition(_vertices[t].Position.X, _vertices[t].Position.Y);
            }
            );

            _vertexBuffer.SetData<VertexPositionColor>(_vertices);
            _indexBuffer.SetData(_indices);

            _graphicsDeviceManager.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDeviceManager.GraphicsDevice.Indices = _indexBuffer;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDeviceManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length);
            }

        }

        private Color RenderPosition(float x, float y)
        {
            Vector3 lightsource = new Vector3(40 * (float)Math.Cos(_cycleRadians), 40, 40 * (float)Math.Sin(_cycleRadians));
            Ray ray = new Ray(_camera, x, y);
            Color pixel = Color.Black;
            Intersection closestIntersection = getClosestIntersectionUsingVoxelTraverse(ray, RENDER_DISTANCE);

            if (closestIntersection.IsHit())
            {
                Vector3 direction = lightsource - closestIntersection.GetPosition();
                float distance = (direction).Length();
                float factor = Vector3.Dot(closestIntersection.GetNormal(), Vector3.Normalize(direction));
                //Använd avstånd för att minska ljuset
                factor *= (float)(LIGHTSOURCE_INTENSITY / Math.Pow(distance, 2));
                factor += AMBIENT_INTENSITY;
                //sätt gränserna till mellan 0 och 1
                factor = (factor < 0) ? 0 : ((factor > 1) ? 1 : factor);

                pixel = Color.Multiply(closestIntersection.GetSphere().GetColor(), factor);
            }
            else
                pixel = Color.Black;
            return pixel;
        }

        private Intersection getClosestIntersectionUsingVoxelTraverse(Ray ray, int distance)
        {
            //närmaste träff 
            Intersection closestIntersection = new Intersection();
            //TODO: Snabba på genom att bygga upp en struktur med grid hitboxar som är tomma?

            Vector3 uPosition = ray.GetStart();
            Vector3 vDirection = ray.GetVector();
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
                    Intersection intersection = ((Sphere)entityInPosition).Intersect(ray);
                    if ((intersection != null) && (intersection.GetT() > 0) && intersection.GetT() < closestIntersection.GetT())
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
            Intersection closestIntersection = new Intersection();

            foreach (Entity entityInPosition in _grid.GetEntityCollection())
            {
                Intersection intersection = ((Sphere)entityInPosition).Intersect(ray);
                if ((intersection != null) && (intersection.GetT() > 0) && intersection.GetT() < closestIntersection.GetT())
                {
                    closestIntersection = intersection;
                }
            }

            return closestIntersection;
        }

    }
}
