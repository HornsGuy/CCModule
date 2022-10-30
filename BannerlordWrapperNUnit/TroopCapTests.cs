using BannerlordWrapper;

namespace BannerlordWrapperNUnit
{
    public class TroopCapTestsParent
    {
        public static int idCount = 0;
        public const int AttackerInfIndex = 0;
        public const int AttackerRangeIndex = 2;
        public const int AttackerCavIndex = 4;
        public const int AttackerHAIndex = 5;

        public const int DefenderInfIndex = 0;
        public const int DefenderRangeIndex = 3;
        public const int DefenderCavIndex = 5;

        [TearDown]
        public void TearDown()
        {
            idCount = 0;
            TeamWrapper.Instance.Reset();
            PlayerWrapper.Instance.Reset();
        }

        public void AddPlayersToTeam(TeamType team, int numToAdd, int troopIndex)
        {
            // Add Kuzhait Infantry Players
            for (int i = 0; i < numToAdd; i++)
            {
                string playerName = team.ToString() + "_" + idCount.ToString() + "_" + idCount;
                Player p = new Player(idCount.ToString(), playerName, team, troopIndex);
                PlayerWrapper.Instance.AddPlayer(p);

                idCount++;
            }
        }

        public void AddTestTeams()
        {
            // Setup Kuzhait for team 1
            Dictionary<int, Troop> khuzait = new Dictionary<int, Troop>();
            khuzait.Add(0, new Troop(0, "Rabble", TroopType.Infantry));
            khuzait.Add(1, new Troop(1, "Spear Infantry", TroopType.Infantry));
            khuzait.Add(2, new Troop(2, "Steppe Bow", TroopType.Ranged));
            khuzait.Add(3, new Troop(3, "Khan's Guard", TroopType.Ranged));
            khuzait.Add(4, new Troop(4, "Nomad", TroopType.Cavalry));
            khuzait.Add(5, new Troop(5, "Mounted Archer", TroopType.HorseArcher));
            khuzait.Add(6, new Troop(6, "Lancer", TroopType.Cavalry));
            TeamWrapper.Instance.SetFactionForTeam(TeamType.Attacker, "khuzait", khuzait);

            // Setup Vlandia for team 2
            Dictionary<int, Troop> vlandia = new Dictionary<int, Troop>();
            vlandia.Add(0, new Troop(0, "Peasant", TroopType.Infantry));
            vlandia.Add(1, new Troop(1, "Voulgier", TroopType.Infantry));
            vlandia.Add(2, new Troop(2, "Sergeant", TroopType.Infantry));
            vlandia.Add(3, new Troop(3, "Arbelist", TroopType.Ranged));
            vlandia.Add(4, new Troop(4, "Sharpshooter", TroopType.Ranged));
            vlandia.Add(5, new Troop(5, "Vanguard", TroopType.Cavalry));
            vlandia.Add(6, new Troop(6, "Knight", TroopType.Cavalry));
            TeamWrapper.Instance.SetFactionForTeam(TeamType.Defender, "vlandia", vlandia);
        }
    }

    public class TroopCapTests200Players : TroopCapTestsParent
    {   

        [SetUp]
        public void Setup()
        {
            AddTestTeams();

            // Add Players
            AddPlayersToTeam(TeamType.Attacker, 40, 0); // Inf
            AddPlayersToTeam(TeamType.Attacker, 30, 2); // Range
            AddPlayersToTeam(TeamType.Attacker, 20, 4); // Cav
            AddPlayersToTeam(TeamType.Attacker, 10, 5); // HA

            AddPlayersToTeam(TeamType.Defender, 50, 0); // Inf
            AddPlayersToTeam(TeamType.Defender, 30, 3); // Range
            AddPlayersToTeam(TeamType.Defender, 20, 5); // Cav
        }

        

