using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyYoutube.Item
{
    public class ItemScript : MonoBehaviour
    {
        public Search.ItemsVideo attribute;

        public void Clicked()
        {
            GrabVideo.Instance.URL = attribute.url;
            GrabVideo.Instance.ShowVideo();
        }
    }
}

