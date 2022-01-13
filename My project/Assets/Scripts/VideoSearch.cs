using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using YoutubeExplode;
using YoutubeExplode.Search;

namespace MyYoutube.Search
{
    public class VideoSearch : MonoBehaviour
    {
        public Transform content;
        public GameObject itemPrefabs;

        public Text SearchBar;

        public async void ShowResult()
        {
            if (!SearchBar.text.Equals(""))
            {
                VideoHelper.ClearSearch();
                foreach (Transform child in content)
                {
                    Destroy(child.gameObject);
                }

                await VideoHelper.SearchYT(SearchBar.text);

                foreach (var item in VideoHelper.dataVideo)
                {
                    var obj = Instantiate(itemPrefabs, content);
                    ItemsVideo items = obj.GetComponent<Item.ItemScript>().attribute;

                    items.title.text = item.title;
                    items.url = item.url;
                    StartCoroutine(GetThumbnail(item.id, items.img));
                }
            }
        }

        async void ShowPlaylist(string link)
        {
            var youtube = new YoutubeClient();

            // Each batch corresponds to one request
            await foreach (var batch in youtube.Playlists.GetVideoBatchesAsync(link))
            {
                foreach (var video in batch.Items)
                {
                    GameObject obj = Instantiate(itemPrefabs, content);
                    ItemsVideo item = obj.GetComponent<Item.ItemScript>().attribute;

                    item.title.text = video.Title;
                    StartCoroutine(GetThumbnail(video.Id, item.img));
                }
            }
        }

        public IEnumerator GetThumbnail(string id, RawImage imageReturn)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://img.youtube.com/vi/" + id + "/mqdefault.jpg");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var thumb = ((DownloadHandlerTexture)www.downloadHandler).texture;
                thumb.name = "thumb - " + id;
                imageReturn.texture = thumb;
            }
        }

    }

    [System.Serializable]
    public class ItemsVideo
    {
        public RawImage img;
        public Text title;
        public string url;
    }

    public class DataVideo
    {
        public string id, title, url;
        public System.TimeSpan? duration;

        public DataVideo(string id, string title, string url)
        {
            this.id = id;
            this.title = title;
            this.url = url;
        }
    }

    public class DataPlaylist
    {
        public string id, title, url;

        public DataPlaylist(string id, string title, string url)
        {
            this.id = id;
            this.title = title;
            this.url = url;
        }
    }

    public class DataChannel
    {
        public string id, title, url;

        public DataChannel(string id, string title, string url)
        {
            this.id = id;
            this.title = title;
            this.url = url;
        }
    }

    public class VideoHelper
    {
        public static List<DataVideo> dataVideo = new List<DataVideo>();
        public static List<DataPlaylist> dataPlaylist = new List<DataPlaylist>();
        public static List<DataChannel> dataChannel = new List<DataChannel>();

        public static async Task SearchYT(string query)
        {
            var youtube = new YoutubeClient();
            int i = 0,max = 20;
            await foreach (var result in youtube.Search.GetResultsAsync(query))
            {
                if (i == max)
                {
                    return;
                }
                else
                {
                    // Use pattern matching to handle different results (videos, playlists, channels)
                    switch (result)
                    {
                        case VideoSearchResult videoResult:
                            dataVideo.Add(new DataVideo(videoResult.Id, videoResult.Title, videoResult.Url));
                            break;
                        case PlaylistSearchResult playlistResult:
                            dataPlaylist.Add(new DataPlaylist(playlistResult.Id, playlistResult.Title, playlistResult.Url));
                            break;
                        case ChannelSearchResult channelResult:
                            dataChannel.Add(new DataChannel(channelResult.Id, channelResult.Title, channelResult.Url));
                            break;
                    }
                    i++;
                }
            }
            

        }

        public static void ClearSearch()
        {
            
            dataVideo.Clear();
            dataPlaylist.Clear();
            dataChannel.Clear();
        }
    }

}



