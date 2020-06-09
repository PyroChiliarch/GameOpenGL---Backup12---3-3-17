using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace GameOpenGL
{
    class Camera : Object
    {
        public Matrix4 projectionMatrix;

        public float zoom;
        public float clipPlaneNear;
        public float clipPlaneFar;

        public Camera ()
        {
            zoom = 1f;
            clipPlaneNear = 0.01f;
            clipPlaneFar = 20f;
        }

        

        public void SetProjection (float newZoom, float newClipPlaneNear, float newClipPlaneFar) 
        {
            
            projectionMatrix = new Matrix4();
            projectionMatrix = Matrix4.Identity;

            projectionMatrix.M11 = newZoom;
            projectionMatrix.M22 = newZoom;
            projectionMatrix.M33 = (newClipPlaneFar + newClipPlaneNear) / (newClipPlaneNear - newClipPlaneFar);
            projectionMatrix.M43 = (2 * newClipPlaneFar * newClipPlaneNear) / (newClipPlaneNear - newClipPlaneFar);
            projectionMatrix.M34 = 1f;

        }

        /// <summary>
        /// Alias for GetModelMatrix()
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetViewMatrix()
        {
            return GetModelMatrix();
        }

        public override Matrix4 GetModelMatrix()
        {
            //The camera has to move position then rotate, unlike a normal object
            //This is why ovveride
            Matrix4 modelMatrix = Matrix4.CreateTranslation(position) * Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateScale(scale);
            return modelMatrix;
        }

        public Matrix4 GetProjectionMatrix ()
        {
            return projectionMatrix;
        }

        
    }
}
