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
        const float AMBIENT_INTENSITY = 0.005f;
        const int MAX_RAY_RECURSIVE_LEVELS = 5;
        const float LIGHT_DISTANCE_COEF = 1; 

        GraphicsDeviceManager _graphicsDeviceManager;
        //float _cycleRadians = (float)Math.PI;
        VertexPositionColor[] _vertices;
        int[] _indices;
        int _taskNumbers = Environment.ProcessorCount;
        Vector2 _minPos, _maxPos;
        double _maxDistance;

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
            _maxDistance = Math.Sqrt(_maxPos.X * _maxPos.X + _maxPos.Y * _maxPos.Y);

            //This will create the triangles used for drawing the screen
            TriangleProjectionGrid projGrid = new TriangleProjectionGrid(_minPos.X, _minPos.Y, _maxPos.X, _maxPos.Y);

            projGrid.CreateGrid();

            _vertices = projGrid.GetTriangleIndex().GetVerticesPositionColor();

            _vertexBuffer = new VertexBuffer(_graphicsDeviceManager.GraphicsDevice, typeof(VertexPositionColor), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(_vertices);

            _indices = projGrid.GetTriangleIndex().GetIndices();
            _indexBuffer = new IndexBuffer(_graphicsDeviceManager.GraphicsDevice, IndexElementSize.ThirtyTwoBits, _indices.Length, BufferUsage.WriteOnly);
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
            //_cycleRadians += (float)gameTime.ElapsedGameTime.TotalSeconds / 15;

            //Use threading or not
            //for (int t = 0; t < _vertices.Length; t++)
            Parallel.For(0, _vertices.Length, (t) =>
            {
                Ray viewRay = _camera.SpawnRay(_vertices[t].Position.X, _vertices[t].Position.Y, _maxDistance);
                _vertices[t].Color = RenderPosition(viewRay, viewRay, _camera.GetPosition(), new RayColor(Color.Black, 1), 1.0f, 0).GetColor();
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
            _world.GetEntity(0).RotateYaw(-0.0007f);
        }

        private RayColor RenderPosition(Ray viewRay, Ray currentRay, Vector3 startPosition, RayColor pixel, float amountOfRay, int recursive_level)
        {
            if (recursive_level >= MAX_RAY_RECURSIVE_LEVELS || amountOfRay <= 0)
                return pixel;
            //Vector3 lightsourcePos = new Vector3(2000 * (float)Math.Cos(_cycleRadians) , 0, 2000 * (float)Math.Sin(_cycleRadians));
            Intersection closestIntersection = _world.GetClosestIntersection(currentRay, RENDER_DISTANCE);

            if (closestIntersection.IsHit())
            {
                float additiveLightProjectionAmount = 0f;
                for (int i = 0; i < _world.LightsourceCount(); i++)
                {
                    Vector3 lightDirection = Vector3.Normalize(_world.GetLightsource(i).GetPosition() - closestIntersection.GetPositionFirstHit());
                    if (inShadow(new Ray(closestIntersection.GetPositionFirstHit() + lightDirection * 0.5f, lightDirection)))
                        continue;

                    //Use distance to lower the impact of the light
                    float lightDistanceFactor = (float)Math.Max((LIGHT_DISTANCE_COEF / Vector3.DistanceSquared(_world.GetLightsource(i).GetPosition(), closestIntersection.GetPositionFirstHit())), 1);
                    Vector3 lightHitNormal = closestIntersection.GetNormalFirstHit();
                    additiveLightProjectionAmount += lightDistanceFactor * calculateLightAmount(viewRay.GetDirection(), currentRay.GetDirection(), closestIntersection.GetNormalFirstHit(), lightDirection, _world.GetLightsource(i).GetIntensity());
                }

                additiveLightProjectionAmount += AMBIENT_INTENSITY;

                //Use distance to the sphere, to lower the impact of the light
                float hitDistanceFactor = (float)(LIGHT_DISTANCE_COEF / Vector3.DistanceSquared(closestIntersection.GetPositionFirstHit(), startPosition));
                additiveLightProjectionAmount *= hitDistanceFactor;

                //limit the factor to between 0 and 1
                //TODO: Auto exposure to get better results 
                additiveLightProjectionAmount = (float) Math.Max(Math.Min(additiveLightProjectionAmount, 1.0), 0.0);
                float totalLightFactor = pixel.GetRayAmount() * additiveLightProjectionAmount;
                pixel = new RayColor(Color.Multiply(closestIntersection.GetSphere().GetTextureMap().GetColorFromDirection(Vector3.Transform(closestIntersection.GetNormalFirstHitTexture(), closestIntersection.GetSphere().GetTextureRotation())), additiveLightProjectionAmount), totalLightFactor);
            }
            else
                pixel = new RayColor(_skyMap.GetColorInSky(currentRay.GetDirection()), 1);
            return pixel;
        }

        private bool inShadow(Ray ray)
        {
            //TODO: Check with specialized functions
            Intersection closestIntersection = _world.GetClosestIntersection(ray, RENDER_DISTANCE);
            return closestIntersection.IsHit();
        }

        private float calculateLightAmount(Vector3 viewRayDirection, Vector3 rayDirection, Vector3 lightHitNormal, Vector3 lightDirection, float lightSourceIntensity)
        {
            //Projection from the normal (the angle the light hits)
            float lightProjectionAmount = Vector3.Dot(lightDirection, lightHitNormal);
            if (lightProjectionAmount <= 0)
                return 0.0f;
            //Equally distibuted light using lambert
            float lambert = lightProjectionAmount * 0.025f;//currentMat.diffuse;

            //Blinn
            //The direction of Blinn is exactly at mid point of the light ray
            //and the view ray.
            //We compute the Blinn vector and then we normalize it
            //             then we compute the coeficient of blinn
            //             which is the specular contribution of the current light.
            float blinn = 0.0f;
            float viewProjectionAmount = Vector3.Dot(viewRayDirection, lightHitNormal);
            Vector3 blinnDir = Vector3.Normalize(lightDirection - viewRayDirection);
            float temp = (float)Math.Sqrt(Vector3.Dot(blinnDir, blinnDir));
            if (temp != 0.0f)
            {
                blinnDir = (1.0f / temp) * blinnDir;
                blinn = Math.Max(Vector3.Dot(blinnDir, lightHitNormal), 0.0f);
                blinn = (float)Math.Pow(blinn, 280.0f) * 0.9f; //currentMat.power); //currentMat.specular;
            }
            return (lambert + blinn) * lightSourceIntensity;
        }
    }
}



