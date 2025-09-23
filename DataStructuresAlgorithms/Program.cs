namespace DataStructuresAlgorithms
{
    internal class Program
    {
        private static void printArray(int[] arr)
        {
            Console.Write(string.Join(", ", arr));
            Console.WriteLine();
        }

        private static int[] removeEvens(int[] arr)
        {
            int count = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] % 2 == 0) count++;
            }

            int[] result = new int[count];
            int resultIndex = 0;
            for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i] % 2 == 0)
                {
                    result[resultIndex++] = arr[i];
                }
            }
            return result;
        }

        private static int[] reverseArray(int[] arr, int start, int end)
        {
            while (start < end)
            {
                int enda = arr[end];
                arr[end] = arr[start];
                arr[start] = enda;
                start++;
                end--;
            }
            return arr;
        }

        private static int minimumArrayValue(int[] arr)
        {
            int minimum = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < minimum) minimum = arr[i];
            }
            return minimum;
        }

        private static int secondMinimumArrayValue(int[] arr)
        {
            int fminimum = arr[0];
            int sminimum = arr[1];
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < fminimum)
                {
                    sminimum = fminimum;
                    fminimum = arr[i];
                }
                else if (arr[i] < sminimum) sminimum = arr[i];
            }
            return sminimum;
        }

        private static int secondMaximumArrayValue(int[] arr)
        {
            int fm = arr[0];
            int sm = arr[1];
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] > fm)
                {
                    sm = fm;
                    fm = arr[i];
                }
                else if (arr[i] > sm) sm = arr[i];
            }

            return sm;
        }

        private static int[] moveZerosToEnd(int[] arr)
        {
            int lastNonZeroIndex = 1;
            int rindex = 2;
            while (rindex <= arr.Length)
            {
                if (arr[^lastNonZeroIndex] == 0)
                {
                    lastNonZeroIndex++;
                    rindex++;
                    continue;
                }
                if (arr[^rindex] == 0)
                {
                    arr[^rindex] = arr[^lastNonZeroIndex];
                    arr[^lastNonZeroIndex] = 0;
                    lastNonZeroIndex++;
                }
                rindex++;
            }
            return arr;
        }

        private static int[] moveZerosToEndB(int[] arr)
        {
            int[] result = new int[arr.Length];
            int idx = 0;
            int nridx = 1;
            int nidx = 0;
            while ((nidx + nridx) <= arr.Length)
            {
                if (arr[idx] != 0)
                {
                    result[nidx] = arr[idx];
                    nidx++;
                }
                else
                {
                    result[^nridx] = 0;
                    nridx++;
                }
                idx++;
            }
            return result;
        }

        private static int[] moveZerosToEndC(int[] arr)
        {
            int pzero = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 0)
                {
                    if (pzero == -1) pzero = i;
                }
                else
                {
                    if (pzero != -1)
                    {
                        arr[pzero] = arr[i];
                        pzero++;
                        arr[i] = 0;
                    }
                }
            }
            return arr;
        }

        private static int findMissingNumber(int[] arr)
        {
            int sumArr = 0;
            int sumTotal = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                sumArr += arr[i];
                sumTotal += i + 1;
            }
            sumTotal += arr.Length + 1;

            return sumTotal - sumArr;
        }

        private static bool isPalindrome(string s)
        {
            bool result = true;
            int index = 0;

            do
            {
                result = s[index] == s[^(index + 1)];
                index++;
            }
            while (result && index < s.Length / 2);

            return result;
        }

        private static int singlyListLength(SinglyLinkedList list)
        {
            int length = 0;

            var current = list.head;
            while (current != null)
            {
                length++;
                current = current.next;
            }

            return length;
        }

        private static void insertNodeBegin(SinglyLinkedList list, ListNode node)
        {
            if (list.head != null)
            {
                node.next = list.head;
            }
            list.head = node;
        }

        private static void insertValueEndList(SinglyLinkedList list, int value)
        {
            var newNode = new ListNode(value);
            var current = list.head;
            while (current != null && current.next != null)
            {
                current = current.next;
            }
            if (current == null) list.head = newNode;
            else current.next = newNode;
        }

        public static void insertValueGivenPosition(SinglyLinkedList list, int value, int position)
        {
            var newNode = new ListNode(value);
            if (list.head == null)
            {
                list.head = newNode;
                return;
            }
            if (position <= 1)
            {
                newNode.next = list.head!;
                list.head = newNode;
                return;
            }

            var current = list.head;
            int index = 2;
            while (current.next != null && index < position)
            {
                current = current.next;
                index++;
            }
            if (current.next != null)
            {
                newNode.next = current.next;
            }
            current.next = newNode;
        }

        private static ListNode deleteFirstNode(SinglyLinkedList list)
        {
            if (list.head == null) return null!;
            var temp = list.head;
            list.head = temp.next;
            temp.next = null!;
            return temp;
        }

        private static ListNode deleteLastNode(SinglyLinkedList list)
        {
            if (list.head == null) return null!;
            ListNode p1 = list.head;
            ListNode p2 = null!;
            while (p1.next != null)
            {
                p2 = p1;
                p1 = p1.next;
            }
            if (p2 == null)
            {
                list.head = null;
                return p1;
            }
            p2.next = null!;
            return p1;
        }

        private static ListNode deleteAtPosition(SinglyLinkedList list, int position)
        {
            // empty list or position less then 1
            if (list.head == null || position < 1) return null!;

            // position one
            if (position == 1)
            {
                ListNode ret = list.head;
                list.head = ret.next;
                ret.next = null!;
                return ret;
            }

            ListNode pointer = list.head;
            ListNode previous = null!;
            int count = 1;

            while (pointer!.next != null && count < position)
            {
                previous = pointer;
                pointer = pointer.next!;
                count++;
            }

            // position grather then the list size
            if (count < position) return null!;

            previous.next = pointer.next!;
            pointer.next = null!;

            return pointer;
        }

        private static int searchElementValue(SinglyLinkedList list, int data)
        {
            int index = 0;
            if (list.head == null) return index;
            ListNode node = list.head!;
            while (node != null)
            {
                index++;
                if (node.data == data) return index;
                node = node.next;
            }
            return 0;
        }

        // use for when you have to run over all elements without conditions
        // user while when you have to run based on one or more conditions
        // if you have to move things to the end, start at the end, and so on
        // look for what you need in the begining... if you are dealing with two array, you need at least two indexes

        // parei no video 57 da playlist do indiado

        static void Main(string[] args)
        {
            var list = getSinglyList();
            printSinglyLinkedList(list);
            var index = searchElementValue(list, 12);

            Console.WriteLine(index);


            // int[] arr = { 0, 0, 11, -2, -3, 1, 0, 2, 3, 4, 0, 5, 6, 7, -4, 0, 0 };
            // int[] arr = { 3,1,2,4,5 };
            // printArray(arr);
            // var result = findMissingNumber(arr);
            // var result = isPalindrome("shannahs");
            // Console.WriteLine(result);
            // printArray(result);
        }

        private static void printSinglyLinkedList(SinglyLinkedList list)
        {
            var current = list.head;
            while (current != null)
            {
                Console.WriteLine(current.data);
                current = current.next;
            }
            Console.WriteLine();
        }

        static SinglyLinkedList getSinglyList()
        {
            SinglyLinkedList list = new SinglyLinkedList();
            list.head = new ListNode(10);
            var second = new ListNode(1);
            var third = new ListNode(8);
            var fourth = new ListNode(11);

            list.head.next = second;
            second.next = third;
            third.next = fourth;

            return list;
        }
    }

    internal class ListNode
    {
        public ListNode(int data)
        {
            this.data = data;
            this.next = null!;
        }

        public int data { get; set; }
        public ListNode next { get; set; }
    }

    internal class SinglyLinkedList
    {
        public ListNode? head { get; set; }
    }
}
