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

        public string _privateComment = "";
        public string _publicComment = "";
        private List<string> _incomingComments = new List<string>();
        private List<string> _incomingPrivateComments = new List<string>();
        private List<string> _incomingPublicComments = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                await Bus.PubSub.SubscribeAsync<Comment>("comments", async message => await HandlePubSubComments(message), message => message.WithTopic("test.*"));
                await Bus.PubSub.SubscribeAsync<Comment>("privatecomments", async message => await HandlePubSubPrivateComments(message), message => message.WithTopic("test.privatecomment"));
                await Bus.PubSub.SubscribeAsync<Comment>("publiccomments", async message => await HandlePubSubPublicComments(message), message => message.WithTopic("test.publiccomment"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating EasyNetQ bus: {ex.Message}");
                Console.WriteLine($"Error creating EasyNetQ bus: {ex.StackTrace}");
            }
        }

        private async Task SendToPrivate()
        {
            await CommentApi.CreateAsync(new Comment
            {
                PostId = Guid.Empty,
                Description = _privateComment
            });
        }

        private async Task SendToPublic()
        {
            await CommentApi.CreateAsync(new Comment
            {
                PostId = Guid.NewGuid(),
                Description = _publicComment
            });
        }

        private async Task HandlePubSubComments(Comment comment)
        {
            if (!string.IsNullOrWhiteSpace(comment.Description))
            {
                _incomingComments.Add(comment.Description);

                await InvokeAsync(() =>
                {
                    _privateComment = "";
                    _publicComment = "";
                    StateHasChanged();
                });
            }
        }

        private async Task HandlePubSubPrivateComments(Comment comment)
        {
            if (!string.IsNullOrWhiteSpace(comment.Description))
            {
                _incomingPrivateComments.Add(comment.Description);

                await InvokeAsync(() =>
                {
                    _privateComment = "";
                    StateHasChanged();
                });
            }
        }

        private async Task HandlePubSubPublicComments(Comment comment)
        {
            if (!string.IsNullOrWhiteSpace(comment.Description))
            {
                _incomingPublicComments.Add(comment.Description);

                await InvokeAsync(() =>
                {
                    _publicComment = "";
                    StateHasChanged();
                });
            }
        }

        private async Task HandleKeyPressSendToPrivate(KeyboardEventArgs e)
        {
            await Task.Run(async () =>
            {
                if (e.Key == "Enter")
                {
                    await SendToPrivate();
                }
            });
        }

        private async Task HandleKeyPressSendToPublic(KeyboardEventArgs e)
        {
            await Task.Run(async () =>
            {
                if (e.Key == "Enter")
                {
                    await SendToPublic();
                }
            });
        }
    }
}