//float red = 0, green = 0, blue = 0;
//float coef = 1.0f;
//int level = 0;
//ray viewRay = { { float(x), float(y), -1000.0f }, { 0.0f, 0.0f, 1.0f } };
//        do 
//        { 
//            // Looking for the closest intersection
//            float t = 2000.0f;
//int currentSphere = -1;

//            for (unsigned int i = 0; i<myScene.sphereContainer.size(); ++i) 
//            { 
//                if (hitSphere(viewRay, myScene.sphereContainer[i], t)) 
//                {
//                    currentSphere = i;
//                }
//            }

//            if (currentSphere == -1)
//                break;

//            point newStart = viewRay.start + t * viewRay.dir;

//// What is the normal vector at the point of intersection ?
//// It's pretty simple because we're dealing with spheres
//vecteur n = newStart - myScene.sphereContainer[currentSphere].pos;
//float temp = n * n;
//            if (temp == 0.0f) 
//                break; 

//            temp = 1.0f / sqrtf(temp);
//n = temp* n;

//material currentMat = myScene.materialContainer[myScene.sphereContainer[currentSphere].materialId]; 

//            // calcul de la valeur d'éclairement au point 
//            for (unsigned int j = 0; j<myScene.lightContainer.size(); ++j) {
//                light current = myScene.lightContainer[j];
//vecteur dist = current.pos - newStart;
//                if (n* dist <= 0.0f)
//                    continue;
//                float t = sqrtf(dist * dist);
//                if ( t <= 0.0f )
//                    continue;
//                ray lightRay;
//lightRay.start = newStart;
//                lightRay.dir = (1/t) * dist;
//// computation of the shadows
//bool inShadow = false; 
//                for (unsigned int i = 0; i<myScene.sphereContainer.size(); ++i) {
//                    if (hitSphere(lightRay, myScene.sphereContainer[i], t)) {
//                        inShadow = true;
//                        break;
//                    }
//                }
//                if (!inShadow) {
//                    // lambert
//                    float lambert = (lightRay.dir * n) * coef;
//red += lambert* current.red * currentMat.red;
//green += lambert* current.green * currentMat.green;
//blue += lambert* current.blue * currentMat.blue;
//                }
//            }

