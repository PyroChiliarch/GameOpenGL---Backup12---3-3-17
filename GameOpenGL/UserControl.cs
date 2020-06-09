using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Input;
using System.Drawing;

using EngineUtility;

namespace GameOpenGL
{
    class UserControl
    {
        Vector2 oldMousePos = new Vector2(0, 0);

        public void TranslateCamera (Camera camera, float cameraSpeed)
        {
            if (PyroInput.Input.KeyDown(Key.Q))
            {
                camera.Translate(new Vector3(0, cameraSpeed, 0.00f));
            }

            if (PyroInput.Input.KeyDown(Key.E))
            {
                camera.Translate(new Vector3(0, -cameraSpeed, 0.00f));
            }

            if (PyroInput.Input.KeyDown(Key.A))
            {
                camera.Translate(new Vector3(cameraSpeed, 0.000f, 0.00f));
            }

            if (PyroInput.Input.KeyDown(Key.D))
            {
                camera.Translate(new Vector3(-cameraSpeed, 0.000f, 0.00f));
            }

            if (PyroInput.Input.KeyDown(Key.S))
            {
                camera.Translate(new Vector3(0, 0, cameraSpeed));
            }

            if (PyroInput.Input.KeyDown(Key.W))
            {
                camera.Translate(new Vector3(0, 0, -cameraSpeed));
            }
        }

        public void RotateCamera(Camera camera, float cameraRotateSpeed, Rectangle screenRect)
        {
            Vector2 newMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
            Vector2 mouseDelta = newMousePos - oldMousePos;

            float pitch = mouseDelta.Y * -cameraRotateSpeed;
            float yaw = mouseDelta.X * -cameraRotateSpeed;
            float roll = 0f;

            

            Vector3 vRight = new Vector3(1, 0, 0);
            Vector3 vRelRight = GraphicsUtil.rotateVecByQuat(vRight, camera.rotation);
            Quaternion pitchOrient = Quaternion.FromAxisAngle(vRelRight, pitch);

            Vector3 vUp = new Vector3(0, 1, 0);
            Vector3 vRelUp = GraphicsUtil.rotateVecByQuat(vUp, camera.rotation);
            Quaternion yawOrient = Quaternion.FromAxisAngle(vRelUp, yaw);

            Quaternion orientation = pitchOrient * yawOrient;
            orientation.Normalize();

            camera.rotation *= orientation;
            
            

            OpenTK.Input.Mouse.SetPosition(screenRect.Left + screenRect.Width / 2, screenRect.Top + screenRect.Height / 2);
            oldMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
        }

        

        
    }
}
