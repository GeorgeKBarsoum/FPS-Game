using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using GlmNet;

namespace Graphics
{
    class Model3D
    {
        vec3 maxPos;
        vec3 minPos;

        Scene assimpNetScene;
        List<Mesh> netMeshes;
        List<Animation> netAnimation;
        List<Material> netMaterials;
        List<EmbeddedTexture> netTextures;
        List<Model> meshes;
        Texture tex;
        public mat4 scalematrix;
        public mat4 transmatrix;
        public mat4 rotmatrix;
        string RootPath;
        public Model3D()
        {

            maxPos.x = float.MinValue;
            maxPos.y = float.MinValue;
            maxPos.z = float.MinValue;

            minPos.x = float.MaxValue;
            minPos.y = float.MaxValue;
            minPos.z = float.MaxValue;

            scalematrix = new mat4(1);
            transmatrix = new mat4(1);
            rotmatrix = new mat4(1);
        }
        public void LoadFile(string path,string fileName, int texUnit)
        {
            RootPath = path;
            var assimpNetimporter = new Assimp.AssimpContext();
            assimpNetScene = assimpNetimporter.ImportFile(path+ "\\" + fileName);
            Initialize(texUnit);
        }

        void Initialize(int texUnit)
        {
            //animations
            netAnimation = assimpNetScene.Animations;

            //meshes
            netMeshes = assimpNetScene.Meshes;

            //material
            netMaterials = assimpNetScene.Materials;

            //Textures
            netTextures = assimpNetScene.Textures;

            //Nodes
            var netRootNodes = assimpNetScene.RootNode;
            
            if (netMaterials.Count > 0)
            {
                for (int i = 0; i < netMaterials.Count; i++)
                {
                    if (netMaterials[i].HasTextureDiffuse)
                    {
                        tex = new Texture(RootPath + "\\" + netMaterials[i].TextureDiffuse.FilePath, texUnit, true);
                        break;
                    }
                }
            }

            meshes = new List<Model>();
            ConvertToMeshes(netRootNodes);

        }
        void ConvertToMeshes(Node node)
        {
            if (node.HasMeshes)
            {
                for (int i = 0; i < node.MeshIndices.Count; i++)
                {
                    Model m = new Model();
                    var mesh = netMeshes[node.MeshIndices[i]];
                    
                    for (int j = 0; j < mesh.Vertices.Count; j++)
                    {
                        maxPos.x = Math.Max(mesh.Vertices[j].X, maxPos.x);
                        maxPos.y = Math.Max(mesh.Vertices[j].Y, maxPos.y);
                        maxPos.z = Math.Max(mesh.Vertices[j].Z, maxPos.z);

                        minPos.x = Math.Min(mesh.Vertices[j].X, minPos.x);
                        minPos.y = Math.Min(mesh.Vertices[j].Y, minPos.y);
                        minPos.z = Math.Min(mesh.Vertices[j].Z, minPos.z);

                        m.vertices.Add(new vec3(mesh.Vertices[j].X, mesh.Vertices[j].Y, mesh.Vertices[j].Z));
                        if(mesh.TextureCoordinateChannels[0].Count > 0)
                            m.uvCoordinates.Add(new vec2(mesh.TextureCoordinateChannels[0][j].X, mesh.TextureCoordinateChannels[0][j].Y));
                        if (mesh.VertexColorChannelCount > 0)
                            m.colors.Add(new vec3(mesh.VertexColorChannels[0][j].R, mesh.VertexColorChannels[0][j].G, mesh.VertexColorChannels[0][j].B));
                        if (mesh.HasNormals)
                            m.normals.Add(new vec3(mesh.Normals[j].X, mesh.Normals[j].Y, mesh.Normals[j].Z));
                    }
                    if (tex != null)
                        m.texture = tex;
                    mat4 transformationMatrix = new mat4(new vec4(node.Transform.A1, node.Transform.A2, node.Transform.A3, node.Transform.A4),
                        new vec4(node.Transform.B1, node.Transform.B2, node.Transform.B3, node.Transform.B4),
                        new vec4(node.Transform.C1, node.Transform.C2, node.Transform.C3, node.Transform.C4),
                        new vec4(node.Transform.D1, node.Transform.D2, node.Transform.D3, node.Transform.D4));
                    m.transformationMatrix = transformationMatrix;
                    m.Initialize();
                    meshes.Add(m);
                }
            }
            if (node.HasChildren)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    ConvertToMeshes(node.Children[i]);
                }
            }
        }

        public void Draw(int matID)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(matID,scalematrix,rotmatrix,transmatrix);
            }
        }

        public vec3 getMaxPos()
        {
            List<mat4> modelmatrices = new List<mat4>() { scalematrix, rotmatrix, transmatrix};
            mat4 trans = MathHelper.MultiplyMatrices(modelmatrices);

            vec4 res = new vec4(1);
            res.x = maxPos.x;
            res.y = maxPos.y;
            res.z = maxPos.z;
            res = trans * res;
            return new vec3(res.x, res.y, res.z);
        }
        public vec3 getMinPos()
        {
            List<mat4> modelmatrices = new List<mat4>() { scalematrix, rotmatrix, transmatrix };
            mat4 trans = MathHelper.MultiplyMatrices(modelmatrices);

            vec4 res = new vec4(1);
            res.x = minPos.x;
            res.y = minPos.y;
            res.z = minPos.z;
            res = trans * res;
            return new vec3(res.x, res.y, res.z);
        }
        public vec3 getBoundingBox()
        {
            vec3 max = getMaxPos();
            vec3 min = getMinPos();
            vec3 bb;
            bb.x = max.x - min.x;
            bb.y = max.y - min.y;
            bb.z = max.z - min.z;
            return bb;
        }


    }
}
