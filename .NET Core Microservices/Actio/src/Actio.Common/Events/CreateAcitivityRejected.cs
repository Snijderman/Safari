namespace Actio.Common.Events
{
   public class CreateAcitivityRejected : IRejectedEvent
   {
      public string Reason { get; }
      public string Code { get; }
   }
}