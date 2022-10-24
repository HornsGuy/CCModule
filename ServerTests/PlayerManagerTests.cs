using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using CCModuleServerOnly;

namespace ServerTests
{
    [TestClass]
    public class PlayerManagerTests
    {
        [TestCleanup]
        public void TearDown()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            File.Delete(Path.Combine(basePath, "admins.json"));
            File.Delete(Path.Combine(basePath, "bans.json"));
            PlayerManager.Instance.Reset();
        }

        [TestMethod]
        public void JsonsCreatedTest()
        {
            PlayerManager.Instance.Setup();

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            Assert.IsTrue(File.Exists(Path.Combine(basePath, "admins.json")));
            Assert.IsTrue(File.Exists(Path.Combine(basePath, "bans.json")));
        }

        [Ignore]
        [TestMethod]
        public void TestFileCreate()
        {
            PlayerManager.Instance.BanPlayer("Test1", "2.0.0.3");
            PlayerManager.Instance.BanPlayer("Test2", "2.0.0.2");
            PlayerManager.Instance.BanPlayer("Test3", "2.0.0.1");

            PlayerManager.Instance.AddAdmin("Admin4", "2.0.0.90");
            PlayerManager.Instance.AddAdmin("Admin3", "2.0.0.80");
            PlayerManager.Instance.AddAdmin("Admin2", "2.0.0.70");
            PlayerManager.Instance.AddAdmin("Admin1", "2.0.0.60");
        }

        [TestMethod]
        public void LoadTest()
        {
            // Copy files to be loaded
            foreach (string testFile in Directory.GetFiles(@"..\..\..\TestFiles\PlayerManager"))
            {
                File.Copy(testFile, @".\"+ Path.GetFileName(testFile), true);
            }

            // Make sure the contents were loaded correctly
            Assert.IsTrue(PlayerManager.Instance.PlayerIsBanned("2.0.0.3"));
            Assert.IsTrue(PlayerManager.Instance.PlayerIsBanned("2.0.0.2"));
            Assert.IsTrue(PlayerManager.Instance.PlayerIsBanned("2.0.0.1"));
            Assert.IsFalse(PlayerManager.Instance.PlayerIsBanned("2.0.0.0"));

            Assert.IsTrue(PlayerManager.Instance.PlayerIsAdmin("2.0.0.90"));
            Assert.IsTrue(PlayerManager.Instance.PlayerIsAdmin("2.0.0.80"));
            Assert.IsTrue(PlayerManager.Instance.PlayerIsAdmin("2.0.0.70"));
            Assert.IsTrue(PlayerManager.Instance.PlayerIsAdmin("2.0.0.60"));
            Assert.IsFalse(PlayerManager.Instance.PlayerIsAdmin("2.0.0.50"));
        }

        [TestMethod]
        public void AddAdmin()
        {
            string ID = "2.0.0.1234";
            PlayerManager.Instance.AddAdmin("TestPlayer", ID);
            Assert.IsTrue(PlayerManager.Instance.PlayerIsAdmin(ID));
            Assert.IsFalse(PlayerManager.Instance.PlayerIsAdmin("TotallyDifferent"));
        }

        [TestMethod]
        public void BanPlayer()
        {
            string ID = "2.0.0.1234";
            PlayerManager.Instance.BanPlayer("TestPlayer", ID);
            Assert.IsTrue(PlayerManager.Instance.PlayerIsBanned(ID));
            Assert.IsFalse(PlayerManager.Instance.PlayerIsBanned("TotallyDifferent"));
        }

        [TestMethod]
        public void UnbanPlayer()
        {
            string ID = "2.0.0.1234";
            PlayerManager.Instance.BanPlayer("TestPlayer", ID);
            Assert.IsTrue(PlayerManager.Instance.PlayerIsBanned(ID));

            PlayerManager.Instance.UnbanPlayer(ID);
            Assert.IsFalse(PlayerManager.Instance.PlayerIsBanned(ID));
        }
    }
}
