namespace ServiciosTecnicos.DataStructures
{
    /// <summary>
    /// Nodo genérico para estructuras de datos enlazadas
    /// </summary>
    /// <typeparam name="T">Tipo de dato almacenado en el nodo</typeparam>
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T>? Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }
}