        [Test]
        // All 100
        [TestCase(AttackerInfIndex,100,true)]
        [TestCase(AttackerRangeIndex,100,true)]
        [TestCase(AttackerCavIndex,100,true)]
        [TestCase(AttackerHAIndex,100,true)]
        // 1 above current breakdown
        [TestCase(AttackerInfIndex, 41, true)]
        [TestCase(AttackerRangeIndex, 31, true)]
        [TestCase(AttackerCavIndex, 21, true)]
        [TestCase(AttackerHAIndex, 11, true)]
        // At breakdown
        [TestCase(AttackerInfIndex, 40, true)]
        [TestCase(AttackerRangeIndex, 30, true)]
        [TestCase(AttackerCavIndex, 20, true)]
        [TestCase(AttackerHAIndex, 10, true)]
        // 1 Below Breakdown
        [TestCase(AttackerInfIndex, 39, false)]
        [TestCase(AttackerRangeIndex, 29, false)]
        [TestCase(AttackerCavIndex, 19, false)]
        [TestCase(AttackerHAIndex, 9, false)]
        // 0
        [TestCase(AttackerInfIndex, 0, false)]
        [TestCase(AttackerRangeIndex, 0, false)]
        [TestCase(AttackerCavIndex, 0, false)]
        [TestCase(AttackerHAIndex, 0, false)]
        public void TroopCapAttackerParameterizedTests(int troopIndex, int troopCap, bool expected)
        {
            Player testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, troopIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, troopCap), Is.EqualTo(expected));
        }

        [Test]
        // All 100
        [TestCase( DefenderInfIndex, 100, true)]
        [TestCase( DefenderRangeIndex, 100, true)]
        [TestCase( DefenderCavIndex, 100, true)]
        // 1 above current breakdown
        [TestCase(DefenderInfIndex, 51, true)]
        [TestCase(DefenderRangeIndex, 31, true)]
        [TestCase(DefenderCavIndex, 21, true)]
        // At breakdown
        [TestCase(DefenderInfIndex, 50, true)]
        [TestCase(DefenderRangeIndex, 30, true)]
        [TestCase(DefenderCavIndex, 20, true)]
        // 1 Below Breakdown
        [TestCase(DefenderInfIndex, 49, false)]
        [TestCase(DefenderRangeIndex, 29, false)]
        [TestCase(DefenderCavIndex, 19, false)]
        // 0
        [TestCase(DefenderInfIndex, 0, false)]
        [TestCase(DefenderRangeIndex, 0, false)]
        [TestCase(DefenderCavIndex, 0, false)]
        public void TroopCapDefenderParameterizedTests(int troopIndex, int troopCap, bool expected)
        {
            Player testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Defender, troopIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, troopCap), Is.EqualTo(expected));
        }
    }

    public class TroopCapTests280v230Players : TroopCapTestsParent
    {


        [SetUp]
        public void Setup()
        {
            AddTestTeams();

            // Add Players
            AddPlayersToTeam(TeamType.Attacker, 70, 0); // Inf
            AddPlayersToTeam(TeamType.Attacker, 60, 1); // Inf
            AddPlayersToTeam(TeamType.Attacker, 50, 2); // Range
            AddPlayersToTeam(TeamType.Attacker, 40, 3); // Range
            AddPlayersToTeam(TeamType.Attacker, 30, 4); // Cav
            AddPlayersToTeam(TeamType.Attacker, 20, 6); // Cav
            AddPlayersToTeam(TeamType.Attacker, 10, 5); // HA

            AddPlayersToTeam(TeamType.Defender, 60, 0); // Inf
            AddPlayersToTeam(TeamType.Defender, 50, 1); // Inf
            AddPlayersToTeam(TeamType.Defender, 40, 2); // Inf
            AddPlayersToTeam(TeamType.Defender, 30, 3); // Range
            AddPlayersToTeam(TeamType.Defender, 30, 4); // Range
            AddPlayersToTeam(TeamType.Defender, 10, 5); // Cav
            AddPlayersToTeam(TeamType.Defender, 10, 6); // Cav
        }

        [Test]
        // All 100
        [TestCase(AttackerInfIndex, 100, true)]
        [TestCase(AttackerRangeIndex, 100, true)]
        [TestCase(AttackerCavIndex, 100, true)]
        [TestCase(AttackerHAIndex, 100, true)]
        // 1 above current breakdown
        [TestCase(AttackerInfIndex, 47, true)]
        [TestCase(AttackerRangeIndex, 33, true)]
        [TestCase(AttackerCavIndex, 18, true)]
        [TestCase(AttackerHAIndex, 4, true)]
        // 1 Below Breakdown
        [TestCase(AttackerInfIndex, 46, false)]
        [TestCase(AttackerRangeIndex, 32, false)]
        [TestCase(AttackerCavIndex, 17, false)]
        [TestCase(AttackerHAIndex, 3, false)]
        // 0
        [TestCase(AttackerInfIndex, 0, false)]
        [TestCase(AttackerRangeIndex, 0, false)]
        [TestCase(AttackerCavIndex, 0, false)]
        [TestCase(AttackerHAIndex, 0, false)]
        public void TroopCapAttackerParameterizedTests(int troopIndex, int troopCap, bool expected)
        {
            Player testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, troopIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, troopCap), Is.EqualTo(expected));
        }

        [Test]
        // All 100
        [TestCase(DefenderInfIndex, 100, true)]
        [TestCase(DefenderRangeIndex, 100, true)]
        [TestCase(DefenderCavIndex, 100, true)]
        // 1 above current breakdown
        [TestCase(DefenderInfIndex, 66, true)]
        [TestCase(DefenderRangeIndex, 27, true)]
        [TestCase(DefenderCavIndex, 9, true)]
        // 1 Below Breakdown
        [TestCase(DefenderInfIndex, 65, false)]
        [TestCase(DefenderRangeIndex, 26, false)]
        [TestCase(DefenderCavIndex, 8, false)]
        // 0
        [TestCase(DefenderInfIndex, 0, false)]
        [TestCase(DefenderRangeIndex, 0, false)]
        [TestCase(DefenderCavIndex, 0, false)]
        public void TroopCapDefenderParameterizedTests(int troopIndex, int troopCap, bool expected)
        {
            Player testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Defender, troopIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, troopCap), Is.EqualTo(expected));
        }
    }

    public class TroopCapTestsSpecialCases : TroopCapTestsParent
    {

        [Test]
        public void HundredPercentSingleTroopType()
        {
            AddTestTeams();
            AddPlayersToTeam(TeamType.Attacker, 100, AttackerInfIndex);

            // Infantry makes up 100% of attackes, make sure players can spawn as inf
            Player testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerInfIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 100), Is.EqualTo(true));

            // Ranged is 0% of attackers, make sure players can't spawn as range
            testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerRangeIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 0), Is.EqualTo(false));

            // Ranged is 0% of attackers, make sure players can't spawn as range
            testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerCavIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 0), Is.EqualTo(false));

            // Ranged is 0% of attackers, make sure players can't spawn as range
            testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerHAIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 0), Is.EqualTo(false));

        }

        [Test]
        public void HundredSpectatorsHundredAttackers()
        {
            AddTestTeams();
            AddPlayersToTeam(TeamType.Spectator, 100, 0);
            AddPlayersToTeam(TeamType.Attacker, 100, AttackerInfIndex);

            // Infantry makes up 100% of attackes, make sure players can spawn as inf
            Player testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerInfIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 100), Is.EqualTo(true));

            // Ranged is 0% of attackers, make sure players can't spawn as range
            testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerRangeIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 0), Is.EqualTo(false));

            // Ranged is 0% of attackers, make sure players can't spawn as range
            testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerCavIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 0), Is.EqualTo(false));

            // Ranged is 0% of attackers, make sure players can't spawn as range
            testPlayer = new Player(idCount++.ToString(), "TestPlayer", TeamType.Attacker, AttackerHAIndex);
            Assert.That(TroopCapLogic.PlayerCanSpawn(testPlayer, 0), Is.EqualTo(false));
        }
    }
}