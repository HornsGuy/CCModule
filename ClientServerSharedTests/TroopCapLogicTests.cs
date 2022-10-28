using ClientServerShared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ClientServerSharedTests
{
    [TestClass]
    public class TroopCapLogicTests
    {
        [TestMethod]
        public void TestGetCurrentTroopClassPercents()
        {
            List<int> myTeamTroopIndeces = new List<int>();
            Dictionary<int, string> troopIndexToClass = new Dictionary<int, string>();
            troopIndexToClass.Add(0, "Infantry");
            troopIndexToClass.Add(1, "Infantry");
            troopIndexToClass.Add(2, "Infantry");
            troopIndexToClass.Add(3, "Ranged");
            troopIndexToClass.Add(4, "Ranged");
            troopIndexToClass.Add(5, "Cavalry");

            myTeamTroopIndeces.Add(0);
            myTeamTroopIndeces.Add(1);
            myTeamTroopIndeces.Add(2);
            myTeamTroopIndeces.Add(3);
            myTeamTroopIndeces.Add(4);
            myTeamTroopIndeces.Add(5);

            Dictionary<string, float> expected = new Dictionary<string, float>();
            expected.Add("Infantry", 50.0f);
            expected.Add("Ranged", 33.333f);
            expected.Add("Cavalry", 16.666f);

            Dictionary<string, float> actual = TroopCapLogic.GetCurrentTeamClassTypeBreakdown(myTeamTroopIndeces, troopIndexToClass, new List<string>() { "Infantry", "Ranged", "Cavalry"});

            foreach (var keyVal in expected)
            {
                Assert.IsTrue(UnitTestHelper.AlmostEquals(keyVal.Value, actual[keyVal.Key]));
                actual.Remove(keyVal.Key);
            }

            Assert.IsTrue(actual.Count == 0);

        }

        [TestMethod]
        public void GetTroopClassAvailabilityDictionaryTestAllAvailable()
        {
            Dictionary<string, float> currentBreakdown = new Dictionary<string, float>();
            currentBreakdown.Add("Infantry", 50.0f);
            currentBreakdown.Add("Ranged", 33.333f);
            currentBreakdown.Add("Cavalry", 16.666f);

            Dictionary<string, int> troopCapPercents = new Dictionary<string, int>();
            troopCapPercents.Add("Infantry", 51);
            troopCapPercents.Add("Ranged", 34);
            troopCapPercents.Add("Cavalry", 17);

            Dictionary<string, bool> expected = new Dictionary<string, bool>();
            expected.Add("Infantry", true);
            expected.Add("Ranged", true);
            expected.Add("Cavalry", true);

            Dictionary<string, bool> actual = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentBreakdown, troopCapPercents);

            foreach (var keyVal in expected)
            {
                Assert.AreEqual(keyVal.Value, actual[keyVal.Key]);
                actual.Remove(keyVal.Key);
            }
            Assert.IsTrue(actual.Count == 0);
        }

        [TestMethod]
        public void GetTroopClassAvailabilityDictionaryTestOneAvailable()
        {
            Dictionary<string, float> currentBreakdown = new Dictionary<string, float>();
            currentBreakdown.Add("Infantry", 50.0f);
            currentBreakdown.Add("Ranged", 33.333f);
            currentBreakdown.Add("Cavalry", 16.666f);

            Dictionary<string, int> troopCapPercents = new Dictionary<string, int>();
            troopCapPercents.Add("Infantry", 51);
            troopCapPercents.Add("Ranged", 33);
            troopCapPercents.Add("Cavalry", 16);

            Dictionary<string, bool> expected = new Dictionary<string, bool>();
            expected.Add("Infantry", true);
            expected.Add("Ranged", false);
            expected.Add("Cavalry", false);

            Dictionary<string, bool> actual = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentBreakdown, troopCapPercents);

            foreach (var keyVal in expected)
            {
                Assert.AreEqual(keyVal.Value, actual[keyVal.Key]);
                actual.Remove(keyVal.Key);
            }
            Assert.IsTrue(actual.Count == 0);
        }

        [TestMethod]
        public void GetTroopClassAvailabilityDictionaryTestNoneAvailable()
        {
            Dictionary<string, float> currentBreakdown = new Dictionary<string, float>();
            currentBreakdown.Add("Infantry", 50.0f);
            currentBreakdown.Add("Ranged", 33.333f);
            currentBreakdown.Add("Cavalry", 16.666f);

            Dictionary<string, int> troopCapPercents = new Dictionary<string, int>();
            troopCapPercents.Add("Infantry", 49);
            troopCapPercents.Add("Ranged", 33);
            troopCapPercents.Add("Cavalry", 16);

            Dictionary<string, bool> expected = new Dictionary<string, bool>();
            expected.Add("Infantry", false);
            expected.Add("Ranged", false);
            expected.Add("Cavalry", false);

            Dictionary<string, bool> actual = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentBreakdown, troopCapPercents);

            foreach (var keyVal in expected)
            {
                Assert.AreEqual(keyVal.Value, actual[keyVal.Key]);
                actual.Remove(keyVal.Key);
            }
            Assert.IsTrue(actual.Count == 0);
        }

        [TestMethod]
        public void GetTroopClassAvailabilityDictionary100Available()
        {
            Dictionary<string, float> currentBreakdown = new Dictionary<string, float>();
            currentBreakdown.Add("Infantry", 100.0f);
            currentBreakdown.Add("Ranged", 0.0f);
            currentBreakdown.Add("Cavalry", 0.0f);

            Dictionary<string, int> troopCapPercents = new Dictionary<string, int>();
            troopCapPercents.Add("Infantry", 100);
            troopCapPercents.Add("Ranged", 100);
            troopCapPercents.Add("Cavalry", 100);

            Dictionary<string, bool> expected = new Dictionary<string, bool>();
            expected.Add("Infantry", true);
            expected.Add("Ranged", true);
            expected.Add("Cavalry", true);

            Dictionary<string, bool> actual = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentBreakdown, troopCapPercents);

            foreach (var keyVal in expected)
            {
                Assert.AreEqual(keyVal.Value, actual[keyVal.Key]);
                actual.Remove(keyVal.Key);
            }
            Assert.IsTrue(actual.Count == 0);
        }

        [TestMethod]
        public void GetTroopClassAvailabilityDictionary0Available()
        {
            Dictionary<string, float> currentBreakdown = new Dictionary<string, float>();
            currentBreakdown.Add("Infantry", 100.0f);
            currentBreakdown.Add("Ranged", 0.0f);
            currentBreakdown.Add("Cavalry", 0.0f);

            Dictionary<string, int> troopCapPercents = new Dictionary<string, int>();
            troopCapPercents.Add("Infantry", 0);
            troopCapPercents.Add("Ranged", 0);
            troopCapPercents.Add("Cavalry", 0);

            Dictionary<string, bool> expected = new Dictionary<string, bool>();
            expected.Add("Infantry", false);
            expected.Add("Ranged", false);
            expected.Add("Cavalry", false);

            Dictionary<string, bool> actual = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentBreakdown, troopCapPercents);

            foreach (var keyVal in expected)
            {
                Assert.AreEqual(keyVal.Value, actual[keyVal.Key]);
                actual.Remove(keyVal.Key);
            }
            Assert.IsTrue(actual.Count == 0);
        }

        [TestMethod]
        public void EmptyBreakdownTest()
        {
            Dictionary<string, float> currentBreakdown = new Dictionary<string, float>();

            Dictionary<string, int> troopCapPercents = new Dictionary<string, int>();
            troopCapPercents.Add("Infantry", 100);
            troopCapPercents.Add("Ranged", 50);
            troopCapPercents.Add("Cavalry", 0);

            Dictionary<string, bool> expected = new Dictionary<string, bool>();
            expected.Add("Infantry", true);
            expected.Add("Ranged", true);
            expected.Add("Cavalry", false);

            Dictionary<string, bool> actual = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentBreakdown, troopCapPercents);

            foreach (var keyVal in expected)
            {
                Assert.AreEqual(keyVal.Value, actual[keyVal.Key]);
                actual.Remove(keyVal.Key);
            }
            Assert.IsTrue(actual.Count == 0);
        }
    }
}
