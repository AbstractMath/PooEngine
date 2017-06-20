using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PooEngine
{
    //The baseObject class introduces the hierarchy structure. All game objects should inherit from this class.
    public abstract class baseObject
    {
        baseObject firstChild = null;
        baseObject nextSibling = null;
        baseObject previousSibling = null;
        baseObject _parent = null;
        public string Name = "";

        public baseObject Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                this.Remove();
                value.addChild(this);
            }
        }

        public void addChild(baseObject newChild) // the object this is called upon is assumed to be the parent. Also operation time is O(1). :)
        {
            newChild._parent = this;

            if (this.firstChild == null)
            {
                this.firstChild = newChild;
            }
            else
            {
                newChild.nextSibling = this.firstChild;
                this.firstChild.previousSibling = newChild;
                this.firstChild = newChild;
            }
        }

        public void Remove()//Removes an object from the hierarchy
        {
            if (this.previousSibling != null)
            {
                //Is not the first child
                this.nextSibling.previousSibling = this.previousSibling;
                this.previousSibling.nextSibling = this.nextSibling;
            }
            else if(this.nextSibling != null)
            {
                //Is the first child, but not an only child
                this._parent.firstChild = this.nextSibling;
                this._parent = null;
            }
            else
            {
                this._parent.firstChild = null;
            }
            this.nextSibling = null;
            this.previousSibling = null;
        }

        //This method tries to find a child object by it's name. Throws an exception if it's not there.
        public baseObject getChildByName(string name)//I have not yet tested this, so I'm not entirely sure if it works. 
        {
            baseObject currentChild = this.firstChild;

            while (currentChild != null)
            {
                if (currentChild.Name == name)
                {
                    return currentChild;
                }
                else
                {
                    currentChild = currentChild.nextSibling;
                }
            }
            throw new Exception("Could not find an object named '" + name + "' ");
        }

        public baseObject getFirstChild()
        {
            return this.firstChild;
        }

        public baseObject getSibling()
        {
            return this.nextSibling;
        }


    }

    public class Scene : baseObject // Again, no transformation matricies, this is only to introduce structure.
    {
        public Camera mainCamera;
        public Vector3 lightDirection;

        private void drawingAlgorithm(baseObject currentObj, GraphicsDevice device)
        {
            if (currentObj is meshObject)//Implying that there is rendering code
            {
                meshObject Obj = (meshObject)currentObj;
                Matrix worldMatrix = Obj.getWorldTransform();
                Matrix cameraMatrix = this.mainCamera.getWorldTransform();

                Obj.shader.Parameters["World"].SetValue(Matrix.Invert(worldMatrix));
                Obj.shader.Parameters["View"].SetValue(cameraMatrix);
                Obj.shader.Parameters["Projection"].SetValue(this.mainCamera.projectionMatrix);
                Obj.shader.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                //Give the video card the vertex buffer.
                device.SetVertexBuffer(Obj.objBuffer);
                
                foreach (EffectPass pass in Obj.shader.CurrentTechnique.Passes)//Run the shader.
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, Obj.triangles.Length);
                }
            }

            if (currentObj.getFirstChild() != null)
            {
                drawingAlgorithm(currentObj.getFirstChild(), device);
            }

            if (currentObj.getSibling() != null)
            {
                drawingAlgorithm(currentObj.getSibling(), device);
            }
        }

        public void drawScene(GraphicsDevice device)
        {
            drawingAlgorithm(this, device);
        }

        public Scene(float aspectRatio)
        {
            mainCamera = new Camera(70f, 0.1f, 100f, aspectRatio);
            this.addChild(mainCamera);
            lightDirection = new Vector3(1, 1, 1);
        }
    }

    //The gameObject class contains object transformation data.
    public class gameObject : baseObject // Will contain actual transformation matricies as well as the structure. But no mesh data.
    {
        private Matrix _transform;
        private Vector3 _position;
        private Vector3 _scale;
        private Matrix _rotation;

        public Matrix transform
        {
            get
            {
                return _transform;
            }
        }

        public Matrix rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                updateTransform();
            }
        }

        public Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                updateTransform();
            }
        }

        public Vector3 scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                updateTransform();
            }
        }

        public void updateTransform()
        {
            _transform = Matrix.CreateScale(_scale) * Matrix.CreateTranslation(_position) * _rotation;
        }

        public Matrix getWorldTransform()
        {
            baseObject currentObj = this;
            Matrix currentTransform = Matrix.Identity;

            while (currentObj.GetType() != typeof(Scene))
            {
                if (currentObj is gameObject)
                {
                    gameObject obj = (gameObject)currentObj;
                    currentTransform = obj.transform * currentTransform;
                }
                if (currentObj.GetType() == typeof(Scene))
                {
                    break;
                }
                currentObj = currentObj.Parent;
            }
            return currentTransform;
        }

        public Vector3 getForwardVector()
        {
            return new Vector3(transform.M13, transform.M23, transform.M33);
        }

        public Vector3 getRightVector()
        {
            return new Vector3(transform.M11, transform.M21, transform.M31);
        }

        public Vector3 getUpVector()
        {
            return new Vector3(transform.M12, transform.M22, transform.M32);
        }

        public gameObject()//Overloads for each property will probably be needed
        {
            _transform = Matrix.Identity;
            _rotation = Matrix.Identity;
            _position = Vector3.Zero;
            _scale = new Vector3(1f, 1f, 1f);
            Name = "GameObject";
        }
    }

    //It should be obvious what this does. Contains camera transformation data, as well as the projection matrix.
    public class Camera : gameObject
    {
        private Matrix _projectionMatrix;
        private float _fieldOfView;
        private float _nearPlane;
        private float _farPlane;
        private float _aspectRatio;

        public Matrix projectionMatrix
        {
            get
            {
                return _projectionMatrix;
            }
        }

        public float fieldOfView
        {
            get
            {
                return _fieldOfView;
            }

            set
            {
                _fieldOfView = value;
                updateProjectionMatrix();
            }
        }

        public float aspectRatio
        {
            get
            {
                return _aspectRatio;
            }

            set
            {
                _aspectRatio = value;
                updateProjectionMatrix();
            }
        }

        public float nearPlane
        {
            get
            {
                return _nearPlane;
            }

            set
            {
                _nearPlane = value;
                updateProjectionMatrix();
            }
        }

        public float farPlane
        {
            get
            {
                return _farPlane;
            }

            set
            {
                _farPlane = value;
                updateProjectionMatrix();
            }
        }

        public void updateProjectionMatrix()
        {
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fieldOfView), _aspectRatio, _nearPlane, _farPlane);
        }

        public Camera(float fov, float near, float far, float aspectRatio)
        {
            _fieldOfView = fov;
            _nearPlane = near;
            _farPlane = far;
            _aspectRatio = aspectRatio;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fieldOfView), _aspectRatio, _nearPlane, _farPlane);
        }
    }

    //TODO: FBX importing
    public class meshObject : gameObject
    {
        public Vector3[] verticies;
        public Vector3[] normals;
        public int[] triangles;
        public Effect shader;
        public VertexBuffer objBuffer;
        public bool visible = true;

        public VertexBuffer getObjectBuffer(GraphicsDevice device)
        {
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[triangles.Length];

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = new VertexPositionNormalTexture(this.verticies[this.triangles[i]], normals[(int)System.Math.Floor((double)i / 3)], new Vector2(0, 0));
            }

            VertexBuffer CVBO = new VertexBuffer(device, typeof(VertexPositionNormalTexture), triangles.Length, BufferUsage.WriteOnly);
            CVBO.SetData<VertexPositionNormalTexture>(verts);

            return CVBO;
        }

        public void calculateNormals()
        {
            normals = new Vector3[triangles.Length / 3];

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 a = this.verticies[this.triangles[i]] - this.verticies[this.triangles[i + 1]];
                Vector3 b = this.verticies[this.triangles[i]] - this.verticies[this.triangles[i + 2]];
                normals[i / 3] = Vector3.Normalize((Vector3.Cross(a, b)));
            }
        }
    }

    //This is more or less a test object. It was suprisingly easy to implement, which must be a good thing.
    //Note that this is generally not how normals are stored. Usually, they're stored per vertex. However, since there wouldn't be any smooth shading in the game, I decided it would be best to go ahead and just store them per triangle, instead.
    public class Cube : meshObject
    {
        public void initializeBuffer(GraphicsDevice device)
        {
            base.objBuffer = base.getObjectBuffer(device);
        }

        public Cube(Effect Shader)
        {
            base.shader = Shader;

            base.verticies = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f)
            };

            base.triangles = new int[]
            {
                1, 0, 2,
                3, 1, 2,
                3, 2, 7,
                7, 2, 6,
                7, 1, 3,
                5, 1, 7,
                7, 6, 4,
                5, 7, 4,
                4, 0, 1,
                4, 1, 5,
                6, 2, 4,
                2, 0, 4
            };

            base.calculateNormals();
        }
    }
}
