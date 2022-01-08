﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quixo
{
    internal class Consts
    {
        public static  int PieceSize = 400 / 5;
        public static int BoardSize = 5;

        private static long[] xWon = {
               //<unused><------x--------------->unused><------y----------------->
                0b0000000111110000000000000000000000000000000000000000000000000000,
                0b0000000000001111100000000000000000000000000000000000000000000000,
                0b0000000000000000011111000000000000000000000000000000000000000000,
                0b0000000000000000000000111110000000000000000000000000000000000000,
                0b0000000000000000000000000001111100000000000000000000000000000000,
                0b0000000100001000010000100001000000000000000000000000000000000000,
                0b0000000010000100001000010000100000000000000000000000000000000000,
                0b0000000001000010000100001000010000000000000000000000000000000000,
                0b0000000000100001000010000100001000000000000000000000000000000000,
                0b0000000100000100000100000100000100000000000000000000000000000000,
                0b0000000000010001000100010001000000000000000000000000000000000000,};

        public static long[] yWon = Array.ConvertAll(XWon, x => x >> 32);

        public static long[] XWon { get => xWon; set => xWon = value; }
    }
}
