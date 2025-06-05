using Supabase.Postgrest.Models;
namespace Time_Managmeent_System.SUPABASE;

public class UserData : ContentPage
{
	[Table("Employee")]
	class UserDataTable : BaseModel
	{

        [PrimaryKey("id")]
        public int Id { get; set; }

    }
        public UserData()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}
}