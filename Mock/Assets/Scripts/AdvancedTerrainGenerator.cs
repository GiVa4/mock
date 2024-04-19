using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AdvancedTerrainGenerator : MonoBehaviour
{
    [SerializeField] protected MeshFilter _filter;
    [SerializeField] protected Vector2 _offset = Vector2.one;
    [SerializeField] protected bool _allowGenerateLoop = false;
    [SerializeField] protected Vector2 _heightRange = new Vector2(-1, 1);
    [SerializeField] [Range(0f, 10f)] protected float _scale = 0.5f;
    [SerializeField] [Range(0.1f, 10f)] protected float _mountainScale = 2;
    [SerializeField] [Range(0f, 1f)] protected float _mountainThreshold = 0.8f;
    [SerializeField] protected float _mountainAddedHeight = 1f;

    [Header("Vegetation")] 
    [SerializeField] protected float _vegetationScale = 0.1f;
    [SerializeField] [Range(0f, 1f)] protected float _plainTreeChance = 0.5f; 
    [SerializeField] [Range(0f, 1f)] protected float _plainBushChance = 0.5f;
    [SerializeField] protected float _valleyHeight = 0.25f;
    [SerializeField] protected float _mountainBaseHeight = 0.6f;
    
    private Mesh _mesh;
    
    private void Awake()
    {
        Gaia.Init();
        _mesh = Instantiate(_filter.sharedMesh);
        GenerateTerrain();
    }

    private void FixedUpdate()
    {
        if (_allowGenerateLoop)
        {
            GenerateTerrain();
        }
    }

    private void GenerateTerrain()
    {
        Vector3[] vertices = _mesh.vertices;

        GenerateHeight(ref vertices);
        
        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();
        _filter.mesh = _mesh;
        
        SpawnVegetation(ref vertices);
    }
    
    private void GenerateHeight(ref Vector3[] vertices)
    {
        float plainRange = _heightRange.y - _heightRange.x;
        float mountainSize = _scale * _mountainScale;
        
        //Calcular coordenadas lcoales
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            float x = (vertex.x + _offset.x);
            float z = (vertex.z + _offset.y);
            float y = GetCloudValue(x, z, _scale);

            float adjustedY = _heightRange.x + y * plainRange;

            float mountainValue = GetCloudValue(x, z, mountainSize);

            if (mountainValue > _mountainThreshold)
            {
                float weight = (mountainValue - _mountainThreshold) / (1 - _mountainThreshold);
                adjustedY += mountainValue * _mountainAddedHeight * weight;
            }
            
            vertex.y = adjustedY;
            
            vertices[i] = vertex;
        }
    }

    private float GetCloudValue(float x, float z, float scale)
    {
        var mesh = _filter.mesh;
        x = x / mesh.bounds.size.x;
        z = z / mesh.bounds.size.z;

        float firstLayer = GetNoise(x, z, scale) * 0.5f;
        float secondLayer = GetNoise(x, z, scale * 2) * 0.33f;
        float thirdLayer = GetNoise(x, z, scale * 4) * 0.17f;

        return firstLayer + secondLayer + thirdLayer;
    }
    
    private float GetNoise(float x, float z, float frequency)
    {
        return Mathf.PerlinNoise(x * frequency, z * frequency);
    }

    private void SpawnVegetation(ref Vector3[] vertices)
    {
        if (_allowGenerateLoop)
        {
            foreach (Transform child in _filter.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        for (int i = 0; i < vertices.Length; ++i)
        {
            float y = vertices[i].y;

            if (y >= _valleyHeight && y < _mountainBaseHeight)
            {
                if (UnityEngine.Random.Range(0f, 1f) < _plainTreeChance)
                {
                    SpawnAt(vertices[i], Gaia.GetRandomTree(true));
                }
                else if (UnityEngine.Random.Range(0f, 1f) < _plainBushChance)
                {
                    SpawnAt(vertices[i], Gaia.GetRandomBush(true));
                }
            }
        }
    }

    private void SpawnAt(Vector3 point, GameObject prefab)
    {
        GameObject go = Instantiate(prefab, _filter.transform);
        go.transform.position = point;
        go.transform.localScale = new Vector3(_vegetationScale, _vegetationScale, _vegetationScale);
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        GenerateTerrain();
    }
    
}