//            // We iterate on the next reflection
//            coef *= currentMat.reflection;
//            float reflet = 2.0f * (viewRay.dir * n);
//viewRay.start = newStart;
//            viewRay.dir = viewRay.dir - reflet* n;

//level++;
//        } 
//        while ((coef > 0.0f) && (level< 10));   

//        imageFile.put((unsigned char)min(blue*255.0f,255.0f)).put((unsigned char)min(green*255.0f, 255.0f)).put((unsigned char)min(red*255.0f, 255.0f));


//color addRay(ray viewRay, scene &myScene)
//{
//    color output = { 0.0f, 0.0f, 0.0f };
//    float coef = 1.0f;
//    int level = 0;
//    do
//    {
//        point ptHitPoint;
//        int currentSphere = -1;
//        {
//            float t = 2000.0f;
//            for (unsigned int i = 0; i < myScene.sphereContainer.size(); ++i)
//            {
//                if (hitSphere(viewRay, myScene.sphereContainer[i], t))
//                {
//                    currentSphere = i;
//                }
//            }
//            if (currentSphere == -1)
//                break;

//            ptHitPoint = viewRay.start + t * viewRay.dir;
//        }
//        vecteur vNormal = ptHitPoint - myScene.sphereContainer[currentSphere].pos;
//        float temp = vNormal * vNormal;
//        if (temp == 0.0f)
//            break;
//        temp = 1.0f / sqrtf(temp);
//        vNormal = temp * vNormal;

//        material currentMat = myScene.materialContainer[myScene.sphereContainer[currentSphere].materialId];

//        ray lightRay;
//        lightRay.start = ptHitPoint;

//        for (unsigned int j = 0; j < myScene.lightContainer.size(); ++j)
//        {
//            light currentLight = myScene.lightContainer[j];

//            lightRay.dir = currentLight.pos - ptHitPoint;
//            float fLightProjection = lightRay.dir * vNormal;

//            if (fLightProjection <= 0.0f)
//                continue;

//            float lightDist = lightRay.dir * lightRay.dir;
//            {
//                float temp = lightDist;
//                if (temp == 0.0f)
//                    continue;
//                temp = invsqrtf(temp);
//                lightRay.dir = temp * lightRay.dir;
//                fLightProjection = temp * fLightProjection;
//            }

//            bool inShadow = false;
//            {
//                float t = lightDist;
//                for (unsigned int i = 0; i < myScene.sphereContainer.size(); ++i)
//                {
//                    if (hitSphere(lightRay, myScene.sphereContainer[i], t))
//                    {
//                        inShadow = true;
//                        break;
//                    }
//                }
//            }


//            if (!inShadow)
//            {
//                float lambert = (lightRay.dir * vNormal) * coef;
//                output.red += lambert * currentLight.intensity.red * currentMat.diffuse.red;
//                output.green += lambert * currentLight.intensity.green * currentMat.diffuse.green;
//                output.blue += lambert * currentLight.intensity.blue * currentMat.diffuse.blue;

//                Blinn
//                The direction of Blinn is exactly at mid point of the light ray
//                and the view ray.
//                We compute the Blinn vector and then we normalize it
//                 then we compute the coeficient of blinn
//                 which is the specular contribution of the current light.

//                float fViewProjection = viewRay.dir * vNormal;
//                vecteur blinnDir = lightRay.dir - viewRay.dir;
//                float temp = blinnDir * blinnDir;
//                if (temp != 0.0f)
//                {
//                    float blinn = invsqrtf(temp) * max(fLightProjection - fViewProjection, 0.0f);
//                    blinn = coef * powf(blinn, currentMat.power);
//                    output += blinn * currentMat.specular * currentLight.intensity;
//                }
//            }
//        }
//        coef *= currentMat.reflection;
//        float reflet = 2.0f * (viewRay.dir * vNormal);
//        viewRay.start = ptHitPoint;
//        viewRay.dir = viewRay.dir - reflet * vNormal;
//        level++;
//    } while ((coef > 0.0f) && (level < 10));
//    return output;
//}






