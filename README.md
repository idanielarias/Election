Given the recent buzz around the New York City mayoral election and its use of ranked-choice voting (a basic explanation of which can be found here: (https://ballotpedia.org/Ranked-choice_voting_(RCV)#Background), we have created a project to simulate the ballot counting and show how a majority winner would not necessarily win a ranked-choice election.
 
The task is to implement SimpleElection.CountVotes() and RankedChoiceElection.CountVotes() in such a way that their respective ElectionRunners will return the correct winner of the election based on the voting system (you may also want to take a look at ElectionRunner.cs).

Changes can be made to anything in the solution, including the BallotGenerators, which currently just generate 100,000 random votes.
 
The submission should include a short explanation of your solution and any unit tests you used along the way.

Explanation:

For Implementing CountVotes in SimpleElection:

 creates a dictionary of candidates and their votes
 iterates through each vote and counts the votes
 finds the candidate with the most votes
 the winner is the candidate with a simple majority

For Implementing CountVotes in SimpleElection:

 create a dictionary of candidates and their votes
 iterate through each vote and counts the votes with rank 1
 find the candidate with the most votes from the tally
 the winner is the candidate with an outright majority (50% + 1)

 if there is no winner, run a n round counting while winner is null or all candidates were eliminated
 find the candidate with the least votes
 run successive counting, eliminating leastVotedCandidates and assigning their second ranked votes to the other candidates
 create a dictionary of the least voted candidate's ballots
 iterate through the ballots and adds the least voted ones to the list
 iterate through the least voted ballots and sums their respective candidate's votes with rank 2 to the tally
 add the second choice preference vote
 find the candidate with the most votes from the tally
 the winner is the candidate with an outright majority (50% + 1)

// we can have a tie (minVotes == maxVotes)
// we can have a split voting with many candidates (to be answered and checked with the technical lead)

Comments:

- After several tests, i found that when there are several candidates (i.e n > 4) we have a split voting and the outright majority is difficult to achieve. I guess we can try to improve the randomize of the ballots generation, giving some weight to certain candidates because in real life, there's strong preference for one or two candidates to be second ranked, giving more probabilities to achieve the outright majority to them on each new counting.
- Test cases include checking that we have a winner