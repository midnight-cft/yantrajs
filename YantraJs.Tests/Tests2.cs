﻿namespace YantraJs.Tests;

[TestFixture]
public class Tests2 : Base
{
    public class C1
    {
        public int F { get; set; }

        public C1 A { get; set; }
    }

    [Test]
    public void CaseSimpleLoop_Should_Be_Handled()
    {
        C1 c1 = new C1();
        C1 c2 = new C1();
        c1.F = 1;
        c2.F = 2;
        c1.A = c2;
        c1.A.A = c1;
        C1 cloned = c1.YantraClone();

        Assert.That(cloned.A, Is.Not.Null);
        Assert.That(cloned.A.A.F, Is.EqualTo(cloned.F));
        Assert.That(cloned.A.A, Is.EqualTo(cloned));
    }

    [Test]
    public void CaseObject_Own_Loop_Should_Be_Handled()
    {
        C1 c1 = new C1
        {
            F = 1
        };
        c1.A = c1;
        C1 cloned = c1.YantraClone();

        Assert.That(cloned.A, Is.Not.Null);
        Assert.That(cloned.A.F, Is.EqualTo(cloned.F));
        Assert.That(cloned.A, Is.EqualTo(cloned));
    }

    [Test]
    public void CaseArray_Of_Same_Objects()
    {
        C1 c1 = new C1();
        C1[] arr = [c1, c1, c1];
        c1.F = 1;
        C1[] cloned = arr.YantraClone();

        Assert.That(cloned.Length, Is.EqualTo(3));
        Assert.That(cloned[0], Is.EqualTo(cloned[1]));
        Assert.That(cloned[1], Is.EqualTo(cloned[2]));
    }
}