﻿using StellaQL;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main シーン
/// </summary>
namespace SceneStellaQLTest
{
    /// <summary>
    /// アニメーターのステート
    /// </summary>
    public class StateExRecord : AbstractStateExRecord
    {
        public static StateExRecord Build(string fullpath, StateExTable.Attr attribute)
        {
            return new StateExRecord(fullpath, Animator.StringToHash(fullpath), attribute);
        }
        public StateExRecord(string fullpath, int fullpathHash, StateExTable.Attr attribute) : base(fullpath, fullpathHash, (int)attribute)
        {
        }

        public override bool HasFlag_attr(int enumration)
        {
            return ((StateExTable.Attr)this.AttributeEnum).HasFlag((StateExTable.Attr)enumration);
        }
    }

    public class StateExTable : AbstractStateExTable
    {
        static StateExTable()
        {
            StateExTable.Instance = new StateExTable();
        }
        public static StateExTable Instance;
        public override Type GetAttributeEnumration() { return typeof(StateExTable.Attr); }

        /// <summary>
        /// 列挙型は拡張できないし、どうしたものか。
        /// </summary>
        [Flags]
        public enum Attr
        {
            Zero = 0,
            Alpha = 1,
            Beta = Alpha << 1,
            Cee = Beta << 1,
            Dee = Cee << 1,
            Eee = Dee << 1,
        }

        public const string FULLPATH_ALPACA = "Base Layer.Alpaca";
        public const string FULLPATH_BEAR = "Base Layer.Bear";
        public const string FULLPATH_CAT = "Base Layer.Cat";
        public const string FULLPATH_DOG = "Base Layer.Dog";
        public const string FULLPATH_ELEPHANT = "Base Layer.Elephant";
        public const string FULLPATH_FOX = "Base Layer.Fox";
        public const string FULLPATH_GIRAFFE = "Base Layer.Giraffe";
        public const string FULLPATH_HORSE = "Base Layer.Horse";
        public const string FULLPATH_IGUANA = "Base Layer.Iguana";
        public const string FULLPATH_JELLYFISH = "Base Layer.Jellyfish";
        public const string FULLPATH_KANGAROO = "Base Layer.Kangaroo";
        public const string FULLPATH_LION = "Base Layer.Lion";
        public const string FULLPATH_MONKEY = "Base Layer.Monkey";
        public const string FULLPATH_NUTRIA = "Base Layer.Nutria";
        public const string FULLPATH_OX = "Base Layer.Ox";
        public const string FULLPATH_PIG = "Base Layer.Pig";
        public const string FULLPATH_QUETZAL = "Base Layer.Quetzal";
        public const string FULLPATH_RABBIT = "Base Layer.Rabbit";
        public const string FULLPATH_SHEEP = "Base Layer.Sheep";
        public const string FULLPATH_TIGER = "Base Layer.Tiger";
        public const string FULLPATH_UNICORN = "Base Layer.Unicorn";
        public const string FULLPATH_VIXEN = "Base Layer.Vixen";
        public const string FULLPATH_WOLF = "Base Layer.Wolf";
        public const string FULLPATH_XENOPUS = "Base Layer.Xenopus";
        public const string FULLPATH_YAK = "Base Layer.Yak";
        public const string FULLPATH_ZEBRA = "Base Layer.Zebra";

        protected StateExTable()
        {
            List<StateExRecordable> temp = new List<StateExRecordable>()
            {
                StateExRecord.Build(  FULLPATH_ALPACA, Attr.Alpha | Attr.Cee),// {E}(1) AC(1) ([(A C)(B)]{E})(1)
                StateExRecord.Build(  FULLPATH_BEAR, Attr.Alpha | Attr.Beta | Attr.Eee),// B(1) AE(1) AE,B,E(1)
                StateExRecord.Build(  FULLPATH_CAT, Attr.Alpha | Attr.Cee),// {E}(2) AC(2) ([(A C)(B)]{E})(2)
                StateExRecord.Build(  FULLPATH_DOG, Attr.Dee),// {E}(3)
                StateExRecord.Build(  FULLPATH_ELEPHANT, Attr.Alpha | Attr.Eee),//AE(2) AE,B,E(2) Nn(1)
                StateExRecord.Build(  FULLPATH_FOX, Attr.Zero),// {E}(4)
                StateExRecord.Build(  FULLPATH_GIRAFFE, Attr.Alpha | Attr.Eee),//AE(3) AE,B,E(3)
                StateExRecord.Build(  FULLPATH_HORSE, Attr.Eee),// AE,B,E(4)
                StateExRecord.Build(  FULLPATH_IGUANA, Attr.Alpha),// {E}(5) Nn(2)
                StateExRecord.Build(  FULLPATH_JELLYFISH, Attr.Eee),// AE,B,E(5)
                StateExRecord.Build(  FULLPATH_KANGAROO, Attr.Alpha),// {E}(6) Nn(3)
                StateExRecord.Build(  FULLPATH_LION, Attr.Zero),// {E}(7) Nn(4)
                StateExRecord.Build(  FULLPATH_MONKEY, Attr.Eee),// AE,B,E(6) Nn(5)
                StateExRecord.Build(  FULLPATH_NUTRIA, Attr.Alpha),// {E}(8) Nn(6)
                StateExRecord.Build(  FULLPATH_OX, Attr.Zero),// {E}(9)
                StateExRecord.Build(  FULLPATH_PIG, Attr.Zero),// {E}(10)
                StateExRecord.Build(  FULLPATH_QUETZAL, Attr.Alpha | Attr.Eee),//AE(4) AE,B,E(7)
                StateExRecord.Build(  FULLPATH_RABBIT, Attr.Alpha | Attr.Beta),// {E}(11) B(2) ([(A C)(B)]{E})(3)  AE,B,E(8)
                StateExRecord.Build(  FULLPATH_SHEEP, Attr.Eee),// AE,B,E(9)
                StateExRecord.Build(  FULLPATH_TIGER, Attr.Eee),// AE,B,E(10)
                StateExRecord.Build(  FULLPATH_UNICORN, Attr.Cee),// {E}(12) Nn(7)
                StateExRecord.Build(  FULLPATH_VIXEN, Attr.Eee),// AE,B,E(11) Nn(8)
                StateExRecord.Build(  FULLPATH_WOLF, Attr.Zero),// {E}(13)
                StateExRecord.Build(  FULLPATH_XENOPUS, Attr.Eee),// AE,B,E(12) Nn(9)
                StateExRecord.Build(  FULLPATH_YAK, Attr.Alpha),// {E}(14)
                StateExRecord.Build(  FULLPATH_ZEBRA, Attr.Alpha | Attr.Beta | Attr.Eee),// B(3) AE(5) AE,B,E(13)
            };
            foreach (StateExRecordable record in temp) { Hash_to_exRecord.Add(record.FullPathHash, record); }
        }
    }
}