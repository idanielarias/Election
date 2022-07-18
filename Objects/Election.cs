using System;
using System.Collections.Generic;

using Election.Interfaces;

namespace Election.Objects
{
    public abstract class Election<TBallot, TVote> : IElection<TBallot, TVote> where TBallot : IBallot<TVote> where TVote : IVote
    {
        public IEnumerable<TBallot> Ballots { get; protected set; }
        public IEnumerable<ICandidate> Candidates { get; protected set; }
        public ICandidate Winner { get; protected set; }
        public string Result { get; protected set; }
        public int Count { get; protected set; }

        public Election(IEnumerable<TBallot> ballots, IEnumerable<ICandidate> candidates)
        {
            this.Ballots = ballots;
            this.Candidates = candidates;
            foreach (TBallot ballot in ballots)
                this.Count++;
        }

        public abstract void CountVotes();
    }

    class SimpleElection : Election<SimpleBallot, SimpleVote>
    {
        public SimpleElection(IEnumerable<SimpleBallot> ballots, IEnumerable<ICandidate> candidates) : base(ballots, candidates) { }

        public override void CountVotes()
        {
            // creates a dictionary of candidates and their votes
            Dictionary<ICandidate, int> votes = new Dictionary<ICandidate, int>();
            int maxVotes = 0;

            foreach (SimpleBallot ballot in this.Ballots)
            {
                foreach (SimpleVote vote in ballot.Votes)
                {
                    if (votes.ContainsKey(vote.Candidate))
                    {
                        votes[vote.Candidate]++;
                    }
                    else
                    {
                        votes.Add(vote.Candidate, 1);
                    }
                }
            }

            foreach (KeyValuePair<ICandidate, int> vote in votes)
            {
                if (vote.Value > maxVotes)
                {
                    maxVotes = vote.Value;
                    this.Winner = vote.Key;
                    this.Result = "Winner";
                }
            }
        }
    }

    class RankedChoiceElection : Election<RankedChoiceBallot, RankedChoiceVote>
    {
        List<RankedChoiceBallot> rankedChoiceBallots = null;
        int minVotes = 0;
        int maxVotes = 0;

        public RankedChoiceElection(IEnumerable<RankedChoiceBallot> ballots, IEnumerable<ICandidate> candidates) : base(ballots, candidates) { }

        public override void CountVotes()
        {
            // creates a dictionary of candidates and their votes
            Dictionary<ICandidate, int> tally = new Dictionary<ICandidate, int>();
            rankedChoiceBallots = new List<RankedChoiceBallot>();
            

            // iterates through each vote and counts the votes with rank 1
            foreach (RankedChoiceBallot ballot in this.Ballots)
            {
                foreach (RankedChoiceVote vote in ballot.Votes)
                {
                    if (vote.Rank == 1)
                    {
                        if (tally.ContainsKey(vote.Candidate))
                        {
                            tally[vote.Candidate]++;
                        }
                        else
                        {
                            tally.Add(vote.Candidate, 1);
                        }
                    }
                }
                rankedChoiceBallots.Add(new RankedChoiceBallot(ballot.Votes));
            }

            CountTallyVotes(tally);

            // if there is no winner, runs a n round counting
            while (this.Winner == null)
            {
                if (tally.Count == 0)
                {
                    this.Winner = null;
                    this.Result = "No winner";
                    break;
                }

                // finds the candidate with the least votes
                ICandidate lessVotedCandidate = null;

                int lvcVotes = 0;
                foreach (KeyValuePair<ICandidate, int> vote in tally)
                {
                    if (lvcVotes == 0)
                    {
                        lvcVotes = vote.Value;
                        lessVotedCandidate = vote.Key;
                    }
                    else if (vote.Value < lvcVotes)
                    {
                        lvcVotes = vote.Value;
                        lessVotedCandidate = vote.Key;
                    }
                }
                RunSecondPreferenceCounting(rankedChoiceBallots, tally, lessVotedCandidate);
                tally.Remove(lessVotedCandidate);
                // TODO: remove ballots from less voted candidates
            }

            // TODO: Test if we have ties
            if (minVotes == maxVotes)
            {
                this.Winner = null;
                this.Result = "Tie";
            }
        }

        private void RunSecondPreferenceCounting(List<RankedChoiceBallot> ballots, Dictionary<ICandidate, int> tally, ICandidate lessVotedCandidate)
        {
            // creates a dictionary of the least voted candidate's ballots
            List<RankedChoiceBallot> lessVotedBallots = new List<RankedChoiceBallot>();

            // iterates through the ballots and adds the least voted ones to the list
            foreach (RankedChoiceBallot ballot in ballots)
            {
                foreach (RankedChoiceVote vote in ballot.Votes)
                {
                    if (vote.Candidate == lessVotedCandidate)
                    {
                        if (tally.ContainsKey(vote.Candidate))
                        {
                            if (vote.Rank == 1)
                                // adds the ballot to the less voted ballots list
                                lessVotedBallots.Add(ballot);
                        }
                    }
                }
            }

            // iterates through the less voted ballots and sums their respective candidate's votes with rank 2 to the tally
            foreach (RankedChoiceBallot ballot in lessVotedBallots)
            {
                foreach (RankedChoiceVote vote in ballot.Votes)
                {
                    if (tally.ContainsKey(vote.Candidate))
                    {
                        if (vote.Rank == 2)
                            // adds the second choice preference vote
                            tally[vote.Candidate]++;
                    }
                }
            }

            CountTallyVotes(tally);
        }

        private void CountTallyVotes(Dictionary<ICandidate, int> tally)
        {
            this.maxVotes = 0;
            // finds the candidate with the most votes
            foreach (ICandidate candidate in this.Candidates)
            {
                if (tally.ContainsKey(candidate))
                {
                    if (tally[candidate] > this.maxVotes)
                    {
                        // the winner is the candidate with an outright majority (50% + 1)
                        if (tally[candidate] > (this.Count / 2) + 1)
                        {
                            this.Winner = candidate;
                            this.Result = "Winner";
                        }
                        this.maxVotes = tally[candidate];
                    }
                }
            }

            this.minVotes = 0;
            // gets the minVotes
            foreach (KeyValuePair<ICandidate, int> vote in tally)
            {
                if (this.minVotes == 0)
                {
                    this.minVotes = vote.Value;
                }
                else if (vote.Value < this.minVotes)
                {
                    this.minVotes = vote.Value;
                }
            }
        }
    }
}
