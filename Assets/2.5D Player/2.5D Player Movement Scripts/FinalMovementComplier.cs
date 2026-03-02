using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FinalMovementComplier : MonoBehaviour
{
    Vector3 _finalMovement;
    List<Vector3> _allMovements = new List<Vector3>();

    public CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void AddToFinalMovement(Vector3 value)
    {
        _allMovements.Add(value);
    }

    public Vector3 GetFinalMovement()
    {
        _finalMovement = Vector3.zero;
        foreach (Vector3 movement in _allMovements)
        {
            _finalMovement += movement;
        }
        //Debug.Log("All Movements: " + string.Join(", ", _allMovements));
        _allMovements.Clear();
        return _finalMovement;
    }

    void MovePlayer()
    {
        Vector3 movementDirection = GetFinalMovement();
        characterController.Move(movementDirection * Time.deltaTime);
        //Debug.Log("Final Movement: " + movementDirection);
    }

    private void Update()
    {
        MovePlayer();
    }

}