//color addRay(ray viewRay, scene &myScene)
//{
//    color output = { 0.0f, 0.0f, 0.0f };
//    float coef = 1.0f;
//    int level = 0;
//    do
//    {
//        point ptHitPoint;
//        int currentSphere = -1;
//        {
//            float t = 2000.0f;
//            for (unsigned int i = 0; i < myScene.sphereContainer.size(); ++i)
//            {
//                if (hitSphere(viewRay, myScene.sphereContainer[i], t))
//                {
//                    currentSphere = i;
//                }
//            }
//            if (currentSphere == -1)
//            {
//                output += coef * readCubemap(myScene.cm, viewRay);
//                break;
//            }
//            ptHitPoint = viewRay.start + t * viewRay.dir;
//        }
//        vecteur vNormal = ptHitPoint - myScene.sphereContainer[currentSphere].pos;
//        float temp = vNormal * vNormal;
//        if (temp == 0.0f)
//            break;
//        temp = invsqrtf(temp);
//        vNormal = temp * vNormal;

//        material currentMat = myScene.materialContainer[myScene.sphereContainer[currentSphere].materialId];

//        if (currentMat.bump)
//        {
//            float noiseCoefx = float(noise(0.1 * double(ptHitPoint.x), 0.1 * double(ptHitPoint.y), 0.1 * double(ptHitPoint.z)));
//            float noiseCoefy = float(noise(0.1 * double(ptHitPoint.y), 0.1 * double(ptHitPoint.z), 0.1 * double(ptHitPoint.x)));
//            float noiseCoefz = float(noise(0.1 * double(ptHitPoint.z), 0.1 * double(ptHitPoint.x), 0.1 * double(ptHitPoint.y)));

//            vNormal.x = (1.0f - currentMat.bump) * vNormal.x + currentMat.bump * noiseCoefx;
//            vNormal.y = (1.0f - currentMat.bump) * vNormal.y + currentMat.bump * noiseCoefy;
//            vNormal.z = (1.0f - currentMat.bump) * vNormal.z + currentMat.bump * noiseCoefz;

//            temp = vNormal * vNormal;
//            if (temp == 0.0f)
//                break;
//            temp = invsqrtf(temp);
//            vNormal = temp * vNormal;
//        }

//        ray lightRay;
//        lightRay.start = ptHitPoint;

//        for (unsigned int j = 0; j < myScene.lightContainer.size(); ++j)
//        {
//            light currentLight = myScene.lightContainer[j];

//            lightRay.dir = currentLight.pos - ptHitPoint;
//            float fLightProjection = lightRay.dir * vNormal;

//            if (fLightProjection <= 0.0f)
//                continue;

//            float lightDist = lightRay.dir * lightRay.dir;
//            {
//                float temp = lightDist;
//                if (temp == 0.0f)
//                    continue;
//                temp = invsqrtf(temp);
//                lightRay.dir = temp * lightRay.dir;
//                fLightProjection = temp * fLightProjection;
//            }

//            bool inShadow = false;
//            {
//                float t = lightDist;
//                for (unsigned int i = 0; i < myScene.sphereContainer.size(); ++i)
//                {
//                    if (hitSphere(lightRay, myScene.sphereContainer[i], t))
//                    {
//                        inShadow = true;
//                        break;
//                    }
//                }
//            }

//            if (!inShadow)
//            {
//                float lambert = (lightRay.dir * vNormal) * coef;
//                float noiseCoef = 0.0f;
//                switch (currentMat.type)
//                {
//                    case material::turbulence:
//                        {
//                            for (int level = 1; level < 10; level++)
//                            {
//                                noiseCoef += (1.0f / level)
//                                    * fabsf(float(noise(level * 0.05 * ptHitPoint.x,
//                                                        level * 0.05 * ptHitPoint.y,
//                                                        level * 0.05 * ptHitPoint.z)));
//                            };
//                            output = output + coef * (lambert * currentLight.intensity)
//                                    * (noiseCoef * currentMat.diffuse + (1.0f - noiseCoef) * currentMat.diffuse2);
//                        }
//                        break;
//                    case material::marble:
//                        {
//                            for (int level = 1; level < 10; level++)
//                            {
//                                noiseCoef += (1.0f / level)
//                                * fabsf(float(noise(level * 0.05 * ptHitPoint.x,
//                                                    level * 0.05 * ptHitPoint.y,
//                                                    level * 0.05 * ptHitPoint.z)));
//                            };
//                            noiseCoef = 0.5f * sinf((ptHitPoint.x + ptHitPoint.y) * 0.05f + noiseCoef) + 0.5f;
//                            output = output + coef * (lambert * currentLight.intensity)
//                                * (noiseCoef * currentMat.diffuse + (1.0f - noiseCoef) * currentMat.diffuse2);
//                        }
//                        break;
//                    default:
//                        {
//                            output.red += lambert * currentLight.intensity.red * currentMat.diffuse.red;
//                            output.green += lambert * currentLight.intensity.green * currentMat.diffuse.green;
//                            output.blue += lambert * currentLight.intensity.blue * currentMat.diffuse.blue;
//                        }
//                        break;
//                }

