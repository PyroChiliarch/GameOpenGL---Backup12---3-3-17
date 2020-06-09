using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace GameOpenGL
{
    class PhysicsObject : Object
    {
        Vector3 velocity = new Vector3(0, 0, 0);
        float mass = 1f;
    }
}
