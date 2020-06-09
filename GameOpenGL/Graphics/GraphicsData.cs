using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EngineUtility;
using Framework;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Drawing;

namespace GameOpenGL
{
    public class GraphicsData
    {

        //The actual data of textures and meshes
        Structs.MeshData[] meshData;
        Structs.TextureData[] textureData;

        //The mapping of textures and meshes to GPU Buffers
        public Dictionary<string, Structs.MeshBufferInfo> meshBufferIDs;
        public Dictionary<string, Structs.TextureLocation> textureIDs;

        public GraphicsData ()
        {
            meshBufferIDs = new Dictionary<string, Structs.MeshBufferInfo>();
            textureIDs = new Dictionary<string, Structs.TextureLocation>();
        }





        /// <summary>
        /// Loads all meshes (from "meshFolder" path in Structs)
        /// </summary>
        public void LoadMeshsFromFile ()
        {
            //Check Directory for files
            DirectoryInfo meshFolderInfo = new DirectoryInfo(Constants.meshFolder);
            FileInfo[] meshFiles = meshFolderInfo.GetFiles("*.obj");

            //Create the Array for loading into
            meshData = new Structs.MeshData[meshFiles.Length];

            //Load Meshes to Meshdata Array
            for (int i = 0; i < meshFiles.Length; i++)
            {
                meshData[i] = Framework.Meshes.LoadMesh(meshFiles[i].Name, meshFiles[i].FullName);
            }
            Console.WriteLine("Loaded " + meshData.Length + " Mesh/s");
        }

        





        /// <summary>
        /// Loads all textures (from "textureFolder" path in Structs)
        /// </summary>
        public void LoadTexturesFromFile ()
        {
            //Check Directory for files
            DirectoryInfo textureFolderInfo = new DirectoryInfo(Constants.textureFolder);
            FileInfo[] textureFiles = textureFolderInfo.GetFiles("*.bmp");

            //Create the Array for loading into
            textureData = new Structs.TextureData[textureFiles.Length];

            //Load textures into an array
            for (int i = 0; i < textureFiles.Length; i++)
            {
                textureData[i] = Framework.Textures.LoadTexture(textureFiles[i].FullName, textureFiles[i].Name);
            }
            Console.WriteLine("Loaded " + textureData.Length + " Texture/s");
        }




        public void LoadMeshesToGPU()
        {
            for (int i = 0; i < meshData.Length; i++)
            {
                Console.WriteLine("Mesh Load: " + meshData[i].name);

                //Make buffers
                int vertexArray = GL.GenVertexArray();
                int vertexBuffer = GL.GenBuffer();
                int indexBuffer = GL.GenBuffer();



                //Update Dictionary
                meshBufferIDs.Add(
                    meshData[i].name,
                    new Structs.MeshBufferInfo(
                        vertexArray,
                        vertexBuffer,
                        0,
                        indexBuffer,
                        meshData[i].vertexIndiceData.Length));


                //Fill buffers
                //Bind the VAO
                GL.BindVertexArray(vertexArray);
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                    {
                        //Misc Variables that will be helpful
                        int vertexPosSize = (Vector3.SizeInBytes * meshData[i].vertexPosData.Length);
                        int vertexColorSize = (Vector4.SizeInBytes * meshData[i].vertexColorData.Length);
                        int vertexUVSize = (Vector2.SizeInBytes * meshData[i].uvData.Length);

                        int vertexBufferSize = (vertexPosSize + vertexColorSize + vertexUVSize);


                        int vertexIndexSize = (2 * meshData[i].vertexIndiceData.Length); //ushort is 2 bytes in size


                        //the vertex buffer will hold bulk information about vertices
                        //pos, color and UV coords
                        
                        //Input Vertex Pos
                        GL.BufferData(
                            BufferTarget.ArrayBuffer,
                            (IntPtr)(vertexBufferSize),
                            meshData[i].vertexPosData,
                            BufferUsageHint.StaticDraw);

                        //Input Color Data
                        GL.BufferSubData(
                            BufferTarget.ArrayBuffer,
                            (IntPtr)(vertexPosSize),
                            (IntPtr)(vertexColorSize),
                            meshData[i].vertexColorData);

                        //Input UV Data
                        GL.BufferSubData(
                            BufferTarget.ArrayBuffer,
                            (IntPtr)(vertexPosSize + vertexColorSize),
                            (IntPtr)(vertexUVSize),
                            meshData[i].uvData);



                        //The element array buffer is used to draw polygons and specifies which vertices to use
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);

                        //Add polygon Indices
                        GL.BufferData(
                            BufferTarget.ElementArrayBuffer,
                            (IntPtr)(vertexIndexSize),
                            meshData[i].vertexIndiceData,
                            BufferUsageHint.StaticDraw);


                        //Now that the Data is in 
                        GL.EnableVertexAttribArray(0);
                        GL.EnableVertexAttribArray(1);
                        GL.EnableVertexAttribArray(2);
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                        GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, vertexPosSize);
                        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, vertexPosSize + vertexColorSize);
                    }
                }
                GL.BindVertexArray(0);
            }
        }



        /// <summary>
        /// Loads its own Textures to the GPU
        /// </summary>
        public void LoadTexturesToGPU()
        {
            for (int i = 0; i < textureData.Length; i++)
            {
                //Make space on graphics card for texture
                int textureID = GL.GenTexture();

                //Add the ID to the dictionary
                textureIDs.Add(
                    textureData[i].name,
                    new Structs.TextureLocation(textureID));

                //Pass texture to graphics card
                //Start working with texture
                GL.ActiveTexture(TextureUnit.Texture0 + 0); //? needed?
                GL.BindTexture(TextureTarget.Texture2D, textureID);

                //Set the settings for GPU Texture, Nearest means minecraft like
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

                var bmp = textureData[i].bitmap;
                var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                //Pass texture data to GPU texture
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    textureData[i].bitmap.Width,
                    textureData[i].bitmap.Height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
                bmp.UnlockBits(data);


                //Stop working with texture
                GL.BindTexture(TextureTarget.Texture2D, 0);


                Console.WriteLine("Tex Load: " + textureData[i].name + " At " + textureID);
                //Now the bitmap is on the graphics card and it is in the dictionary for easy reference
            }
        }
    }
}