//                // Blinn 
//                // The direction of Blinn is exactly at mid point of the light ray 
//                // and the view ray. 
//                // We compute the Blinn vector and then we normalize it
//                // then we compute the coeficient of blinn
//                // which is the specular contribution of the current light.

//                float fViewProjection = viewRay.dir * vNormal;
//                vecteur blinnDir = lightRay.dir - viewRay.dir;
//                float temp = blinnDir * blinnDir;
//                if (temp != 0.0f)
//                {
//                    float blinn = invsqrtf(temp) * max(fLightProjection - fViewProjection, 0.0f);
//                    blinn = coef * powf(blinn, currentMat.power);
//                    output += blinn * currentMat.specular * currentLight.intensity;
//                }
//            }
//        }
//        coef *= currentMat.reflection;
//        float reflet = 2.0f * (viewRay.dir * vNormal);
//        viewRay.start = ptHitPoint;
//        viewRay.dir = viewRay.dir - reflet * vNormal;
//        level++;
//    } while ((coef > 0.0f) && (level < 10));
//    return output;
//}

//float AutoExposure(scene &myScene)
//{
//#define ACCUMULATION_SIZE 16
//    float exposure = -1.0f;
//    float accufacteur = float(max(myScene.sizex, myScene.sizey));

//    accufacteur = accufacteur / ACCUMULATION_SIZE;

//    float mediumPoint = 0.0f;
//    const float mediumPointWeight = 1.0f / (ACCUMULATION_SIZE * ACCUMULATION_SIZE);
//    for (int y = 0; y < ACCUMULATION_SIZE; ++y)
//    {
//        for (int x = 0; x < ACCUMULATION_SIZE; ++x)
//        {
//            ray viewRay = { { float(x) * accufacteur, float(y) * accufacteur, -1000.0f }, { 0.0f, 0.0f, 1.0f } };
//            color currentColor = addRay(viewRay, myScene);
//            float luminance = 0.2126f * currentColor.red
//                            + 0.715160f * currentColor.green
//                            + 0.072169f * currentColor.blue;
//            mediumPoint = mediumPoint + mediumPointWeight * (luminance * luminance);
//        }
//    }

//    float mediumLuminance = sqrtf(mediumPoint);

//    if (mediumLuminance > 0.0f)
//    {
//        exposure = logf(0.5f) / mediumLuminance;
//    }

//    return exposure;
//}




//color addRay(ray viewRay, scene &myScene, context myContext)
//{
//    color output = { 0.0f, 0.0f, 0.0f };
//    float coef = 1.0f;
//    int level = 0;
//    do
//    {
//        point ptHitPoint;
//        vecteur vNormal;
//        material currentMat;
//        {
//            int currentBlob = -1;
//            int currentSphere = -1;
//            float t = 2000.0f;
//            for (unsigned int i = 0; i < myScene.blobContainer.size(); ++i)
//            {
//                if (isBlobIntersected(viewRay, myScene.blobContainer[i], t))
//                {
//                    currentBlob = i;
//                }
//            }
//            for (unsigned int i = 0; i < myScene.sphereContainer.size(); ++i)
//            {
//                if (hitSphere(viewRay, myScene.sphereContainer[i], t))
//                {
//                    currentSphere = i;
//                    currentBlob = -1;
//                }
//            }
//            if (currentBlob != -1)
//            {
//                ptHitPoint = viewRay.start + t * viewRay.dir;
//                blobInterpolation(ptHitPoint, myScene.blobContainer[currentBlob], vNormal);
//                float temp = vNormal * vNormal;
//                if (temp == 0.0f)
//                    break;
//                vNormal = invsqrtf(temp) * vNormal;
//                currentMat = myScene.materialContainer[myScene.blobContainer[currentBlob].materialId];
//            }
//            else if (currentSphere != -1)
//            {
//                ptHitPoint = viewRay.start + t * viewRay.dir;
//                vNormal = ptHitPoint - myScene.sphereContainer[currentSphere].pos;
//                float temp = vNormal * vNormal;
//                if (temp == 0.0f)
//                    break;
//                temp = invsqrtf(temp);
//                vNormal = temp * vNormal;
//                currentMat = myScene.materialContainer[myScene.sphereContainer[currentSphere].materialId];
//            }
//            else
//            {
//                break;
//            }
//        }

