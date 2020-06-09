using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing;

using OpenTK;


namespace EngineUtility
{
    //Contains:
    //1) Constants
    //2) Structs
    //3) GraphicsUtil


    public static class Constants
    {
        //Constants
        public const string meshFolder = "Meshes";
        public const string textureFolder = "Textures";
    }

    public static class Structs
    {
        


        /// <summary>
        /// Holds Raw Mesh Data
        /// Veticie pos, Veticie color, vertex UVs, and elementData for draw plus their lengths
        /// </summary>
        public struct MeshData
        {
            public string name;                 //Name of the mesh
            public Vector3[] vertexPosData;     //Vertex Positions
            public Vector4[] vertexColorData;   //Colour of vertex
            public ushort[] vertexIndiceData;   //Triangle Vertex Pointts
            public Vector2[] uvData;            //UV Positions

            public MeshData(string _name, Vector3[] _vertexPosData, Vector4[] _vertexColorData, ushort[] _vertexIndiceData, Vector2[] _uvData)
            {
                name = _name;
                vertexPosData = _vertexPosData;
                vertexColorData = _vertexColorData;
                vertexIndiceData = _vertexIndiceData;
                uvData = _uvData;

            }
        }


        /// <summary>
        /// Holds raw Image Data
        /// its name for referencing in Dictionarys, filePath and The Bitmap itself
        /// </summary>
        public struct TextureData
        {
            public string name;
            public string filePath;
            public Bitmap bitmap;


            public TextureData(string _name, string _filePath, Bitmap _bitmap)
            {
                name = _name;
                filePath = _filePath;
                bitmap = _bitmap;
            }
        }



        /// <summary>
        /// Maps a meshName to a buffer on the GPU and the offset and everything needed for drawing
        /// To be used for drawing meshes
        /// This data should be cached inside the model
        /// </summary>
        public struct MeshBufferInfo
        {
            //This file can be trimmed down later
            public int vertexArrayID;
            public int bufferID;
            public int bufferOffset;
            public int indexBufferID;
            public int amountOfIndices;

            public MeshBufferInfo(int _vertexArrayID, int _bufferID, int _bufferOffset, int _indexBufferID, int _amountOfIndices)
            {
                vertexArrayID = _vertexArrayID;
                bufferID = _bufferID;
                bufferOffset = _bufferOffset;
                indexBufferID = _indexBufferID;
                amountOfIndices = _amountOfIndices;
            }
        }


        /// <summary>
        /// Maps a textureName to a texture on the GPU
        /// This data should be cached
        /// </summary>
        public struct TextureLocation
        {
            public int textureID;

            public TextureLocation(int _textureID)
            {
                textureID = _textureID;
            }
        }









        /// <summary>
        /// Converts a quaternion to an AxisAngle\n
        /// (x, y, z, angle)\n
        /// Angle is returned in radians!
        /// </summary>
        /// <param name="quat"></param>
        /// <returns></returns>
        public static Vector4 quatToAxisAngle(Quaternion quat)
        {
            //http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToAngle/


            quat.Normalize();

            double angle = 2 * Math.Acos(quat.W);
            double s = Math.Sqrt(1 - (quat.W * quat.W));

            double x;
            double y;
            double z;

            if (s < 0.001) //If s is almost 0 we dont need to normalize
            {
                x = quat.X;
                y = quat.Y;
                z = quat.Z;
            }
            else
            {
                x = quat.X / s;
                y = quat.Y / s;
                z = quat.Z / s;
            }



            return new Vector4((float)x, (float)y, (float)z, (float)angle);

        }
    }

    public static class GraphicsUtil
    {
        /// <summary>
        /// Rotates a vector using a quaternion
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="quat"></param>
        /// <returns></returns>
        public static Vector3 rotateVecByQuat(Vector3 vec, Quaternion quat)
        {
            //http://math.stackexchange.com/questions/40164/how-do-you-rotate-a-vector-by-a-unit-quaternion
            //
            //conjugate is the mirror? of a quaternion, and is a quaternion itself
            //a pure quaternion is a point in space, also W=0, so we can turn translation to a pure quat
            //
            //where p = pure quaternion, q* = quaternion conjugate, v = pure quat based on vector, q = quaternion
            //p = q*vq
            //


            Quaternion conjugate = quat;
            conjugate.Conjugate();

            Quaternion pureQuat = new Quaternion(vec, 0f);

            Quaternion newVec = conjugate * pureQuat * quat;

            return new Vector3(newVec.X, newVec.Y, newVec.Z);
        }
    }
}
