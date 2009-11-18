// Mono Framework
using System;

/// <summary>
/// ArrayStack_AStarBase
/// Pila basada en un array de elementos que mantiene referencias a objetos externos
/// no creados por esta clase.
/// 
/// No existe asignación ni desasignación de memoria durante las inserciones y
/// las extracciones.
/// </summary>
public class ArrayStack_AStarBase
{
	public ArrayStack_AStarBase(int max)
	{
        _list = new AStarBase[max];
		_lastElement = 0;
	}
	
	/// <summary>
	/// Inserta elementos en la pila
	/// </summary>
	/// <param name="p"></param>
	/// <returns></returns>
	public bool Insert(AStarBase p)
	{
		// ¿La pila está llena?
		if (_lastElement == _list.Length)
			return false;

		// Fijo el valor de un elemento en el último elemento
		_list[_lastElement] = p;

		// Incremento el contador que indica el último elemento
		_lastElement++;

		return true;
	}

	/// <summary>
	/// Extrae elementos de la pila
	/// </summary>
	/// <param name="p"></param>
	/// <returns></returns>
	public bool Extract(out AStarBase p)
	{
		// ¿La pila está vacía?
		if (_lastElement == 0)
		{
            p = null;
			return false;
		}

		_lastElement--;
		p = _list[_lastElement];
		return true;
	}

    /// <summary>
    /// Retorna primer elemento
    /// </summary>
    /// <returns></returns>
	public AStarBase GetFirst()
	{
		// ¿La pila está vacía?
		if (_lastElement == 0)
            return null;

		_cursor = 0;
		return _list[_cursor];
	}

    /// <summary>
    /// Retorna siguiente elemento
    /// </summary>
    /// <returns></returns>
	public AStarBase GetNext()
	{
		_cursor++;

		// ¿No hay mas datos que leer?
		if (_cursor >= _lastElement)
            return null;

		return _list[_cursor];
	}

    public int Count
    {
        get { return _lastElement; }
    }

    public void Clear()
    {
        _lastElement = 0;
    }


	// --------------------------------------------------------
    // Variables
	private AStarBase[] _list = null;        // Lista de elementos
    private int _lastElement;        // Índice de la última partícula
    private int _cursor;             // Cursor utilizado para métodos GetFirst y GetNext
}

