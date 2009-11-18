//#define CAN_CLIMB_ON_PROPS

// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

/// <summary>
/// AStar Class
/// 
/// A* algorithm implementation. Do not requiere memory allocation in each step.
/// 
/// Dependencies:
/// 
/// UnityEngine
/// AStarBase
/// ArrayStack_AStarBase
/// 
/// </summary>
public class AStar
{
	// ------------------------------------------------
    // Constants
	const int NOT_USING_DIAGONALS = 4;
	const int USING_DIAGONALS = 8;

    // ------------------------------------------------
    // Private Variables
    private AStarBase[] _bases = null;                      // Array of bases
    private int _lastUsedBase;                              // Last used base of the array
    private ArrayList _openBases;                           // List of open bases
    private ArrayStack_AStarBase _closedBases;              // List of closed bases

    private Vector3 _finalPos;                              // Goal position

    private int _possibleMovesCount = NOT_USING_DIAGONALS;  // Type of movements
 

    private TileMap _map;                                   // Map

    private AStarBase _solutionBase;                        // First base to the solution
    private AStarBase _cursorBase;                          // Path to the solution
	private int _solutionLen;

    /// <summary>
    /// Find the path to the goal
    /// </summary>
    /// <param name="start"></param>
    /// <param name="finish"></param>
    public bool Resolve(TileMap map, Vector3 start, Vector3 finish)
    {
        
        _finalPos = finish;
        _map = map;
		_solutionLen = 0;
           
        int count = _map.Cols * _map.Rows * 2;

        if (_bases == null || _bases.Length < count)
            CreateBases(count);
        else
        {
            // Limpio las listas de bases
            _lastUsedBase = 0;
            _openBases.Clear();
            _closedBases.Clear();
        }

        _solutionBase = null;

        return Resolve(new AStarBase(start, 0), 0);
    }

    /// <summary>
    /// Solicito memoria. Esto se realiza sólo en la primera ejecución del
    /// algoritmo o cuando se cambia el tamaño del mapa a uno más grande.
    /// </summary>
    /// <param name="count"></param>
    private void CreateBases(int count)
    {
        _bases = new AStarBase[count];
        _lastUsedBase = 0;

        _openBases = new ArrayList(count);
        _closedBases = new ArrayStack_AStarBase(count);

        for (int i = 0; i < count; i++)
            _bases[i] = new AStarBase();
    }

    /// <summary>
    /// Find the path to the goal. Recursive method.
    /// </summary>
    /// <param name="curBase"></param>
    /// <param name="Gn"></param>
    /// <returns></returns>
    private bool Resolve(AStarBase curBase, float Gn)
    {
        Vector3 outPos;
        float cost;
		
        // --------------------------------------------------------
        // 1. Busco todos los movimientos posibles, desde el lugar
        // donde me encuentro y los inserto en la lista de bases
        // abiertas
        for (uint i = 0; i < _possibleMovesCount; i++)
        {
            // Obtengo una movida posible dentro del mapa
            // (podría ser obstáculo)
            if (_map.GetMove(i, curBase.pos, out outPos))
            {
                // Verifico que no esté en la lista de bases cerradas
                // NOTA: La siguiente búsqueda es LINEAL
                if (IsInClosedBases(outPos) == false)
                {
                    if (_map.IsObstacle(outPos) == false)
                    {
                        cost = Gn + CalculateFn(outPos, _finalPos);

                        AStarBase newBase = GetOpenBase(outPos);
                        
                        // Verifico si debo actualizar costos
                        if (newBase != null)
                        {
                            // Posiblemente sea necesario actualizar el costo
                            if (newBase.cost > cost)
                            {                  
                                newBase.cost = cost;
                                newBase.prevBase = curBase;
                            }
                        }
                        else
                        {
                            if (_lastUsedBase < _bases.Length)
                            {
                                newBase = _bases[_lastUsedBase++];

                                newBase.pos = outPos;
                                newBase.cost = cost;
                                newBase.prevBase = curBase;
                                newBase.nextBase = null;

                                _openBases.Add(newBase);
                            }
                            else
                            {
                                // La cola de bases ya no tiene bases, esto NUNCA 
                                // podría ocurrir. Si es así existe un bug en el algoritmo
                                // ya que en CreateBases se pide memoria para todas las bases
                                // que se necesitan.
                                Debug.Log("There is no bases in the bases queue. A* algorithm error.");
                                return false;
                            }
                        }
                        
                        // Reach the goal?
						// Debug.Log(String.Format("Distance: {0}", Vector3Util.DistanceXZ(outPos, _finalPos)));
						
                        if (Vector3Util.DistanceXZ(outPos, _finalPos) <= _map.GetMaxDistanceToGoal())
                        {
                            newBase.prevBase = curBase;

                            BuildNextBaseReferences(newBase);
                                                            
                            // DebugShowSolution(newBase);

                            return true;
                        }
                    }
                }
            }
			//else
			//	Debug.Log(String.Format("Cannot move to {0}", i));
        }
        
        // --------------------------------------------------------
        // 2. Elijo la "mejor" movida entre las bases abiertas

        AStarBase bestBase = SelectBestMove();

        if (bestBase != null)
        {
			
            // Elimino la base seleccionada de la lista de bases
            // abiertas
            _openBases.Remove(bestBase);

            // Coloco la base actual en la lista de bases cerradas
            _closedBases.Insert(curBase);

            if (Resolve(bestBase, Gn + _map.GetBlockCost(bestBase.pos)) == false)
            {
                // Es necesario cambiar de camino, debo sacar la última base seleccionada
                // como mejor de la lista de bases abiertas
                //Debug.Log("Debo cambiar camino");

                return false;
            }
            else
            {                             
				
                // Relaciono base nueva con el padre
                bestBase.prevBase = curBase;
                
                return true;
            }
        }
        else
        {
            // No existen más bases por elegir, el camino
            // a la meta se encuentra bloqueado.
            return false;
        }

    }

