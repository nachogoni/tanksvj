// Mono Framework
using System;

// Unity Framework
using UnityEngine;

public class Primitive
{
    /// <summary>
    /// Create a quad polygon
    /// </summary>
    /// <param name="dim">Dimension of the quad. The local zero is in the center</param>
    /// <returns></returns>
    public static Mesh CreateQuad(float dim)
    {
        return CreateQuad(dim, "Quad");
    }


    /// <summary>
    /// Create a quad polygon
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="dim"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Mesh CreateQuad(float dim, string name)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(dim, -dim, 0.01f);
        vertices[1] = new Vector3(-dim, -dim, 0.01f);
        vertices[2] = new Vector3(dim, dim, 0.01f);
        vertices[3] = new Vector3(-dim, dim, 0.01f);

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0.0f, 0.0f);
        uv[1] = new Vector2(1.0f, 0.0f);
        uv[2] = new Vector2(0.0f, 1.0f);
        uv[3] = new Vector2(1.0f, 1.0f);

        int[] triangles = new int[] { 1, 0, 3, 3, 0, 2 };

        mesh.name = name;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Calculate the normal of the quad
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Create a cube
    /// </summary>
    /// <param name="dim"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Mesh CreateCube(float dim, string name)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[24];
        vertices[0] = new Vector3(dim, -dim, dim);
        vertices[1] = new Vector3(-dim, -dim, dim);
        vertices[2] = new Vector3(dim, dim, dim);
        vertices[3] = new Vector3(-dim, dim, dim);
        vertices[4] = new Vector3(dim, dim, -dim);
        vertices[5] = new Vector3(-dim, dim, -dim);
        vertices[6] = new Vector3(dim, -dim, -dim);
        vertices[7] = new Vector3(-dim, -dim, -dim);
        vertices[8] = new Vector3(dim, dim, dim);
        vertices[9] = new Vector3(-dim, dim, dim);
        vertices[10] = new Vector3(dim, dim, -dim);
        vertices[11] = new Vector3(-dim, dim, -dim);
        vertices[12] = new Vector3(dim, -dim, -dim);
        vertices[13] = new Vector3(-dim, -dim, dim);
        vertices[14] = new Vector3(-dim, -dim, -dim);
        vertices[15] = new Vector3(dim, -dim, dim);
        vertices[16] = new Vector3(-dim, -dim, dim);
        vertices[17] = new Vector3(-dim, dim, -dim);
        vertices[18] = new Vector3(-dim, -dim, -dim);
        vertices[19] = new Vector3(-dim, dim, dim);
        vertices[20] = new Vector3(dim, -dim, -dim);
        vertices[21] = new Vector3(dim, dim, dim);
        vertices[22] = new Vector3(dim, -dim, dim);
        vertices[23] = new Vector3(dim, dim, -dim);

        Vector2[] uv = new Vector2[24];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        uv[4] = new Vector2(0, 1);
        uv[5] = new Vector2(1, 1);
        uv[6] = new Vector2(0, 1);
        uv[7] = new Vector2(1, 1);
        uv[8] = new Vector2(0, 0);
        uv[9] = new Vector2(1, 0);
        uv[10] = new Vector2(0, 0);
        uv[11] = new Vector2(1, 0);
        uv[12] = new Vector2(0, 0);
        uv[13] = new Vector2(1, 1);
        uv[14] = new Vector2(1, 0);
        uv[15] = new Vector2(0, 1);
        uv[16] = new Vector2(0, 0);
        uv[17] = new Vector2(1, 1);
        uv[18] = new Vector2(1, 0);
        uv[19] = new Vector2(0, 1);
        uv[20] = new Vector2(0, 0);
        uv[21] = new Vector2(1, 1);
        uv[22] = new Vector2(1, 0);
        uv[23] = new Vector2(0, 1);

        int[] triangles = new int[] { 1, 0, 3, 3, 0, 2, 9, 8, 5, 5, 8, 4, 11, 10, 7, 7, 10, 6, 14, 12, 13, 13, 12, 15, 18, 16, 17, 17, 16, 19, 22, 20, 21, 21, 20, 23};

        mesh.name = name;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Calculate the normal of the quad
        mesh.RecalculateNormals();

        return mesh;
    }

}
