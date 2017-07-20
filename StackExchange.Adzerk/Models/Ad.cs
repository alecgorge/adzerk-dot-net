using System;

namespace StackExchange.Adzerk.Models
{
    public enum DistributionType
    {
        AutoBalanced = 1,
        Percentage = 2,
        FixedNumberOfImpressions = 3
    }


    public class Ad
    {
        public long Id { get; set; }
        public long CampaignId { get; set; }
        public long FlightId { get; set; }
        public long SiteId { get; set; }
        public long ZoneId { get; set; }
        public bool Iframe { get; set; }
        public long PublisherAccountId { get; set; }
        public long Impressions { get; set; }
        public long Percentage { get; set; }
        public DistributionType DistributionType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string CustomTargeting { get; set; }
        public int FreqCap { get; set; }
        public int FreqCapDuration { get; set; }
        public FreqCapType FreqCapType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public GoalType GoalType { get; set; }
        public int Goal { get; set; }
        public bool IsGoalOverride { get; set; }
        public bool IsStartDateOverride { get; set; }
        public string[] ActiveKeywords { get; set; }
        public string RtbCustomFields { get; set; }
        public Creative Creative { get; set; }
    }


    public class AdDTO
    {
        public long Id { get; set; }
        public long CampaignId { get; set; }
        public long FlightId { get; set; }
        public long SiteId { get; set; }
        public long ZoneId { get; set; }
        public bool Iframe { get; set; }
        public long PublisherAccountId { get; set; }
        public long Impressions { get; set; }
        public long Percentage { get; set; }
        public int DistributionType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string CustomTargeting { get; set; }
        public int FreqCap { get; set; }
        public int FreqCapDuration { get; set; }
        public int FreqCapType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int GoalType { get; set; }
        public int Goal { get; set; }
        public bool IsGoalOverride { get; set; }
        public bool IsStartDateOverride { get; set; }
        public string[] ActiveKeywords { get; set; }
        public string RtbCustomFields { get; set; }
        public Creative Creative { get; set; }

        public Ad ToAd()
        {
            return new Ad()
            {
                Id = Id,
                CampaignId = CampaignId,
                FlightId = FlightId,
                SiteId = SiteId,
                ZoneId = ZoneId,
                Iframe = Iframe,
                PublisherAccountId = PublisherAccountId,
                Impressions = Impressions,
                Percentage = Percentage,
                DistributionType = (DistributionType)DistributionType,
                IsDeleted = IsDeleted,
                IsActive = IsActive,
                CustomTargeting = CustomTargeting,
                FreqCap = FreqCap,
                FreqCapDuration = FreqCapDuration,
                FreqCapType = (FreqCapType)FreqCapType,
                StartDate = AdzerkDateTimeHelpers.ParseAdzerkDate(StartDate),
                EndDate = AdzerkDateTimeHelpers.ParseAdzerkDate(EndDate),
                GoalType = (GoalType)GoalType,
                Goal = Goal,
                IsGoalOverride = IsGoalOverride,
                IsStartDateOverride = IsStartDateOverride,
                ActiveKeywords = ActiveKeywords,
                RtbCustomFields = RtbCustomFields,
                Creative = Creative
            };
        }
    }
}
