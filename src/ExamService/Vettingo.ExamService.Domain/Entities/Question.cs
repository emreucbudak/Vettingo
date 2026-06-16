using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Domain.Entities
{
    public class Question
    {
        public Question()
        {
        }

        public Guid Id { get; private set; }
        public Guid ExamId { get; private set; }
        public Exam? Exam { get; private set; }
        public string QuestionText { get; private set; } = string.Empty;
        public string Topic { get; private set; } = string.Empty;
        public QuestionType QuestionType { get; private set; }
        public int Point { get; private set; }
        public int DisplayOrder { get; private set; }
        public string Explanation { get; private set; } = string.Empty;
        public List<QuestionOption> Options { get; private set; } = new();
        public List<ExamAnswer> Answers { get; private set; } = new();

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void setExamId(Guid examId)
        {
            ExamId = examId;
        }

        public void setQuestionText(string questionText)
        {
            QuestionText = questionText;
        }

        public void setTopic(string topic)
        {
            Topic = topic;
        }

        public void setQuestionType(QuestionType questionType)
        {
            QuestionType = questionType;
        }

        public void setPoint(int point)
        {
            Point = point;
        }

        public void setDisplayOrder(int displayOrder)
        {
            DisplayOrder = displayOrder;
        }

        public void setExplanation(string explanation)
        {
            Explanation = explanation;
        }

        public void CreateQuestion(Guid examId, string questionText, string topic, QuestionType questionType, int point, int displayOrder, string explanation)
        {
            SetId();
            setExamId(examId);
            setQuestionText(questionText);
            setTopic(topic);
            setQuestionType(questionType);
            setPoint(point);
            setDisplayOrder(displayOrder);
            setExplanation(explanation);
        }

        public void UpdateQuestion(string questionText, string topic, QuestionType questionType, int point, int displayOrder, string explanation)
        {
            setQuestionText(questionText);
            setTopic(topic);
            setQuestionType(questionType);
            setPoint(point);
            setDisplayOrder(displayOrder);
            setExplanation(explanation);
        }

        public void AddOption(QuestionOption option)
        {
            Options.Add(option);
        }

        public void ClearOptions()
        {
            Options.Clear();
        }
    }
}
