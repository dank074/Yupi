using System.Collections.Generic;
using System.Linq;
using Yupi.Emulator.Game.Polls.Enums;
using Yupi.Emulator.Messages.Buffers;

namespace Yupi.Emulator.Game.Polls
{
    /// <summary>
    ///     Class PollQuestion.
    /// </summary>
    internal class PollQuestion
    {
        /// <summary>
        ///     The answers
        /// </summary>
        internal List<string> Answers;

        /// <summary>
        ///     a type
        /// </summary>
        internal PollAnswerType AType;

        /// <summary>
        ///     The correct answer
        /// </summary>
        internal string CorrectAnswer;

        /// <summary>
        ///     The index
        /// </summary>
        internal uint Index;

        /// <summary>
        ///     The question
        /// </summary>
        internal string Question;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PollQuestion" /> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="question">The question.</param>
        /// <param name="aType">a type.</param>
        /// <param name="answers">The answers.</param>
        /// <param name="correctAnswer">The correct answer.</param>
        internal PollQuestion(uint index, string question, int aType, IEnumerable<string> answers, string correctAnswer)
        {
            Index = index;
            Question = question;
            AType = (PollAnswerType) aType;
            Answers = answers.ToList();
            CorrectAnswer = correctAnswer;
        }

        /// <summary>
        ///     Serializes the specified messageBuffer.
        /// </summary>
        /// <param name="message">The messageBuffer.</param>
        /// <param name="questionNumber">The question number.</param>
        public void Serialize(SimpleServerMessageBuffer messageBuffer, int questionNumber)
        {
            messageBuffer.AppendInteger(Index);
            messageBuffer.AppendInteger(questionNumber);
            messageBuffer.AppendInteger((int) AType);
            messageBuffer.AppendString(Question);

            if (AType != PollAnswerType.Selection && AType != PollAnswerType.RadioSelection)
                return;

            messageBuffer.AppendInteger(1);
            messageBuffer.AppendInteger(Answers.Count);

            foreach (string current in Answers)
            {
                messageBuffer.AppendString(current);
                messageBuffer.AppendString(current);
            }
        }
    }
}