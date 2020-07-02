// Blu-Ray Discord Bot
//
// Copyright © 2020, The Blu-Ray authors 
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class Google
    {
        public class SearchResult
        {
            internal SearchResult(string title, string url, string description)
            {
                Description = description;
                Title = title;
                URL = url;
            }

            public string Description { get; private set; }
            public string Title { get; private set; }
            public string URL { get; private set; }
        }

        private static CustomsearchService service;
        private static string searchId;

        public static void InitializeService(string apikey, string cx)
        {
            service = new CustomsearchService(new BaseClientService.Initializer
            {
                ApiKey          = apikey,
                ApplicationName = "Blu-Ray"
            });

            searchId = cx;
        }

        public static async Task<SearchResult> SearchAsync(string query)
        {
            CseResource.ListRequest request = service.Cse.List();

            request.Cx  = searchId;
            request.Q   = query;

            Search data = await request.ExecuteAsync();
            if (data.Items.Count == 0) throw new FileNotFoundException();

            return new SearchResult(data.Items[0].Title, data.Items[0].FormattedUrl, data.Items[0].Snippet);
        }
    }
}
