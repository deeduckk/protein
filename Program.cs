using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab1
{
    public struct GeneticData
    {
        public static List<char> letters = new List<char>() { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };

        public string name;
        public string organism;
        public string formula;

        static string Encode(string formula)

        {
            string encoded = string.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                char ch = formula[i];
                int count = 1;
                while (i < formula.Length - 1 && formula[i + 1] == ch)
                {
                    count++;
                    i++;
                }
                if (count > 2) encoded = encoded + count + ch;
                if (count == 1) encoded = encoded + ch;
                if (count == 2) encoded = encoded + ch + ch;

            }
            return encoded;
        }
        public static string Decode(string formula)
        {
            string decoded = string.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    for (int j = 0; j < conversion - 1; j++) decoded = decoded + letter;
                }
                else decoded = decoded + formula[i];
            }
            return decoded;
        }
        public bool IsValid()
        {
            foreach (char ch in Decode(formula))
            {
                if (!letters.Contains(ch)) return false;
            }

            return true;
        }
    }
    class Program
    {
        static List<char> letters = new List<char>() { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };
        static List<GeneticData> data = FileParser.ReadGeneticData();

        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return "";
        }

        static List<int> Search(string amino_acid)
        {
            List<int> list = new List<int>();
            string decoded = GeneticData.Decode(amino_acid);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        static int Diff(string name1, string name2)
        {
            int size, counter = 0;
            string formula1 = GetFormula(name1), formula2 = GetFormula(name2);
            if (formula1 == "" && formula2 == "")
            {
                return -3;
            }
            else if (formula2 == "")
            {
                return -2;
            }
            else if (formula1 == "")
            {
                return -1;
            }
            if (formula1.Length > formula2.Length)
            {
                size = formula2.Length;
                counter += formula1.Length - formula2.Length;
            }
            else
            {
                size = formula1.Length;
                counter += formula2.Length - formula1.Length;
            }
            for (int i = 0; i < size; i++)
            {
                if (formula1[i] != formula2[i]) counter++;
            }
            return counter;
        }
        static List<int> numberOfAcids = new List<int>(new int[20]);
        static int Mode(string name)
        {
            string formula = GetFormula(name);
            int number = 0, index = 0;
            if (formula == "") return -1;
            for (int i = 0; i < 20; i++)
            {
                numberOfAcids[i] = 0;
            }
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < formula.Length; j++)
                {
                    if (formula[j] == letters[i]) numberOfAcids[i]++;
                }
                if (numberOfAcids[i] > number) number = numberOfAcids[i];
            }
            for (int i = 0; i < 20; i++)
            {
                if (numberOfAcids[i] == number)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static void ReadHandleCommands()
        {
            StreamReader reader = new StreamReader(FileParser.CommandsDataFile);
            int counter = 0;

            StreamWriter writer = new StreamWriter(FileParser.OutputDataFile);
            Console.WriteLine("Daria Dehterevich\nGenetic Searching");
            writer.WriteLine("Daria Dehterevich\nGenetic Searching");

            while (!reader.EndOfStream)
            {
                Console.WriteLine("========================================================================");
                writer.WriteLine("========================================================================");
                string line = reader.ReadLine(); counter++;
                string[] command = line.Split('\t');
                if (command[0].Equals("search"))
                {
                    Console.WriteLine($"{counter.ToString("D3")}   {"search"}   {GeneticData.Decode(command[1])}");
                    writer.WriteLine(counter.ToString("D3") + "\t" + command[0] + "\t" + GeneticData.Decode(command[1]));
                    List<int> index = Search(command[1]);
                    Console.WriteLine("organism\t\t\t\tprotein");
                    writer.WriteLine("organism\t\t\t\tprotein");
                    if (index.Count > 0)
                    {
                        for (int i = 0; i < index.Count; i++)
                        {
                            Console.WriteLine($"{data[index[i]].organism}    {data[index[i]].name}");
                            writer.WriteLine(data[index[i]].organism + "\t\t" + data[index[i]].name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("NOT FOUND");
                        writer.WriteLine("NOT FOUND");
                    }
                }
                if (command[0].Equals("diff"))
                {
                    Console.WriteLine($"{counter.ToString("D3")}   {"diff"}   {command[1]}   {command[2]}");
                    writer.WriteLine(counter.ToString("D3") + "\t" + command[0] + "\t" + command[1] + "\t" + command[2]);
                    Console.WriteLine("amino-acids difference: ");
                    writer.WriteLine("amino-acids difference: ");
                    if (Diff(command[1], command[2]) >= 0)
                    {
                        Console.WriteLine(Diff(command[1], command[2]));
                        writer.WriteLine(Diff(command[1], command[2]));
                    }
                    else if (Diff(command[1], command[2]) == -1)
                    {
                        Console.WriteLine("MISSING: " + command[1]);
                        writer.WriteLine("MISSING: " + command[1]);
                    }
                    else if (Diff(command[1], command[2]) == -2)
                    {
                        Console.WriteLine("MISSING: " + command[2]);
                        writer.WriteLine("MISSING: " + command[2]);
                    }
                    else
                    {
                        Console.WriteLine("MISSING: " + command[1] + "\t" + command[2]);
                        writer.WriteLine("MISSING: " + command[1] + "\t" + command[2]);
                    }
                }
                if (command[0].Equals("mode"))
                {
                    Console.WriteLine($"{counter.ToString("D3")}   {"mode"}   {command[1]}");
                    writer.WriteLine(counter.ToString("D3") + "\t" + command[0] + "\t" + command[1]);
                    Console.WriteLine("amino-acid occurs: ");
                    writer.WriteLine("amino-acid occurs: ");
                    if (Mode(command[1]) == -1)
                    {
                        Console.WriteLine("MISSING: " + command[1]);
                        writer.WriteLine("MISSING: " + command[1]);
                    }
                    else
                    {
                        Console.WriteLine(letters[Mode(command[1])] + "\t\t" + numberOfAcids[Mode(command[1])]);
                        writer.WriteLine(letters[Mode(command[1])] + "\t\t" + numberOfAcids[Mode(command[1])]);
                    }
                }
            }
            writer.WriteLine("========================================================================");
            reader.Close();
            writer.Close();
        }

        static void Main()
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].IsValid() == false)
                {
                    Console.WriteLine("ERROR. Wrong formula of the " + (i + 1) + "-st protein");
                    return;
                }
            }
            ReadHandleCommands();
        }
    }
}