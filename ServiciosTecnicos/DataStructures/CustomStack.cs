namespace ServiciosTecnicos.DataStructures
{
    /// <summary>
    /// Pila (Stack) implementada desde cero usando lista enlazada
    /// TAD: LIFO (Last In, First Out)
    /// </summary>
    /// <typeparam name="T">Tipo de dato a almacenar</typeparam>
    public class CustomStack<T>
    {
        private Node<T>? top;
        private int count;

        public int Count => count;
        public bool IsEmpty => count == 0;

        public CustomStack()
        {
            top = null;
            count = 0;
        }

        /// <summary>
        /// Agrega un elemento al tope de la pila - O(1)
        /// </summary>
        public void Push(T data)
        {
            Node<T> newNode = new Node<T>(data);
            newNode.Next = top;
            top = newNode;
            count++;
        }

        /// <summary>
        /// Elimina y retorna el elemento del tope - O(1)
        /// </summary>
        public T Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("La pila está vacía");

            T data = top!.Data;
            top = top.Next;
            count--;
            return data;
        }

        /// <summary>
        /// Retorna el elemento del tope sin eliminarlo - O(1)
        /// </summary>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("La pila está vacía");

            return top!.Data;
        }

        /// <summary>
        /// Limpia toda la pila - O(1)
        /// </summary>
        public void Clear()
        {
            top = null;
            count = 0;
        }

        /// <summary>
        /// Convierte la pila a un array - O(n)
        /// </summary>
        public T[] ToArray()
        {
            T[] array = new T[count];
            Node<T>? current = top;
            int index = 0;

            while (current != null)
            {
                array[index++] = current.Data;
                current = current.Next;
            }
            return array;
        }
    }
}
