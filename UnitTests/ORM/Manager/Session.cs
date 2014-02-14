﻿/*
Copyright (c) 2014 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utilities.ORM.BaseClasses;
using Utilities.ORM.Interfaces;
using Utilities.ORM.Manager.Schema.Default.Database;
using Utilities.ORM.Manager.Schema.Interfaces;
using Utilities.ORM.Manager.SourceProvider.Interfaces;
using Xunit;

namespace UnitTests.ORM.Manager
{
    public class Session : DatabaseBaseClass
    {
        public Session()
            : base()
        {
            var BootLoader = Utilities.IoC.Manager.Bootstrapper;
        }

        [Fact]
        public void Create()
        {
            Assert.DoesNotThrow(() => new Utilities.ORM.Manager.Session());
        }

        public override void Dispose()
        {
            base.Dispose();
            Utilities.ORM.Manager.QueryProvider.Default.DatabaseBatch Temp = new Utilities.ORM.Manager.QueryProvider.Default.DatabaseBatch(MasterDatabaseSource);
            try
            {
                Temp.AddCommand(null, null, CommandType.Text, "ALTER DATABASE SessionTestDatabase SET OFFLINE WITH ROLLBACK IMMEDIATE")
                        .AddCommand(null, null, CommandType.Text, "ALTER DATABASE SessionTestDatabase SET ONLINE")
                        .AddCommand(null, null, CommandType.Text, "DROP DATABASE SessionTestDatabase")
                        .Execute();
            }
            catch { }
        }

        [Fact]
        public void Save()
        {
            Utilities.ORM.Manager.Session TestObject = new Utilities.ORM.Manager.Session();
            TestClass TempObject = new TestClass();
            TempObject.BoolReference = true;
            TempObject.ByteArrayReference = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            TempObject.ByteReference = 12;
            TempObject.CharReference = 'v';
            TempObject.DecimalReference = 1.4213m;
            TempObject.DoubleReference = 1.32645d;
            TempObject.EnumReference = TestEnum.Value2;
            TempObject.FloatReference = 1234.5f;
            TempObject.GuidReference = Guid.NewGuid();
            TempObject.IntReference = 145145;
            TempObject.LongReference = 763421;
            TempObject.ManyToManyIEnumerable = new TestClass[] { new TestClass(), new TestClass() };
            TempObject.ManyToManyList = new TestClass[] { new TestClass(), new TestClass(), new TestClass() }.ToList();
            TempObject.ManyToOneIEnumerable = new TestClass[] { new TestClass(), new TestClass(), new TestClass() };
            TempObject.ManyToOneItem = new TestClass();
            TempObject.ManyToOneList = new TestClass[] { new TestClass(), new TestClass(), new TestClass() }.ToList();
            TempObject.Map = new TestClass();
            TempObject.NullStringReference = null;
            TempObject.ShortReference = 5423;
            TempObject.StringReference = "agsdpghasdg";
            TestObject.Save<TestClass, int>(TempObject);

            Utilities.ORM.Manager.QueryProvider.Default.DatabaseBatch Temp = new Utilities.ORM.Manager.QueryProvider.Default.DatabaseBatch(new Utilities.ORM.Manager.SourceProvider.Manager().GetSource("Data Source=localhost;Initial Catalog=SessionTestDatabase;Integrated Security=SSPI;Pooling=false"));

            TestClass Item = Temp.AddCommand(null, null, CommandType.Text, "SELECT * FROM TestClass_")
                .Execute()
                .First()
                .First();
            Assert.Equal(true, Item.BoolReference);
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, Item.ByteArrayReference);
            Assert.Equal(12, Item.ByteReference);
            Assert.Equal('v', Item.CharReference);
            Assert.Equal(1.4213m, Item.DecimalReference);
            Assert.Equal(1.32645d, Item.DoubleReference);
            Assert.Equal(TestEnum.Value2, Item.EnumReference);
            Assert.Equal(1234.5f, Item.FloatReference);
            Assert.Equal(Item.GuidReference, TempObject.GuidReference);
            Assert.Equal(145145, TempObject.IntReference);
            Assert.Equal(763421, TempObject.LongReference);
            Assert.Equal(2, TempObject.ManyToManyIEnumerable.Count());
            Assert.Equal(3, TempObject.ManyToManyList.Count);
            Assert.Equal(3, TempObject.ManyToOneIEnumerable.Count());
            Assert.NotNull(TempObject.ManyToOneItem);
            Assert.Equal(4, TempObject.ManyToOneList.Count);
            Assert.NotNull(TempObject.Map);
            Assert.Equal(null, TempObject.NullStringReference);
            Assert.Equal(5423, TempObject.ShortReference);
            Assert.Equal("agsdpghasdg", TempObject.StringReference);
        }

        public enum TestEnum
        {
            Value1 = 0,
            Value2,
            Value3
        }

        public class TestClass
        {
            public bool BoolReference { get; set; }

            public byte[] ByteArrayReference { get; set; }

            public byte ByteReference { get; set; }

            public char CharReference { get; set; }

            public decimal DecimalReference { get; set; }

            public double DoubleReference { get; set; }

            public TestEnum EnumReference { get; set; }

            public float FloatReference { get; set; }

            public Guid GuidReference { get; set; }

            public int ID { get; set; }

            public int IntReference { get; set; }

            public long LongReference { get; set; }

            public IEnumerable<TestClass> ManyToManyIEnumerable { get; set; }

            public List<TestClass> ManyToManyList { get; set; }

            public IEnumerable<TestClass> ManyToOneIEnumerable { get; set; }

            public TestClass ManyToOneItem { get; set; }

            public List<TestClass> ManyToOneList { get; set; }

            public TestClass Map { get; set; }

            public string NullStringReference { get; set; }

            public short ShortReference { get; set; }

            public string StringReference { get; set; }
        }

        public class TestClassDatabase : IDatabase
        {
            public bool Audit
            {
                get { return false; }
            }

            public string Name
            {
                get { return "Data Source=localhost;Initial Catalog=SessionTestDatabase;Integrated Security=SSPI;Pooling=false"; }
            }

            public int Order
            {
                get { return 0; }
            }

            public bool Readable
            {
                get { return true; }
            }

            public bool Update
            {
                get { return true; }
            }

            public bool Writable
            {
                get { return true; }
            }
        }

        public class TestClassMapping : MappingBaseClass<TestClass, TestClassDatabase>
        {
            public TestClassMapping()
                : base()
            {
                ID(x => x.ID).SetAutoIncrement();
                ManyToMany(x => x.ManyToManyIEnumerable).SetTableName("ManyToManyIEnumerable").SetCascade();
                ManyToMany(x => x.ManyToManyList).SetTableName("ManyToManyList").SetCascade();
                ManyToOne(x => x.ManyToOneIEnumerable).SetTableName("ManyToOneIEnumerable").SetCascade();
                ManyToOne(x => x.ManyToOneList).SetTableName("ManyToOneList").SetCascade();
                ManyToOne(x => x.ManyToOneItem).SetTableName("ManyToOneList");
                Map(x => x.Map).SetCascade();
                Reference(x => x.BoolReference);
                Reference(x => x.ByteArrayReference).SetMaxLength(100);
                Reference(x => x.ByteReference);
                Reference(x => x.CharReference);
                Reference(x => x.DecimalReference);
                Reference(x => x.DoubleReference);
                Reference(x => x.EnumReference);
                Reference(x => x.FloatReference);
                Reference(x => x.GuidReference);
                Reference(x => x.IntReference);
                Reference(x => x.LongReference);
                Reference(x => x.NullStringReference).SetMaxLength(100);
                Reference(x => x.ShortReference);
                Reference(x => x.StringReference).SetMaxLength(100);
            }
        }
    }
}