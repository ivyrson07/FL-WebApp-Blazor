using FL.Common.Models;
using Newtonsoft.Json;

namespace FL.WebApp.Blazor.Services
{
    public interface ICommentService
    {
        Task<Comment> GetAsync(Guid id);

        Task<Comment> CreateAsync(Comment comment);
    }

    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;

        public CommentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Comment> GetAsync(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<Comment>($"https://localhost:44349/comments/{id}");

            if (response != null)
                return response;
            else
            {
                Console.WriteLine($"Response is null");

                return new Comment();
            }
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:44349/comments/create", new Comment
                {
                    PostId = comment.PostId,
                    Description = comment.Description
                });

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var incomingComment = JsonConvert.DeserializeObject<Comment>(responseString);

                    if (incomingComment != null && incomingComment.Description != null)
                    {
                        return new Comment
                        {
                            PostId = incomingComment.PostId,
                            Description = incomingComment.Description
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on API call /comments/create : {ex.Message}");
                Console.WriteLine($"Error on API call /comments/create : {ex.StackTrace}");
            }

            return new Comment();
        }
    }
}