//        float bInside;

//        if (vNormal * viewRay.dir > 0.0f)
//        {
//            vNormal = -1.0f * vNormal;
//            bInside = true;
//        }
//        else
//        {
//            bInside = false;
//        }

//        if (currentMat.bump)
//        {
//            float noiseCoefx = float(noise(0.1 * double(ptHitPoint.x), 0.1 * double(ptHitPoint.y), 0.1 * double(ptHitPoint.z)));
//            float noiseCoefy = float(noise(0.1 * double(ptHitPoint.y), 0.1 * double(ptHitPoint.z), 0.1 * double(ptHitPoint.x)));
//            float noiseCoefz = float(noise(0.1 * double(ptHitPoint.z), 0.1 * double(ptHitPoint.x), 0.1 * double(ptHitPoint.y)));

//            vNormal.x = (1.0f - currentMat.bump) * vNormal.x + currentMat.bump * noiseCoefx;
//            vNormal.y = (1.0f - currentMat.bump) * vNormal.y + currentMat.bump * noiseCoefy;
//            vNormal.z = (1.0f - currentMat.bump) * vNormal.z + currentMat.bump * noiseCoefz;

//            float temp = vNormal * vNormal;
//            if (temp == 0.0f)
//                break;
//            temp = invsqrtf(temp);
//            vNormal = temp * vNormal;
//        }

//        float fViewProjection = viewRay.dir * vNormal;
//        float fReflectance, fTransmittance;
//        float fCosThetaI, fSinThetaI, fCosThetaT, fSinThetaT;

//        if (((currentMat.reflection != 0.0f) || (currentMat.refraction != 0.0f)) && (currentMat.density != 0.0f))
//        {
//            // glass-like material, we're computing the fresnel coefficient.

//            float fDensity1 = myContext.fRefractionCoef;
//            float fDensity2;
//            if (bInside)
//            {
//                // We only consider the case where the ray is originating a medium close to the void (or air) 
//                // In theory, we should first determine if the current object is inside another one
//                // but that's beyond the purpose of our code.
//                fDensity2 = context::getDefaultAir().fRefractionCoef;
//            }
//            else
//            {
//                fDensity2 = currentMat.density;
//            }

//            // Here we take into account that the light movement is symmetrical
//            // From the observer to the source or from the source to the oberver.
//            // We then do the computation of the coefficient by taking into account
//            // the ray coming from the viewing point.
//            fCosThetaI = fabsf(fViewProjection);

//            if (fCosThetaI >= 0.999f)
//            {
//                // In this case the ray is coming parallel to the normal to the surface
//                fReflectance = (fDensity1 - fDensity2) / (fDensity1 + fDensity2);
//                fReflectance = fReflectance * fReflectance;
//                fSinThetaI = 0.0f;
//                fSinThetaT = 0.0f;
//                fCosThetaT = 1.0f;
//            }
//            else
//            {
//                fSinThetaI = sqrtf(1 - fCosThetaI * fCosThetaI);
//                // The sign of SinThetaI has no importance, it is the same as the one of SinThetaT
//                // and they vanish in the computation of the reflection coefficient.
//                fSinThetaT = (fDensity1 / fDensity2) * fSinThetaI;
//                if (fSinThetaT * fSinThetaT > 0.9999f)
//                {
//                    // Beyond that angle all surfaces are purely reflective
//                    fReflectance = 1.0f;
//                    fCosThetaT = 0.0f;
//                }
//                else
//                {
//                    fCosThetaT = sqrtf(1 - fSinThetaT * fSinThetaT);
//                    // First we compute the reflectance in the plane orthogonal 
//                    // to the plane of reflection.
//                    float fReflectanceOrtho = (fDensity2 * fCosThetaT - fDensity1 * fCosThetaI)
//                        / (fDensity2 * fCosThetaT + fDensity1 * fCosThetaI);
//                    fReflectanceOrtho = fReflectanceOrtho * fReflectanceOrtho;
//                    // Then we compute the reflectance in the plane parallel to the plane of reflection
//                    float fReflectanceParal = (fDensity1 * fCosThetaT - fDensity2 * fCosThetaI)
//                        / (fDensity1 * fCosThetaT + fDensity2 * fCosThetaI);
//                    fReflectanceParal = fReflectanceParal * fReflectanceParal;

