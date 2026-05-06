namespace ServiciosTecnicos.DataStructures
{
    /// <summary>
    /// Lista enlazada simple implementada desde cero
    /// TAD: Tipo Abstracto de Datos para almacenar elementos de forma secuencial
    /// </summary>
    /// <typeparam name="T">Tipo de dato a almacenar</typeparam>
    public class CustomLinkedList<T>
    {
        private Node<T>? head;
        private int count;

        public int Count => count;

        public CustomLinkedList()
        {
            head = null;
            count = 0;
        }

        /// <summary>
        /// Agrega un elemento al final de la lista - O(n)
        /// </summary>
        public void Add(T data)
        {
            Node<T> newNode = new Node<T>(data);

            if (head == null)
            {
                head = newNode;
            }
            else
            {
                Node<T> current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
            count++;
        }

        /// <summary>
        /// Agrega un elemento al inicio de la lista - O(1)
        /// </summary>
        public void AddFirst(T data)
        {
            Node<T> newNode = new Node<T>(data);
            newNode.Next = head;
            head = newNode;
            count++;
        }

        /// <summary>
        /// Elimina un elemento de la lista - O(n)
        /// </summary>
        public bool Remove(T data)
        {
            if (head == null) return false;

            if (head.Data?.Equals(data) == true)
            {
                head = head.Next;
                count--;
                return true;
            }

            Node<T> current = head;
            while (current.Next != null)
            {
                if (current.Next.Data?.Equals(data) == true)
                {
                    current.Next = current.Next.Next;
                    count--;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        /// <summary>
        /// Busca un elemento en la lista - O(n)
        /// </summary>
        public bool Contains(T data)
        {
            Node<T>? current = head;
            while (current != null)
            {
                if (current.Data?.Equals(data) == true)
                    return true;
                current = current.Next;
            }
            return false;
        }

        /// <summary>
        /// Obtiene el elemento en el índice especificado - O(n)
        /// </summary>
        public T? GetAt(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            Node<T>? current = head;
            for (int i = 0; i < index; i++)
            {
                current = current?.Next;
            }
            return current != null ? current.Data : default(T);
        }

        /// <summary>
        /// Convierte la lista a un array - O(n)
        /// </summary>
        public T[] ToArray()
        {
            T[] array = new T[count];
            Node<T>? current = head;
            int index = 0;

            while (current != null)
            {
                array[index++] = current.Data;
                current = current.Next;
            }
            return array;
        }

        /// <summary>
        /// Limpia toda la lista - O(1)
        /// </summary>
        public void Clear()
        {
            head = null;
            count = 0;
        }
    }
}
