using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Drawing;

using EngineUtility;

//This is the shader framework, it is for loading shaders from disk (Other things too)

namespace Framework
{


    public static class Textures
    {

        public static Structs.TextureData LoadTexture (string texturePath, string name)
        {
            //Pretty simple, not much to do cause making the bitmap does all the work >.>
            Bitmap texture_source = new Bitmap(texturePath);


            return new Structs.TextureData(
                name,
                texturePath,
                texture_source);
        }
    }




    public static class Meshes
    {
        public static Structs.MeshData LoadMesh (string meshName, string meshPath)
        {
            

            StreamReader streamReader = new StreamReader(meshPath);
            string line;



            string objectName = "Unknown_Name";
            List<Vector3> vPosData = new List<Vector3>();
            List<ushort> vIndiceData = new List<ushort>();
            List<Vector4> vColorData = new List<Vector4>();
            List<Vector2> uvPosData = new List<Vector2>();
            List<ushort> uvIndiceData = new List<ushort>();


            while ((line = streamReader.ReadLine()) != null)
            {

                Console.WriteLine(line);



                //# is a comment, skip it
                if (line.StartsWith("#")) { continue; }




                //o is the object name
                if (line.StartsWith("o"))
                {
                    objectName = line.Substring(2);
                }


                //v mean vPosData
                if (line.StartsWith("v ")) //if a verticePos
                {
                    string[] fields = line.Split(' ');

                    //Skip 0 because it is data type identifier
                    float x = float.Parse(fields[1]);
                    float y = float.Parse(fields[2]);
                    float z = float.Parse(fields[3]);

                    vPosData.Add(new Vector3(x, y, z));

                    //Add Generic Colour to vertice
                    //Temporary, until this code is known to be bug free
                    vColorData.Add(new Vector4(1, 0, 1, 1));
                }



                //vt means uvPosData
                if (line.StartsWith("vt "))
                {
                    string[] fields = line.Split(' ');

                    float x = float.Parse(fields[1]);
                    float y = float.Parse(fields[2]);

                    uvPosData.Add(new Vector2(x, y));
                }



                //if a face indice data info - All the data that is needed for drawing triangles
                if (line.StartsWith("f ")) 
                {


                    string[] segment = line.Split(' ');  //Split the line into seperate Vertice Elements
                    for (int i = 1; i < segment.Length; i++) //Organise the different elements of each vertice, Skip first one cause its a letter to indicate da
                    {
                        string[] fields = segment[i].Split('/');

                        //Add the Vector Pos Indice
                        vIndiceData.Add( (ushort)(ushort.Parse(fields[0]) - 1));


                        //Add the Vector UV Indice
                        if (fields.Length > 1)
                        {
                            uvIndiceData.Add( (ushort)(ushort.Parse(fields[1]) - 1));//Add the UV indice
                        }
                    }



                }
            }

            //Console.WriteLine("Object Name: " + name);
            //Console.WriteLine("Vertice Positions: " + vPosData.Count);
            //Console.WriteLine("Triangles: " + vIndiceData.Count / 3);
            //Console.WriteLine("UV Positions: " + uvPosData.Count);
            //Console.WriteLine("UV Triangles: " + uvIndiceData.Count / 3);



            //Now that we have parsed the file we can change it to something readable by OpenGL
            //We need to make the indices matchup
            List<Vector3> newVPosData = new List<Vector3>();
            List<Vector4> newVColorData = new List<Vector4>();
            List<Vector2> newUVPosData = new List<Vector2>();
            List<ushort> newIndiceData = new List<ushort>();

            

            for (int i = 0; i < vIndiceData.Count; i++)
            {
                newVPosData.Add(vPosData[vIndiceData[i]]);
                newVColorData.Add(vColorData[vIndiceData[i]]); //Using the Indices for vPos since we are fakeing colours
                newUVPosData.Add(uvPosData[uvIndiceData[i]]);
                newIndiceData.Add((ushort)(i));
            }




            return new Structs.MeshData(
                meshName,
                newVPosData.ToArray(),          //Vec3
                newVColorData.ToArray(),        //Vec4
                newIndiceData.ToArray(),        //ufloat
                newUVPosData.ToArray());        //Vector2

        }
    }














    public static class Shaders
    {
        public static int LoadShader(ShaderType eShaderType, string strShaderFilename)
        {
            if (!File.Exists(strShaderFilename))
            {
                Debug.WriteLine("Could not find the file " + strShaderFilename, "Error");
                throw new Exception("Could not find the file " + strShaderFilename);
            }

            int shader = GL.CreateShader(eShaderType);
            using (var streamReader = new StreamReader(strShaderFilename))
            {
                GL.ShaderSource(shader, streamReader.ReadToEnd());
            }

            GL.CompileShader(shader);

            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                int infoLogLength;
                GL.GetShader(shader, ShaderParameter.InfoLogLength, out infoLogLength);

                string strInfoLog;
                GL.GetShaderInfoLog(shader, out strInfoLog);

                string strShaderType;
                switch (eShaderType)
                {
                    case ShaderType.FragmentShader:
                        strShaderType = "fragment";
                        break;
                    case ShaderType.VertexShader:
                        strShaderType = "vertex";
                        break;
                    case ShaderType.GeometryShader:
                        strShaderType = "geometry";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("eShaderType");
                }
                Console.WriteLine("Compile failure in " + strShaderType + " shader:\n" + strInfoLog, "Error");
                Debug.WriteLine("Compile failure in " + strShaderType + " shader:\n" + strInfoLog, "Error");
                throw new Exception("Compile failure in " + strShaderType + " shader:\n" + strInfoLog);
            }

            return shader;
        }

        public static int CreateProgram(List<int> shaderList)
        {
            int program = GL.CreateProgram();

            foreach (int shader in shaderList)
            {
                GL.AttachShader(program, shader);
            }

            GL.LinkProgram(program);

            int status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                int infoLogLength;
                GL.GetProgram(program, GetProgramParameterName.InfoLogLength, out infoLogLength);

                string strInfoLog;
                GL.GetProgramInfoLog(program, out strInfoLog);
                Debug.WriteLine("Linker failure: " + strInfoLog, "Error");
                throw new Exception("Linker failure: " + strInfoLog);
            }

            foreach (int shader in shaderList)
            {
                GL.DetachShader(program, shader);
            }

            return program;
        }

        /// <summary>
        /// Creates a deep clone of an object using serialization.
        /// </summary>
        /// <typeparam name="T">The type to be cloned/copied.</typeparam>
        /// <param name="o">The object to be cloned.</param>
        public static T DeepClone<T>(this T o)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, o);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}