//                    // The reflectance coefficient is the average of those two.
//                    // If we consider a light that hasn't been previously polarized.
//                    fReflectance = 0.5f * (fReflectanceOrtho + fReflectanceParal);
//                }
//            }
//        }
//        else
//        {
//            // Reflection in a metal-like material. Reflectance is equal in all directions.
//            // Note, that metal are conducting electricity and as such change the polarity of the
//            // reflected ray. But of course we ignore that..
//            fReflectance = 1.0f;
//            fCosThetaI = 1.0f;
//            fCosThetaT = 1.0f;
//        }

//        fTransmittance = currentMat.refraction * (1.0f - fReflectance);
//        fReflectance = currentMat.reflection * fReflectance;

//        float fTotalWeight = fReflectance + fTransmittance;
//        bool bDiffuse = false;

//        if (fTotalWeight > 0.0f)
//        {
//            float fRoulette = (1.0f / RAND_MAX) * rand();

//            if (fRoulette <= fReflectance)
//            {
//                coef *= currentMat.reflection;

//                float fReflection = -2.0f * fViewProjection;

//                viewRay.start = ptHitPoint;
//                viewRay.dir += fReflection * vNormal;
//            }
//            else if (fRoulette <= fTotalWeight)
//            {
//                coef *= currentMat.refraction;
//                float fOldRefractionCoef = myContext.fRefractionCoef;
//                if (bInside)
//                {
//                    myContext.fRefractionCoef = context::getDefaultAir().fRefractionCoef;
//                }
//                else
//                {
//                    myContext.fRefractionCoef = currentMat.density;
//                }

//                // Here we compute the transmitted ray with the formula of Snell-Descartes
//                viewRay.start = ptHitPoint;

//                viewRay.dir = viewRay.dir + fCosThetaI * vNormal;
//                viewRay.dir = (fOldRefractionCoef / myContext.fRefractionCoef) * viewRay.dir;
//                viewRay.dir += (-fCosThetaT) * vNormal;
//            }
//            else
//            {
//                bDiffuse = true;
//            }
//        }
//        else
//        {
//            bDiffuse = true;
//        }


//        if (!bInside && bDiffuse)
//        {
//            // Now the "regular lighting"

//            ray lightRay;
//            lightRay.start = ptHitPoint;
//            for (unsigned int j = 0; j < myScene.lightContainer.size(); ++j)
//            {
//                light currentLight = myScene.lightContainer[j];

//                lightRay.dir = currentLight.pos - ptHitPoint;
//                float fLightProjection = lightRay.dir * vNormal;

//                if (fLightProjection <= 0.0f)
//                    continue;

//                float lightDist = lightRay.dir * lightRay.dir;
//                {
//                    float temp = lightDist;
//                    if (temp == 0.0f)
//                        continue;
//                    temp = invsqrtf(temp);
//                    lightRay.dir = temp * lightRay.dir;
//                    fLightProjection = temp * fLightProjection;
//                }

//                bool inShadow = false;
//                {
//                    float t = lightDist;
//                    for (unsigned int i = 0; i < myScene.sphereContainer.size(); ++i)
//                    {
//                        if (hitSphere(lightRay, myScene.sphereContainer[i], t))
//                        {
//                            inShadow = true;
//                            break;
//                        }
//                    }
//                    for (unsigned int i = 0; i < myScene.blobContainer.size(); ++i)
//                    {
//                        if (isBlobIntersected(lightRay, myScene.blobContainer[i], t))
//                        {
//                            inShadow = true;
//                            break;
//                        }
//                    }
//                }

//                if (!inShadow && (fLightProjection > 0.0f))
//                {

