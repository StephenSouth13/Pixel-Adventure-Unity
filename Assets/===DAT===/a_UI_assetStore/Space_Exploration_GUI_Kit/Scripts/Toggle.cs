using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceGUI
{
    public class Toggle : MonoBehaviour
    {
        public Image first;
        public Image second;
        public Image third;
        public Image fourth;
        public Image background;
        int index;


        void Update()
        {
            if (index == 1)
            {
                background.gameObject.SetActive(false);
            }
            if (index == 0)
            {
                background.gameObject.SetActive(true);
            }
        }
        public void On()
        {
            index = 1;
            second.gameObject.SetActive(true);
            first.gameObject.SetActive(false);
            third.gameObject.SetActive(true);
            fourth.gameObject.SetActive(false);
        }
        public void Off()
        {
            index = 0;
            first.gameObject.SetActive(true);
            second.gameObject.SetActive(false);
            third.gameObject.SetActive(false);
            fourth.gameObject.SetActive(true);
        }
    }
}