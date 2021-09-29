using System;
using System.Collections.Generic;
using System.IO;

namespace SaberTestTask
{
    class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }

    class ListRandom
    {
        private const char DataSeparator = ':';

        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            //Сохранение ссылок на рандомный элемент через индекс, конечно, добавляет лишний лист,
            //но не дату же их сохранять, в самом деле
            var nodesList = new List<ListNode>();
            var current = Head;

			while (current != null) {
                nodesList.Add(current);
                current = current.Next;
            }
			using (var sw = new StreamWriter(s)) {
				for (int i = 0; i < nodesList.Count; i++) {
                    var data = nodesList[i].Data;
                    var indexOfRandomNode = nodesList.IndexOf(nodesList[i].Random);
                    sw.WriteLine($"{data}{DataSeparator}{indexOfRandomNode}");
				}
			}
        }

        public void Deserialize(FileStream s)
        {
            var current = new ListNode();
            Head = current;
            var nodesList = new List<ListNode>();
            using (StreamReader sr = new StreamReader(s)) {
                while (true) {
                    var line = sr.ReadLine();
                    if (!line.IsNullOrEmpty()) {
                        Count++;
                        current.Data = line;
                        var next = new ListNode {
                            Previous = current
                        };
                        current.Next = next;
                        nodesList.Add(current);
                        current = next;
                    } else {
                        break;
                    }
                }
            }
            Tail = current.Previous;
            Tail.Next = null;
			for (int i = 0; i < nodesList.Count; i++) {
                var node = nodesList[i];
                var separatedData = node.Data.Split(DataSeparator);
                var indexOfRandom = int.Parse(separatedData[1]);
                node.Data = separatedData[0];
                node.Random = nodesList[indexOfRandom];
			}
        }

        public bool Equals(ListRandom other)
		{
			if (Count != other.Count) {
                return false;
			}
            var thisNode = Head;
            var otherNode = other.Head;
			for (int i = 0; i < Count; i++) {
				if (!thisNode.Data.Equals(otherNode.Data)) {
                    return false;
				}
                thisNode = thisNode.Next;
                otherNode = otherNode.Next;
			}
            return true;
		}
    }

    class Program
	{
        private const string FileName = "File.imao";
        private static string GenerateData() => new Random().Next().ToString();

        static void Main(string[] args)
        {
            var count = new Random().Next(5, 50);
            var head = new ListNode {
                Data = GenerateData()
            };
            var tail = head;
            //голова уже есть, так что надо на один меньше
			for (int i = 0; i < count - 1; i++) {
                var nextTail = new ListNode {
                    Previous = tail,
                    Data = GenerateData()
                };
                tail.Next = nextTail;
                tail = nextTail;
            }
            var current = head;
            for (int i = 0; i < count; i++) {
                current.Random = GetRandomNode(head, count);
                current = current.Next;
            }
            var initialList = new ListRandom {
                Head = head,
                Tail = tail,
                Count = count
            };
            var deserializedList = new ListRandom();

			try {
                Console.WriteLine("Поехали!");
                using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write)) {
                    initialList.Serialize(fs);
                }
                Console.WriteLine("Успешно сериализовали!");
                using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read)) {
                    deserializedList.Deserialize(fs);
                }
                Console.WriteLine("Успешно десериализовали!");
                if (deserializedList.Equals(initialList)) {
                    Console.WriteLine("И результат верен. Ура, товарищи!");
                }
            } catch (Exception e) {
                Console.WriteLine("Что-то пошло не так:");
                Console.Write(e.Message);
            }
            Console.ReadLine();
        }

        //Добавил в мейне массив, сугубо для рандома, но он чет прям совсем лишним смотрится, поэтому вот так
        private static ListNode GetRandomNode(ListNode head, int count)
		{
            var result = head;
            var targetIndex = new Random().Next(count);
            var currentIndex = 0;
			while (currentIndex != targetIndex) {
                result = result.Next;
                currentIndex++;
			}
            return result;
		}
    }

    static class StringExtensions
	{
        //потому что могу)
        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s == string.Empty;
        }
    }
}