//                    float lambert = (lightRay.dir * vNormal) * coef;
//                    float noiseCoef = 0.0f;
//                    switch (currentMat.type)
//                    {
//                        case material::turbulence:
//                            {
//                                for (int level = 1; level < 10; level++)
//                                {
//                                    noiseCoef += (1.0f / level)
//                                        * fabsf(float(noise(level * 0.05 * ptHitPoint.x,
//                                                            level * 0.05 * ptHitPoint.y,
//                                                            level * 0.05 * ptHitPoint.z)));
//                                };
//                                output = output + coef * (lambert * currentLight.intensity)
//                                        * (noiseCoef * currentMat.diffuse + (1.0f - noiseCoef) * currentMat.diffuse2);
//                            }
//                            break;
//                        case material::marble:
//                            {
//                                for (int level = 1; level < 10; level++)
//                                {
//                                    noiseCoef += (1.0f / level)
//                                    * fabsf(float(noise(level * 0.05 * ptHitPoint.x,
//                                                        level * 0.05 * ptHitPoint.y,
//                                                        level * 0.05 * ptHitPoint.z)));
//                                };
//                                noiseCoef = 0.5f * sinf((ptHitPoint.x + ptHitPoint.y) * 0.05f + noiseCoef) + 0.5f;
//                                output = output + coef * (lambert * currentLight.intensity)
//                                    * (noiseCoef * currentMat.diffuse + (1.0f - noiseCoef) * currentMat.diffuse2);
//                            }
//                            break;
//                        default:
//                            {
//                                output.red += lambert * currentLight.intensity.red * currentMat.diffuse.red;
//                                output.green += lambert * currentLight.intensity.green * currentMat.diffuse.green;
//                                output.blue += lambert * currentLight.intensity.blue * currentMat.diffuse.blue;
//                            }
//                            break;
//                    }

//                    // Blinn 
//                    // The direction of Blinn is exactly at mid point of the light ray 
//                    // and the view ray. 
//                    // We compute the Blinn vector and then we normalize it
//                    // then we compute the coeficient of blinn
//                    // which is the specular contribution of the current light.

//                    vecteur blinnDir = lightRay.dir - viewRay.dir;
//                    float temp = blinnDir * blinnDir;
//                    if (temp != 0.0f)
//                    {
//                        float blinn = invsqrtf(temp) * max(fLightProjection - fViewProjection, 0.0f);
//                        blinn = coef * powf(blinn, currentMat.power);
//                        output += blinn * currentMat.specular * currentLight.intensity;
//                    }
//                }
//            }
//            coef = 0.0f;
//        }

//        level++;
//    } while ((coef > 0.0f) && (level < 10));

//    if (coef > 0.0f)
//    {
//        output += coef * readCubemap(myScene.cm, viewRay);
//    }
//    return output;
//}

//float AutoExposure(scene &myScene)
//{
//#define ACCUMULATION_SIZE 16
//    float exposure = -1.0f;
//    float accufacteur = float(max(myScene.sizex, myScene.sizey));

//    accufacteur = accufacteur / ACCUMULATION_SIZE;

//    float mediumPoint = 0.0f;
//    const float mediumPointWeight = 1.0f / (ACCUMULATION_SIZE * ACCUMULATION_SIZE);
//    for (int y = 0; y < ACCUMULATION_SIZE; ++y)
//    {
//        for (int x = 0; x < ACCUMULATION_SIZE; ++x)
//        {

//            if (myScene.persp.type == perspective::orthogonal)
//            {
//                ray viewRay = { { float(x) * accufacteur, float(y) * accufacteur, -1000.0f }, { 0.0f, 0.0f, 1.0f } };
//                color currentColor = addRay(viewRay, myScene, context::getDefaultAir());
//                float luminance = 0.2126f * currentColor.red
//                                + 0.715160f * currentColor.green
//                                + 0.072169f * currentColor.blue;
//                mediumPoint = mediumPoint + mediumPointWeight * (luminance * luminance);
//            }
//            else
//            {
//                vecteur dir = {(float(x)*accufacteur - 0.5f * myScene.sizex) * myScene.persp.invProjectionDistance,
//                                (float(y) * accufacteur - 0.5f * myScene.sizey) * myScene.persp.invProjectionDistance,
//                                1.0f};

//                float norm = dir * dir;
//                // I don't think this can happen but we've never too prudent
//                if (norm == 0.0f)
//                    break;
//                dir = invsqrtf(norm) * dir;

//                ray viewRay = { { 0.5f * myScene.sizex, 0.5f * myScene.sizey, 0.0f }, { dir.x, dir.y, dir.z } };
//                color currentColor = addRay(viewRay, myScene, context::getDefaultAir());
//                float luminance = 0.2126f * currentColor.red
//                                + 0.715160f * currentColor.green
//                                + 0.072169f * currentColor.blue;
//                mediumPoint = mediumPoint + mediumPointWeight * (luminance * luminance);
//            }
//        }
//    }

//    float mediumLuminance = sqrtf(mediumPoint);

//    if (mediumLuminance > 0.0f)
//    {
//        exposure = -logf(1.0f - myScene.tonemap.fMidPoint) / mediumLuminance;
//    }

//    return exposure;
//}
