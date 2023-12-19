using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Minecraft3DS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Contains(".blang"))
            {
                Extract(args[0]);
            }
            else
            {
                Replace(args[1], args[0]);
            }
        }
        public static void Extract(string game)
        {
            var reader = new BinaryReader(File.OpenRead(game));
            int stringCount = reader.ReadInt32();
            int[] pointers = new int[stringCount];
            string[] strings = new string[stringCount];
            for (int i = 0; i < stringCount; i++)
            {
                reader.BaseStream.Position += 4;
                pointers[i] = reader.ReadInt32();
            }
            int fileEnd = reader.ReadInt32() + 4;
            int point1 = (int)reader.BaseStream.Position;
            for (int i = 0; i < stringCount; i++)
            {
                reader.BaseStream.Position = point1;
                reader.BaseStream.Position += pointers[i];
                strings[i] = Utils.ReadString(reader, Encoding.UTF8);
            }
            File.WriteAllLines(game.Replace(".blang",".txt"), strings);
        }
        public static void Replace(string game, string txt)
        {
            var reader = new BinaryReader(File.OpenRead(game));
            string[] strings = File.ReadAllLines(txt);
            int stringBeg = (reader.ReadInt16() * 8) + 8;
            int[] newPointers = new int[strings.Length];
            reader.Close();
            var writer = new BinaryWriter(File.OpenWrite(game));
            writer.Write(strings.Length);
            writer.BaseStream.Position = stringBeg;
            for (int i = 0; i < strings.Length; i++)
            {
                newPointers[i] = (int)writer.BaseStream.Position - stringBeg;
                writer.Write(Encoding.UTF8.GetBytes(strings[i]));
                writer.Write(new byte());
            }
            writer.BaseStream.Position = 4;
            for (int i = 0; i < strings.Length; i++)
            {
                writer.BaseStream.Position += 4;
                writer.Write(newPointers[i]);
            }
        }
    }
}
