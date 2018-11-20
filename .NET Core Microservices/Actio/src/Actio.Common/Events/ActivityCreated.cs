using System;

namespace Actio.Common.Events
{
   public class ActivityCreated : IAuthenticatedEvent
   {
      protected ActivityCreated()
      { }

      public ActivityCreated(Guid id, Guid userId, string category, string name)
      {
         this.Id = id;
         this.UserId = userId;
         this.Category = category;
         this.Name = name;
      }

      public Guid Id { get; }
      public Guid UserId { get; }
      public string Category { get; }
      public string Name { get; }
      public string Description { get; }
      public DateTime CreatedAt { get; }
   }
}