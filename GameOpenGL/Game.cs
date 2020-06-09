using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using EngineUtility;
using Framework;


namespace GameOpenGL
{
    class Game : GameWindow
    {
        
        int shaderProg;
        int modelMatrixUni;
        int viewMatrixUni;
        int projectionMatrixUni;

        int texUni;

        Camera camera = new Camera();
        UserControl user = new UserControl();

        GraphicsData graphicsData = new GraphicsData();
        List<Object> objectList = new List<Object>();

       

        public Game (int width, int height) : base(width, height)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PyroInput.Input.Initialise(this);
            

            graphicsData.LoadMeshsFromFile();
            graphicsData.LoadTexturesFromFile();

            graphicsData.LoadMeshesToGPU();
            graphicsData.LoadTexturesToGPU();


            //GL.Enable(EnableCap.Texture2D);

            //Enable DepthMask
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthRange(0.0, 1.0);



            //Load Shaders
            List<int> shaders = new List<int>();
            shaders.Add(Shaders.LoadShader(ShaderType.VertexShader, "Graphics/Shaders/Standard.vert"));
            shaders.Add(Shaders.LoadShader(ShaderType.FragmentShader, "Graphics/Shaders/Standard.frag"));

            shaderProg = Shaders.CreateProgram(shaders);

            //Find Uniforms
            modelMatrixUni = GL.GetUniformLocation(shaderProg, "modelMatrix");
            viewMatrixUni = GL.GetUniformLocation(shaderProg, "viewMatrix");
            projectionMatrixUni = GL.GetUniformLocation(shaderProg, "projectionMatrix");
            texUni = GL.GetUniformLocation(shaderProg, "tex");

            //New Lines, Are these needed???, Added because told to
            //GL.ActiveTexture(TextureUnit.Texture0 + 0);
            //GL.BindTexture(TextureTarget.Texture2D, graphicsData.textureIDs["Cube.bmp"].textureID);
            //GL.Uniform1(texUni, 0);

            Matrix4 identityMatrix = Matrix4.Identity;
            //Set Uniform Initial Values
            GL.UseProgram(shaderProg);
            GL.UniformMatrix4(modelMatrixUni, false, ref identityMatrix);
            GL.UniformMatrix4(viewMatrixUni, false, ref identityMatrix);
            GL.UniformMatrix4(projectionMatrixUni, false, ref identityMatrix);
            GL.Uniform1(texUni, 0);     //Set the texture uniform
            GL.UseProgram(0);





            //Doing Stuff Code
            camera.Translate(new Vector3(0, 0.00f, 0f));
            camera.SetProjection(1f, 0.001f, 20f);

            Object o = new Object();
            o.meshID = graphicsData.meshBufferIDs["Cube.obj"].bufferID;
            o.textureID = graphicsData.textureIDs["atex.bmp"].textureID;
            o.Translate(new Vector3(1, 0, 0));
            objectList.Add(o);

            o = new Object();
            o.meshID = graphicsData.meshBufferIDs["Cube.obj"].bufferID;
            o.textureID = graphicsData.textureIDs["Cube2.bmp"].textureID;
            o.Translate(new Vector3(-1, 0, 0));
            objectList.Add(o);

            o = new Object();
            o.meshID = graphicsData.meshBufferIDs["Cube.obj"].bufferID;
            o.textureID = graphicsData.textureIDs["Cube2.bmp"].textureID;
            o.Translate(new Vector3(0, 0.5f, 0));
            objectList.Add(o);
        }








        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(0, 0, 1, 0);
            GL.ClearDepth(1.0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProg);



            //Load Matrices
            Matrix4 identMatrix = Matrix4.Identity;
            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 projectionMatrix = camera.GetProjectionMatrix();
            GL.UniformMatrix4(viewMatrixUni, false, ref viewMatrix);//camer.view matrix
            GL.UniformMatrix4(projectionMatrixUni, false, ref projectionMatrix);//camera.projectionMatrix);


            //Draw All Objects
            for (int i = 0; i < objectList.Count; i++)
            {
                //Vars
                //Grab object information from arrays
                Matrix4 modelMatrix = objectList[i].GetModelMatrix();
                int texture = objectList[i].textureID;
                Structs.MeshBufferInfo mesh = graphicsData.meshBufferIDs["Cube.obj"];

                //Upload Matrix
                GL.UniformMatrix4(modelMatrixUni, false, ref modelMatrix);

                //Set Texture
                GL.ActiveTexture(TextureUnit.Texture0 + 0);
                GL.BindTexture(TextureTarget.Texture2D, texture);

                //Draw Mesh
                GL.BindVertexArray(mesh.vertexArrayID);
                GL.DrawElements(
                    (BeginMode)PrimitiveType.Triangles,
                    mesh.amountOfIndices,
                    DrawElementsType.UnsignedShort,
                    mesh.bufferOffset);
            }

            
            GL.UseProgram(0);
            this.SwapBuffers();


            //Error Checking, for OpenGL error codes
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine(error.ToString());
            }
            
        }
            






        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            float cameraSpeed = 0.05f;
            float cameraRotateSpeed = 0.01f;

            //Camera Translate

            user.TranslateCamera(camera, cameraSpeed);
            user.RotateCamera(camera, cameraRotateSpeed, this.Bounds);

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        



    }
}
