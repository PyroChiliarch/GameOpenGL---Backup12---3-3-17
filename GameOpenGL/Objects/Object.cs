using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

using EngineUtility;

namespace GameOpenGL
{
    /// <summary>
    /// A drawable object
    /// can be given a null mesh / texture, but i dont know why youd wanna do that
    /// </summary>
    class Object
    {
        public int textureID;
        public int meshID;
        
        public Vector3 position;
        public Quaternion rotation;
        public float scale;

        public Object()
        {
            //Sets everything to nill/nothing
            position = new Vector3(0, 0, 0);
            rotation = new Quaternion(0, 0, 0, 1f);
            scale = 1f;
            textureID = 0;
            meshID = 0;
        }



        public virtual void Load ()
        {
            
        }

        public virtual void Update()
        {

        }



        public virtual Matrix4 GetModelMatrix()
        {
            Matrix4 modelMatrix = Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateTranslation(position) * Matrix4.CreateScale(scale);
            return modelMatrix;
        }

        public void Translate(Vector3 translation)
        {

            //position = position + translation;
            position += GraphicsUtil.rotateVecByQuat(translation, rotation);


        }

        public void RelativeTranslate(OpenTK.Vector3 translation)
        {

            //http://math.stackexchange.com/questions/40164/how-do-you-rotate-a-vector-by-a-unit-quaternion
            //
            //conjugate is the mirror? of a quaternion, and is a quaternion itself
            //a pure quaternion is a point in space, also W=0, so we can turn translation to a pure quat
            //
            //where p = pure quaternion, q* = quaternion conjugate, v = pure quat based on vector, q = quaternion
            //p = q*vq
            //

            position += GraphicsUtil.rotateVecByQuat(translation, rotation);

        }

        public void Scale(float scale)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Quaternion newAngle)
        {
            rotation = rotation * newAngle;
            rotation.Normalize();
        }
    }
}
