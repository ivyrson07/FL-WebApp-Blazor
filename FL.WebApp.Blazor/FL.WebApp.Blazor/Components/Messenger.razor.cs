using EasyNetQ;
using FL.Common.Models;
using FL.WebApp.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FL.WebApp.Blazor.Components
{
    public partial class Messenger : ComponentBase
    {
        [Inject]
        private ICommentService? CommentApi { get; set; }

        [Inject]
        private IBus Bus { get; set; }

        public string comment = "";
        private List<string> incomingComments = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                await Bus.PubSub.SubscribeAsync<Comment>("comment", message => Task.Factory.StartNew(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(message.Description))
                    {
                        incomingComments.Add(message.Description);

                        await InvokeAsync(() =>
                        {
                            comment = "";
                            StateHasChanged();
                        });
                    }
                }).ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating EasyNetQ bus: {ex.Message}");
                Console.WriteLine($"Error creating EasyNetQ bus: {ex.StackTrace}");
            }
        }

        private async Task Send()
        {
            await CommentApi.CreateAsync(new Comment
            {
                PostId = Guid.NewGuid(),
                Description = comment
            });
        }

        private async Task SendHandleKeyPress(KeyboardEventArgs e)
        {
            await Task.Run(async () =>
            {
                if (e.Key == "Enter")
                {
                    await Send();
                }
            });
        }
    }
}
