using System;
using Xunit;
using Election.Interfaces;
using Election.Objects;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Election.Tests
{
    public class RunElectionTests
    {
        int numVoters = 1000000;
        int totalVoters = 0;
        static Random _random = new Random();
        static List<ICandidate> _candidates = new List<ICandidate>()
        {
            new SimpleCandidate(10001, "Eric Adams"),
            new SimpleCandidate(10002, "Shaun Donovan"),
            new SimpleCandidate(10003, "Kathryn Garcia"),
            new SimpleCandidate(10004, "Raymond McGuire"),
            /*new SimpleCandidate(10005, "Dianne Morales"),
            new SimpleCandidate(10006, "Scott Stringer"),
            new SimpleCandidate(10007, "Maya Wiley"),
            new SimpleCandidate(10008, "Andrew Yang")*/
        };

        public RunElectionTests()
        {
            this.totalVoters = numVoters + _candidates.Count;
        }

        [Fact]
        public void RunSimpleElection_RunElection_ReturnsAWinner()
        {
            List<IVoter> voters = GenerateVoters(numVoters, totalVoters);
            SimpleBallotGenerator simpleVoteGenerator = new SimpleBallotGenerator(_random);
            List<SimpleBallot> simpleBallots = simpleVoteGenerator.GenerateBallots(voters, _candidates);
            SimpleElection simpleElection = new SimpleElection(simpleBallots, _candidates);
            ICandidate simpleWinner = new SimpleElectionRunner().RunElection(simpleElection);
            Assert.NotNull(simpleWinner);
        }
        [Fact]
        public void RunRankedChoiceElection_RunElection_ReturnsAWinner()
        {
            List<IVoter> voters = GenerateVoters(numVoters, totalVoters);
            SimpleBallotGenerator simpleVoteGenerator = new SimpleBallotGenerator(_random);
            List<SimpleBallot> simpleBallots = simpleVoteGenerator.GenerateBallots(voters, _candidates);
            RankedChoiceBallotGenerator rankedChoiceVoteGenerator = new RankedChoiceBallotGenerator(_random);
            List<RankedChoiceBallot> rankedChoiceBallots = rankedChoiceVoteGenerator.GenerateBallots(voters, _candidates, simpleBallots);
            RankedChoiceElection rankedChoiceElection = new RankedChoiceElection(rankedChoiceBallots, _candidates);
            ICandidate rankedChoiceWinner = new RankedChoiceElectionRunner().RunElection(rankedChoiceElection);
            Assert.NotNull(rankedChoiceWinner);
        }

        private static List<IVoter> GenerateVoters(int numVoters, int totalVoters)
        {
            string voterFormat = $"{{0:{Regex.Replace(totalVoters.ToString(), "[1-9]", "0")}}}";
            List<IVoter> voters = new List<IVoter>(totalVoters);
            voters.AddRange(_candidates.Cast<IVoter>());
            int maxCandidateId = _candidates.Max(c => c.Id);
            for (int i = 1; i <= numVoters; i++)
            {
                int voterId = maxCandidateId + i;
                IVoter voter = new SimpleVoter(voterId, $"Voter {string.Format(voterFormat, voterId)}");
                voters.Add(voter);
            }
            return voters;
        }
    }
}
