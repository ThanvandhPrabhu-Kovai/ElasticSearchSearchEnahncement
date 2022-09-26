namespace Kovai.Churn360.Customers.Core.Models
{
    public class CampaignResponse
    {
        public CampaignResponse()
        {
        }

        public CampaignResponse(
            int score,
            string responderSentiment,
            string followUpAnswer)
        {
            this.Score = score;
            this.ResponderSentiment = responderSentiment;
            this.FollowUpAnswer = followUpAnswer;
        }

        public int Score { get; set; }

        public string ResponderSentiment { get; set; }

        public string FollowUpAnswer { get; set; }
    }
}
