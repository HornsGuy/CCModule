using BannerlordWrapper;

namespace BannerlordWrapperNUnit
{
    public class PlayerWrapperTests
    {
        [TearDown]
        public void TearDown()
        {
            TeamWrapper.Instance.Reset();
            PlayerWrapper.Instance.Reset();
        }

        [Test]
        [TestCase("1.2.3.4", "1.2.3.4",TeamType.Attacker)]
        [TestCase("1.2.3.4", "1.2.3.5",TeamType.Spectator)]
        public void TestPlayerKeepingTeamAfterLeaving(string ID1, string ID2, TeamType expected)
        {
            Player p = new Player(ID1, "Test", TeamType.Spectator);
            PlayerWrapper.Instance.AddPlayer(p);

            p.ChangeTeam(TeamType.Attacker);

            PlayerWrapper.Instance.RemovePlayer(p.ID);

            Player p1 = new Player(ID2, "Test", TeamType.Spectator);
            PlayerWrapper.Instance.AddPlayer(p1);

            Assert.IsTrue(p1.Team.TeamType == expected);

        }
    }
}
