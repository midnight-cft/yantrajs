﻿using System.Collections.Concurrent;

namespace YantraJs.Tests;

[TestFixture]
public class Tests5 : Base
{
    public class T1
    {
        private T1()
        {
        }

        public static T1 Create() => new T1();

        public int X { get; set; }
    }

    public class T2
    {
        public T2(int arg1, int arg2)
        {
        }

        public int X { get; set; }
    }

    public class ExClass
    {
        public ExClass() => throw new Exception();

        public ExClass(string x)
        {
            // does not throw here
        }

        public override bool Equals(object obj) => throw new Exception();

        public override int GetHashCode() => throw new Exception();

        public override string ToString() => throw new Exception();
    }

    [Test]
    public void CaseGetOrAdd_ParallelAccess_ShouldBeThreadSafe()
    {
        // Arrange
        int iterations = 1000;
        List<Task> parallelTasks = [];
        ConcurrentDictionary<Type, string> typeCache = new ConcurrentDictionary<Type, string>();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            Task task = Task.Run(() =>
            {
                string value = typeCache.GetOrAdd(typeof(string), t =>
                {
                    Thread.Sleep(10);
                    return "computed value";
                });
            });
            parallelTasks.Add(task);
        }

        // Assert
        Assert.DoesNotThrowAsync(async () => await Task.WhenAll(parallelTasks));
        Assert.That(typeCache.Count, Is.EqualTo(1));
    }

    [Test]
    public void CaseObject_With_Private_Constructor()
    {
        T1 t1 = T1.Create();
        t1.X = 42;
        T1 cloned = t1.YantraClone();
        t1.X = 0;
        Assert.That(cloned.X, Is.EqualTo(42));
    }

    [Test]
    public void CaseObject_With_Complex_Constructor()
    {
        T2 t2 = new T2(1, 2)
        {
            X = 42
        };
        T2 cloned = t2.YantraClone();
        t2.X = 0;
        Assert.That(cloned.X, Is.EqualTo(42));
    }

    [Test]
    public void CaseAnonymous_Object()
    {
        var t2 = new { A = 1, B = "x" };
        var cloned = t2.YantraClone();
        Assert.That(cloned.A, Is.EqualTo(1));
        Assert.That(cloned.B, Is.EqualTo("x"));
    }

    [Test]
    public void CaseCloner_Should_Not_Call_Any_Method_Of_Class_Be_Cloned()
    {
        Assert.DoesNotThrow(() => new ExClass("x").YantraClone());
        ExClass exClass = new ExClass("x");
        Assert.DoesNotThrow(() => new[] { exClass, exClass }.YantraClone());
    }
}