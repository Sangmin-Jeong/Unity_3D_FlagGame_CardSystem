using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Face
{
    public List<Vector3> vertices { get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    private Mesh m_mesh;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;

    private List<Face> m_faces;

    public Material material;


    public float innerSize;
    public float outerSize;
    public float height;
    public float amountOfDots;
    public bool isFlatTopped = false;

    public void start()
    {
        m_meshFilter= GetComponent<MeshFilter>();
        m_meshRenderer= GetComponent<MeshRenderer>();

        m_mesh = new Mesh();
        m_mesh.name = "Hex";

        m_meshFilter.mesh = m_mesh;
        m_meshFilter.mesh.MarkDynamic();
        m_meshFilter.mesh.SetIndices(m_mesh.GetIndices(0), MeshTopology.Lines, 0);

    }


    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
        
    }

    private void DrawFaces ()
    {
        
        m_faces = new List<Face>();

        //Top faces
        for(int point = 0; point < 6; point++)
        {
            for(int dot = 1; dot <= amountOfDots; dot++)
            {
                //if(dot == 1)
                //{
                //    m_faces.Add(CreateFace(innerSize, outerSize, 0, 0, point, dot, amountOfDots));
                //}
                //else if(dot == amountOfDots)
                //{
                //    m_faces.Add(CreateFace(innerSize, outerSize, 0, 0, point, dot, amountOfDots));
                //}
                //else if(dot == 3)
                //{
                //    m_faces.Add(CreateFace(innerSize, outerSize, 0, 0, point, dot, amountOfDots));
                //}
                //else if(dot == 5)
                //{
                //    m_faces.Add(CreateFace(innerSize, outerSize, 0, 0, point, dot, amountOfDots));
                //}

                if (dot % 2 == 1)
                {
                    m_faces.Add(CreateFace(innerSize, outerSize, 0, 0, point, dot, amountOfDots));
                }


            }
            
           
        }

        ////Bottom faces
        //for (int point = 0; point < 6; point++)
        //{
            
        //    m_faces.Add(CreateFace(innerSize, outerSize, -height / 2f, -height / 2f, point, true));
        //}

        ////outer faces
        //for (int point = 0; point < 6; point++)
        //{
            
        //    m_faces.Add(CreateFace(outerSize, outerSize, height / 2f, -height / 2f, point, true));
        //}

        ////inner faces
        //for (int point = 0; point < 6; point++)
        //{
            
        //    m_faces.Add(CreateFace(innerSize, innerSize, -height / 2f, height / 2f, point, true));
        //}

       

    }

    private Face CreateFace(float innerRad,float outerRad, float heightA, float heightB, int point, float amoutDots, float maxAmoutDots, bool reverse = false)
    {
        
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point<5)? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        Vector3 pointG = GetPointBetween(pointA, pointB, amoutDots / maxAmoutDots);
        Vector3 pointY = GetPointBetween(pointD, pointC, amoutDots / maxAmoutDots);

        List<Vector3> vertices;
        if (amoutDots != 1)
        {

            Vector3 pointP = GetPointBetween(pointA, pointB, (amoutDots -1) / maxAmoutDots); ;
            Vector3 pointQ = GetPointBetween(pointD, pointC, (amoutDots -1) / maxAmoutDots); ;
            vertices = new List<Vector3>() { pointP, pointG, pointY, pointQ };
        }
        else
        {

            vertices = new List<Vector3>() { pointA, pointG, pointY, pointD };
        }

        

        List<int> triangles = new List<int>() { 0,1,2,2,3,0};
        List<Vector2> uvs = new List<Vector2>() { new Vector2 (0,0) , new Vector2(1,0), new Vector2(1,1), new Vector2(0, 1) };


        if(reverse)
        {
            vertices.Reverse();
        }

        return new Face(vertices,triangles,uvs);
    }

    protected Vector3 GetPoint(float size, float height, int index)
    {
        float angle_deg = 60*index-30;
        float angle_rad = Mathf.PI / 180f * angle_deg;

        return new Vector3((size* Mathf.Cos(angle_rad)),height, size* Mathf.Sin(angle_rad));
    }
    protected Vector3 GetPointBetween(Vector3 startPoint, Vector3 endPoint, float t)
    {
        // Use Vector3.Lerp to interpolate between the start and end points
        return Vector3.Lerp(startPoint, endPoint, t);
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();   

        for(int i = 0; i < m_faces.Count; i++) 
        {
            // add the vertices
            vertices.AddRange(m_faces[i].vertices);
            uvs.AddRange(m_faces[i].uvs);

            // offset the triangles
            int offset = (4 * i);
            foreach(int triangle in m_faces[i].triangles)
            {
                tris.Add(triangle + offset);
            }

        }

        m_mesh.vertices = vertices.ToArray();
        m_mesh.triangles = tris.ToArray();
        m_mesh.uv = uvs.ToArray();
        m_mesh.RecalculateNormals();
    }

    public void SetMaterial(Material mat)
    {
        m_meshRenderer.material = mat;
        mat.SetFloat("_LineWidth", 0);
    }



    
}
