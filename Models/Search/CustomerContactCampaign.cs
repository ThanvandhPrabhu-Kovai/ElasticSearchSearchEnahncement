using System;
using System.Collections.Generic;
using System.Text;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class CustomerContactCampaign
    {
        public CustomerContactCampaign()
        {
        }

        public CustomerContactCampaign(
            Guid campaignInstanceId,
            DateTime triggeredAt,
            DateTime respondedOn,
            string fromEmail,
            int campaignId,
            string campaignName,
            int deliveryTypeId,
            int surveyTypeId,
            string mainQuestion,
            string followUpQuestion,
            CampaignResponse campaignResponse)
        {
            this.Id = campaignId;
            this.DeliveryTypeId = deliveryTypeId;
            this.Name = campaignName;
            this.SurveyTypeId = surveyTypeId;
            this.MainQuestion = mainQuestion;
            this.FollowUpQuestion = followUpQuestion;
            this.InstanceId = campaignInstanceId.ToString();
            this.FromEmail = fromEmail;
            this.TriggeredAt = triggeredAt;
            this.RespondedOn = respondedOn;
            this.Response = campaignResponse;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int DeliveryTypeId { get; set; }

        public int SurveyTypeId { get; set; }

        public string MainQuestion { get; set; }

        public string FollowUpQuestion { get; set; }

        public string InstanceId { get; set; }

        public string FromEmail { get; set; }

        public DateTime TriggeredAt { get; set; }

        public DateTime RespondedOn { get; set; }

        public CampaignResponse Response { get; set; }
    }
}
