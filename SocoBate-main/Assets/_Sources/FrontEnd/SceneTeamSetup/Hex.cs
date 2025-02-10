using UnityEngine;

public class Hex : MonoBehaviour
{
    public GameObject currentUnit = null;  // A referência ao sprite da unidade (se houver uma sobre o hex)

    // Função para verificar se o hexágono está ocupado
    public bool IsOccupied()
    {
        return currentUnit != null;  // Se o sprite da unidade estiver presente, o hex está ocupado
    }

    // Função para colocar a unidade no hexágono
    public void PlaceUnit(GameObject unit)
    {
        currentUnit = unit;  // Coloca a unidade no hex e marca o hex como ocupado
    }

    // Função para remover a unidade
    public void RemoveUnit()
    {
        currentUnit = null;  // Remove a unidade e marca o hex como desocupado
    }
}