    /// <summary>
    /// Determina si la posición pasada como parámetro
    /// se encuentra en la lista de bases cerradas.
    /// </summary>
    /// <param name="pnt"></param>
    /// <returns></returns>
    private bool IsInClosedBases(Vector3 pnt)
    {
        AStarBase p = _closedBases.GetFirst();
        
        while (p != null)
        {
            if (p.pos == pnt)
                return true;

            p = _closedBases.GetNext();
        }

        return false;
    }

    /// <summary>
    /// Retorna la base abierta en función de la posición pasada
    /// como parámetro (si la misma existe, sino retorna null)
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private AStarBase GetOpenBase(Vector3 pos)
    {
        for (int i = 0; i < _openBases.Count; i++)
        {
        	AStarBase asb = (AStarBase) _openBases[i];
            if (asb.pos == pos)
                return asb;
        }

        return null;
    }

    /// <summary>
    /// Imprime información de debug en el stdout
    /// </summary>
    /// <param name="b"></param>
    private void DebugShowSolution(AStarBase b)
    {
        AStarBase p = b;
        
        while (p != null)
        {
            Debug.Log(String.Format("x: {0} y: {1}", p.pos.x, p.pos.y));
            p = p.prevBase;
        }

        Debug.Log(String.Format("Open Bases: {0}", _openBases.Count));
        Debug.Log(String.Format("Closed Bases: {0}", _closedBases.Count));
    }

    /// <summary>
    /// Selecciona la base de menor costo (total + estimado)
    /// </summary>
    /// <returns></returns>
    private AStarBase SelectBestMove()
    {
        float minCost = float.MaxValue;
        AStarBase selectedBase = null;
        for (int i = _openBases.Count - 1; i >= 0; i--)
        {
        	AStarBase asb = (AStarBase) _openBases[i];
            if (asb.cost < minCost)
            {
                minCost = asb.cost;
                selectedBase = asb;
            }
        }

        return selectedBase;
    }

    /// <summary>
    /// Arma la lista solución hacia adelante
    /// </summary>
    /// <param name="finalBase"></param>
    private void BuildNextBaseReferences(AStarBase finalBase)
    {
		_solutionLen = 1;
        AStarBase curBase = finalBase;
        AStarBase prevBase = curBase.prevBase;

        while (prevBase != null)
        {
            prevBase.nextBase = curBase;

            curBase = prevBase;
            prevBase = curBase.prevBase;
			
			_solutionLen++;
       }

        _solutionBase = curBase;
		
    }

    /// <summary>
    /// Returns the first position of the solution
    /// </summary>
    /// <param name="pnt"></param>
    /// <returns></returns>
    public bool GetFirstSolutionPos(out Vector3 pnt)
    {
        if (_solutionBase == null)
        {
            pnt = Vector3.zero;
            return false;
        }

        _cursorBase = _solutionBase;
        return GetNextSolutionPos(out pnt);
    }

    /// <summary>
    /// Returns the next solution position
    /// </summary>
    /// <param name="pnt"></param>
    /// <returns></returns>
    public bool GetNextSolutionPos(out Vector3 pnt)
    {
        if (_cursorBase == null)
        {
            pnt = Vector3.zero;
            return false;
        }

        pnt = _cursorBase.pos;
        _cursorBase = _cursorBase.nextBase;

        return true;
    }
	
	/// <summary>
	/// Get the number of points to the solution 
	/// </summary>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public int GetSolutionStepCount()
	{
		return _solutionLen;
	}

	/// <summary>
	/// Return the path to goal 
	/// </summary>
    public Vector3[] GetSolution()
    {
        Vector3[] sol = new Vector3[GetSolutionStepCount()];

        int i = 0;
        Vector3 pos;

        if (GetFirstSolutionPos(out pos))
        {
            sol[i++] = pos;

            while (GetNextSolutionPos(out pos))
                sol[i++] = pos;
        }

        return sol;
    }

    /// <summary>
    /// Returns the Fn
    /// </summary>
    /// <param name="p1">First point</param>
    /// <param name="p2">Second point</param>
    /// <returns></returns>
    public float CalculateFn(Vector3 p1, Vector3 p2)
    {
        if (_possibleMovesCount == NOT_USING_DIAGONALS)
            return Mathf.Abs(p2.x - p1.x) + Mathf.Abs(p2.y - p1.y);
        else
            return Mathf.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));
    }

    /// <summary>
    /// Can go in diagonals
    /// </summary>
    public bool MovingInDiagonals
    {
        get { return (_possibleMovesCount == USING_DIAGONALS); }
        set { _possibleMovesCount = (value ? USING_DIAGONALS : NOT_USING_DIAGONALS); }
    }

    
}

