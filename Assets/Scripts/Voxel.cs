using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum VoxelState { Dead = 0, Alive = 1, Available = 2 }

public class Voxel : IEquatable<Voxel>
{
    #region Public fields
    private bool _showVoxel;

    public Vector3Int Index;
    public List<Face> Faces = new List<Face>(6);
    public Vector3 Center => (Index + _voxelGrid.Origin) * _size;

    #region kevin code
    private List<Corner> _corners;
    private VoxelState _voxelStatus;

    public List<Corner> Corners
    {
        get
        {
            if (_corners == null)
            {
                _corners = new List<Corner>();
                _corners.Add(_voxelGrid.Corners[Index.x, Index.y, Index.z]);
                _corners.Add(_voxelGrid.Corners[Index.x + 1, Index.y, Index.z]);
                _corners.Add(_voxelGrid.Corners[Index.x, Index.y + 1, Index.z]);
                _corners.Add(_voxelGrid.Corners[Index.x, Index.y, Index.z + 1]);
                _corners.Add(_voxelGrid.Corners[Index.x + 1, Index.y + 1, Index.z]);
                _corners.Add(_voxelGrid.Corners[Index.x, Index.y + 1, Index.z + 1]);
                _corners.Add(_voxelGrid.Corners[Index.x + 1, Index.y, Index.z + 1]);
                _corners.Add(_voxelGrid.Corners[Index.x + 1, Index.y + 1, Index.z + 1]);
            }
            return _corners;
        }
    }
    public bool ShowVoxel
    {
        get
        {
            return _showVoxel;
        }
        set
        {
            _showVoxel = value;
            if (!value)
                _voxelGO.SetActive(value);
            else
                _voxelGO.SetActive(Status == VoxelState.Alive);

        }
    }
    #endregion

    private GameObject _VoxelGo;


    public bool IsActive;
    public Vector3Int Direction;
    public bool Defined;

    #endregion

    #region Protected fields

    protected GameObject _voxelGO;
    protected VoxelGrid _voxelGrid;
    protected float _size;

    #endregion

    public VoxelState Status
    {
        get
        {
            return _voxelStatus;
        }
        set
        {
            _voxelGO.SetActive(value == VoxelState.Alive && _showVoxel);
            _voxelStatus = value;
        }
    }

    #region Contructors

    /// <summary>
    /// Creates a regular voxel on a voxel grid
    /// </summary>
    /// <param name="index">The index of the Voxel</param>
    /// <param name="voxelgrid">The <see cref="VoxelGrid"/> this <see cref="Voxel"/> is attached to</param>
    /// <param name="voxelGameObject">The <see cref="GameObject"/> used on the Voxel</param>
    public Voxel(Vector3Int index, GameObject voxelGo, float sizeFactor = 1f)
    {
        Index = index;
        _size = _voxelGrid.VoxelSize;
        _voxelGO = GameObject.Instantiate(voxelGo, Center, Quaternion.identity);
        _voxelGO.GetComponent<VoxelTrigger>().TriggerVoxel = this;
        _voxelGO.transform.position = (_voxelGrid.Origin + Index) * _size;
        _voxelGO.transform.localScale *= _voxelGrid.VoxelSize * sizeFactor;
        _voxelGO.name = $"Voxel_{Index.x}_{Index.y}_{Index.z}";
        _voxelGO.tag = "Voxel";
        _voxelGO.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Basic");
    }

    public void ShowDirection()
    {
        if (Direction == null) return;
        //DO THIS!!!!
    }
    /// <summary>
    /// Generic constructor, alllows the use of inheritance
    /// </summary>
    public Voxel() { }

    #endregion

    #region Public methods



    /// <summary>
    /// Get the neighbouring voxels at each face, if it exists
    /// </summary>
    /// <returns>All neighbour voxels</returns>
    public IEnumerable<Voxel> GetFaceNeighbours()
    {
        int x = Index.x;
        int y = Index.y;
        int z = Index.z;
        var s = _voxelGrid.GridSize;

        if (x != 0) yield return _voxelGrid.Voxels[x - 1, y, z];
        if (x != s.x - 1) yield return _voxelGrid.Voxels[x + 1, y, z];

        if (y != 0) yield return _voxelGrid.Voxels[x, y - 1, z];
        if (y != s.y - 1) yield return _voxelGrid.Voxels[x, y + 1, z];

        if (z != 0) yield return _voxelGrid.Voxels[x, y, z - 1];
        if (z != s.z - 1) yield return _voxelGrid.Voxels[x, y, z + 1];
    }

    /// <summary>
    /// Activates the visibility of this voxel
    /// </summary>
    public void ActivateVoxel(bool state)
    {
        IsActive = state;
        _voxelGO.GetComponent<MeshRenderer>().enabled = state;
        _voxelGO.GetComponent<BoxCollider>().enabled = state;
    }

    #endregion

    #region Equality checks
    /// <summary>
    /// Checks if two Voxels are equal based on their Index
    /// </summary>
    /// <param name="other">The <see cref="Voxel"/> to compare with</param>
    /// <returns>True if the Voxels are equal</returns>
    public bool Equals(Voxel other)
    {
        return (other != null) && (Index == other.Index);
    }

    /// <summary>
    /// Get the HashCode of this <see cref="Voxel"/> based on its Index
    /// </summary>
    /// <returns>The HashCode as an Int</returns>
    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    internal static void SetState(Voxel state)
    {
        throw new NotImplementedException();
    }

    #endregion


    public void SetColor(Color color)
    {
        _voxelGO.GetComponent<MeshRenderer>().material.color = color;
    }
}