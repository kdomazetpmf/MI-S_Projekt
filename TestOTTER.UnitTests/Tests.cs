using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OTTER;

namespace TestOTTER.UnitTests
{
    [TestClass]
    public class GlavniLikTests 
    {
        GlavniLik lik = new GlavniLik ("sprites\\glavnilik.png", 50, 235);
        int ocekivano = 0;

        [TestMethod]
        public void VratiZetoneBezUdarcaTest ( )
        {
            lik.SkupljeniZetoni = 5;
            int dobiveno = lik.VratiZetoneBezUdarca();
            Assert.AreEqual(ocekivano, dobiveno, "Krivo vraćanje brojača žetona.");

        }
    }
    [TestClass]
    public class StatickaTests
    {
        [TestMethod]
        public void Random1Test ( )
        {
            int dobiveno = Staticka.Random1();
            if (1 <= dobiveno || dobiveno < 3)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }

        [TestMethod]
        public void Random2Test ( )
        {
            int dobiveno = Staticka.Random2();
            if (1 <= dobiveno || dobiveno < 3)
                Assert.IsTrue(true);
            else
                Assert.IsTrue(false);

        }
    }
}
