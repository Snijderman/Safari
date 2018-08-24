namespace Actio.Common.Commands
{
   public class CreateUser : ICommand
   {
      public string Emai { get; set; }

      public string Password { get; set; }

      public string Name { get; set; }
   }
}