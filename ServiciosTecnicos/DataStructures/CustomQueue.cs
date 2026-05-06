namespace ServiciosTecnicos.DataStructures
{
    /// <summary>
    /// Cola (Queue) implementada desde cero usando lista enlazada
    /// TAD: FIFO (First In, First Out)
    /// </summary>
    /// <typeparam name="T">Tipo de dato a almacenar</typeparam>
    public class CustomQueue<T>
    {
        private Node<T>? front;  // Frente de la cola
        private Node<T>? rear;   // Final de la cola
        private int count;

        public int Count => count;
        public bool IsEmpty => count == 0;

        public CustomQueue()
        {
            front = null;
            rear = null;
            count = 0;
        }

        /// <summary>
        /// Agrega un elemento al final de la cola - O(1)
        /// </summary>
        public void Enqueue(T data)
        {
            Node<T> newNode = new Node<T>(data);

            if (rear == null)
            {
                front = rear = newNode;
            }
            else
            {
                rear.Next = newNode;
                rear = newNode;
            }
            count++;
        }

        /// <summary>
        /// Elimina y retorna el elemento al frente de la cola - O(1)
        /// </summary>
        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("La cola está vacía");

            T data = front!.Data;
            front = front.Next;

            if (front == null)
                rear = null;

            count--;
            return data;
        }

        /// <summary>
        /// Retorna el elemento al frente sin eliminarlo - O(1)
        /// </summary>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("La cola está vacía");

            return front!.Data;
        }

        /// <summary>
        /// Limpia toda la cola - O(1)
        /// </summary>
        public void Clear()
        {
            front = null;
            rear = null;
            count = 0;
        }

        /// <summary>
        /// Convierte la cola a un array - O(n)
        /// </summary>
        public T[] ToArray()
        {
            T[] array = new T[count];
            Node<T>? current = front;
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
